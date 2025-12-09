using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands.SendNotification;
using DreamLuso.Application.CQ.Contracts.Commands.CreateContract;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.ApproveProposal;

public class ApproveProposalCommandHandler : IRequestHandler<ApproveProposalCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly ILogger<ApproveProposalCommandHandler> _logger;

    public ApproveProposalCommandHandler(
        IUnitOfWork unitOfWork,
        ISender sender,
        ILogger<ApproveProposalCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _sender = sender;
        _logger = logger;
    }

    public async Task<Result<bool, Success, Error>> Handle(ApproveProposalCommand request, CancellationToken cancellationToken)
    {
        var proposalObj = await _unitOfWork.PropertyProposalRepository.GetByIdAsync(request.ProposalId);
        if (proposalObj == null)
            return Error.NotFound;

        var proposal = (PropertyProposal)proposalObj;
        
        // Validar se a proposta pode ser aprovada
        if (proposal.Status == ProposalStatus.Approved)
            return new Error("ProposalAlreadyApproved", "Esta proposta já foi aprovada.");
        
        if (proposal.Status == ProposalStatus.Rejected)
            return new Error("ProposalAlreadyRejected", "Não é possível aprovar uma proposta que foi rejeitada.");
        
        if (proposal.Status == ProposalStatus.Cancelled)
            return new Error("ProposalCancelled", "Não é possível aprovar uma proposta cancelada.");
        
        if (proposal.Status == ProposalStatus.Completed)
            return new Error("ProposalCompleted", "Esta proposta já foi concluída.");
        
        // Get property and client info for notification
        var propertyObj = await _unitOfWork.PropertyRepository.GetByIdAsync(proposal.PropertyId);
        if (propertyObj == null)
        {
            _logger.LogWarning("Imóvel não encontrado: {PropertyId}", proposal.PropertyId);
            return Error.PropertyNotFound;
        }
        var property = (Property)propertyObj;
        
        var clientObj = await _unitOfWork.ClientRepository.GetByIdWithFavoritesAsync(proposal.ClientId);
        if (clientObj == null)
        {
            _logger.LogWarning("Cliente não encontrado: {ClientId}", proposal.ClientId);
            return Error.ClientNotFound;
        }
        var client = (Client)clientObj;
        
        // Verificar se o imóvel já está vendido/alugado
        if (property.Status == PropertyStatus.Sold || property.Status == PropertyStatus.Rented)
        {
            _logger.LogWarning("Imóvel já não está disponível: {PropertyId}, Status: {Status}", 
                proposal.PropertyId, property.Status);
            return new Error("PropertyUnavailable", "Este imóvel já não está disponível.");
        }
        
        // Aprovar a proposta
        proposal.Approve();
        
        // Marcar propriedade como Reservada (Reserved) ou Em Contrato (UnderContract) baseado no tipo de transação
        if (proposal.Type == ProposalType.Purchase)
        {
            // Para compra: marcar como Reservada
            property.UpdateStatus(PropertyStatus.Reserved);
            _logger.LogInformation("Imóvel {PropertyId} marcado como Reservado após aprovação de proposta de compra", property.Id);
        }
        else if (proposal.Type == ProposalType.Rent)
        {
            // Para arrendamento: marcar como Em Contrato
            property.UpdateStatus(PropertyStatus.UnderContract);
            _logger.LogInformation("Imóvel {PropertyId} marcado como Em Contrato após aprovação de proposta de arrendamento", property.Id);
        }
        
        // Rejeitar automaticamente outras propostas pendentes para o mesmo imóvel
        var otherProposals = await _unitOfWork.PropertyProposalRepository.GetByPropertyAsync(proposal.PropertyId);
        var pendingProposals = otherProposals
            .Where(p => p.Id != proposal.Id && 
                       (p.Status == ProposalStatus.Pending || 
                        p.Status == ProposalStatus.UnderAnalysis || 
                        p.Status == ProposalStatus.InNegotiation))
            .ToList();
        
        foreach (var otherProposal in pendingProposals)
        {
            var otherProposalObj = (PropertyProposal)otherProposal;
            otherProposalObj.Reject("Outra proposta foi aprovada para este imóvel.");
            _logger.LogInformation("Proposta {ProposalId} rejeitada automaticamente porque outra proposta foi aprovada", 
                otherProposalObj.Id);
        }
        
        // Obter o agente do imóvel (carregar com User para notificações)
        var agentObj = await _unitOfWork.RealEstateAgentRepository.GetByIdAsync(property.RealEstateAgentId);
        if (agentObj == null)
        {
            _logger.LogWarning("Agente não encontrado: {AgentId}", property.RealEstateAgentId);
            return Error.AgentNotFound;
        }
        var agent = (RealEstateAgent)agentObj;
        
        // Carregar User do agente se não estiver carregado
        if (agent.User == null && agent.UserId != Guid.Empty)
        {
            var agentUser = await _unitOfWork.UserRepository.GetByIdAsync(agent.UserId);
            if (agentUser != null)
            {
                agent.User = (User)agentUser;
                _logger.LogInformation("User do agente carregado: {UserId}", agent.UserId);
            }
            else
            {
                _logger.LogWarning("User do agente não encontrado: {UserId}", agent.UserId);
            }
        }
        
        // Criar contrato automaticamente após aprovar a proposta
        ContractType contractType = proposal.Type == ProposalType.Purchase 
            ? ContractType.Sale 
            : ContractType.Rent;
        
        // Calcular datas do contrato
        DateTime startDate = proposal.IntendedMoveDate ?? DateTime.UtcNow;
        
        // Garantir que a data de início não seja no passado
        if (startDate < DateTime.UtcNow.Date)
        {
            startDate = DateTime.UtcNow.Date;
            _logger.LogInformation("Data de início do contrato ajustada para hoje: {StartDate}", startDate);
        }
        
        DateTime? endDate = null;
        decimal? monthlyRent = null;
        
        // Para arrendamento, definir valores padrão
        if (proposal.Type == ProposalType.Rent)
        {
            monthlyRent = proposal.ProposedValue;
            // Contrato de arrendamento padrão: 1 ano
            endDate = startDate.AddYears(1);
        }
        
        // Calcular comissão (padrão: 5% do valor)
        decimal commission = proposal.ProposedValue * 0.05m;
        
        // Criar contrato em rascunho
        var createContractCommand = new CreateContractCommand(
            PropertyId: proposal.PropertyId,
            ClientId: proposal.ClientId,
            RealEstateAgentId: property.RealEstateAgentId,
            Type: contractType,
            Value: proposal.ProposedValue,
            StartDate: startDate,
            EndDate: endDate,
            MonthlyRent: monthlyRent,
            SecurityDeposit: proposal.Type == ProposalType.Rent ? proposal.ProposedValue * 2 : null, // Caução: 2 meses de renda
            Commission: commission,
            PaymentFrequency: proposal.Type == ProposalType.Rent ? PaymentFrequency.Monthly : null,
            PaymentDay: proposal.Type == ProposalType.Rent ? startDate.Day : null,
            AutoRenewal: proposal.Type == ProposalType.Rent,
            TermsAndConditions: $"Contrato gerado automaticamente a partir da proposta aprovada {proposal.ProposalNumber}. " +
                               $"Valor acordado: €{proposal.ProposedValue:N2}. " +
                               $"Método de pagamento: {proposal.PaymentMethod ?? "A definir"}. " +
                               (proposal.AdditionalNotes != null ? $"Notas: {proposal.AdditionalNotes}" : "")
        );
        
        var contractResult = await _sender.Send(createContractCommand, cancellationToken);
        
        if (contractResult.IsSuccess)
        {
            _logger.LogInformation("Contrato {ContractId} criado automaticamente após aprovação da proposta {ProposalId}", 
                contractResult.Value!.ContractId, proposal.Id);
            
            // Marcar proposta como concluída após criar o contrato
            proposal.Status = ProposalStatus.Completed;
        }
        else
        {
            _logger.LogWarning("Erro ao criar contrato após aprovar proposta {ProposalId}: {Error}", 
                proposal.Id, contractResult.Error?.Description);
            // Continuar mesmo se o contrato não for criado (pode ser criado manualmente depois)
        }
        
        await _unitOfWork.CommitAsync(cancellationToken);

        // Send notification to client
        if (property != null && client != null)
        {
            string notificationMessage;
            if (contractResult.IsSuccess)
            {
                notificationMessage = $"🎉 Ótimas notícias! Sua proposta de €{proposal.ProposedValue:N2} para o imóvel '{property.Title}' foi APROVADA! " +
                                     $"Um contrato foi criado automaticamente e está aguardando revisão. " +
                                     $"Entraremos em contato em breve para finalizar os detalhes.";
            }
            else
            {
                notificationMessage = $"🎉 Ótimas notícias! Sua proposta de €{proposal.ProposedValue:N2} para o imóvel '{property.Title}' foi APROVADA! " +
                                     $"Entraremos em contato em breve para os próximos passos e criação do contrato.";
            }
            
            var notificationCommand = new SendNotificationCommand(
                SenderId: null, // System notification
                RecipientId: client.UserId,
                Message: notificationMessage,
                Type: NotificationType.Proposal,
                Priority: NotificationPriority.High,
                ReferenceId: proposal.Id,
                ReferenceType: "ProposalApproved"
            );

            await _sender.Send(notificationCommand, cancellationToken);
            
            // Enviar notificação ao agente sobre o contrato criado
            if (contractResult.IsSuccess && agent.User != null && agent.UserId != Guid.Empty)
            {
                var agentNotificationMessage = $"📄 Novo contrato criado! Um contrato foi gerado automaticamente após a aprovação da proposta {proposal.ProposalNumber} " +
                                              $"do cliente {client.User?.Name?.FullName ?? "Cliente"} para o imóvel '{property.Title}'. " +
                                              $"O contrato está em rascunho e aguarda sua revisão.";
                
                var agentNotificationCommand = new SendNotificationCommand(
                    SenderId: null,
                    RecipientId: agent.UserId,
                    Message: agentNotificationMessage,
                    Type: NotificationType.Contract,
                    Priority: NotificationPriority.Medium,
                    ReferenceId: contractResult.Value!.ContractId,
                    ReferenceType: "ContractCreated"
                );
                
                var agentNotificationResult = await _sender.Send(agentNotificationCommand, cancellationToken);
                
                if (agentNotificationResult.IsSuccess)
                {
                    _logger.LogInformation("Notificação enviada com sucesso ao agente {AgentId} sobre contrato {ContractId} criado", 
                        agent.Id, contractResult.Value!.ContractId);
                }
                else
                {
                    _logger.LogWarning("Erro ao enviar notificação ao agente {AgentId}: {Error}", 
                        agent.Id, agentNotificationResult.Error?.Description);
                }
            }
            else if (contractResult.IsSuccess && (agent.User == null || agent.UserId == Guid.Empty))
            {
                _logger.LogWarning("Não foi possível enviar notificação ao agente: User é null ou UserId é vazio. AgentId: {AgentId}, UserId: {UserId}", 
                    agent.Id, agent.UserId);
            }
            
            _logger.LogInformation("Proposta {ProposalId} aprovada e notificação enviada ao cliente {ClientId}", 
                request.ProposalId, client.Id);
        }

        return Success.Ok;
    }
}

