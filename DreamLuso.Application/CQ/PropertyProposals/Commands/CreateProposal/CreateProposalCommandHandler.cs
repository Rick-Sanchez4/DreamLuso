using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands.SendNotification;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.CreateProposal;

public class CreateProposalCommandHandler : IRequestHandler<CreateProposalCommand, Result<Guid, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly ILogger<CreateProposalCommandHandler> _logger;

    public CreateProposalCommandHandler(
        IUnitOfWork unitOfWork,
        ISender sender,
        ILogger<CreateProposalCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Guid, Success, Error>> Handle(CreateProposalCommand request, CancellationToken cancellationToken)
    {
        // Verificar se a propriedade existe (usar método que inclui o agente)
        var propertyObj = await _unitOfWork.PropertyRepository.GetByIdWithAllDetailsAsync(request.PropertyId);
        if (propertyObj == null)
            return Error.NotFound;

        var property = (Property)propertyObj;

        // Verificar se o cliente existe (usar método que inclui o User)
        var clientObj = await _unitOfWork.ClientRepository.GetByIdWithFavoritesAsync(request.ClientId);
        if (clientObj == null)
            return Error.NotFound;

        var client = clientObj;

        // Verificar se já existe proposta pendente
        var hasPendingProposal = await _unitOfWork.PropertyProposalRepository.HasPendingProposalAsync(request.ClientId, request.PropertyId);
        if (hasPendingProposal)
            return new Error("ProposalAlreadyExists", "Já existe uma proposta pendente para este imóvel");

        var proposal = new PropertyProposal(
            request.PropertyId,
            request.ClientId,
            request.ProposedValue,
            request.Type,
            request.PaymentMethod,
            request.IntendedMoveDate
        )
        {
            AdditionalNotes = request.AdditionalNotes
        };

        await _unitOfWork.PropertyProposalRepository.SaveAsync(proposal);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Enviar notificação para o cliente
        var clientNotificationMessage = $"✅ Sua proposta de €{request.ProposedValue:N2} para o imóvel '{property.Title}' foi enviada com sucesso! " +
                                        $"O agente responsável analisará sua proposta e entrará em contato em breve.";

        var clientNotificationCommand = new SendNotificationCommand(
            SenderId: null, // System notification
            RecipientId: client.UserId,
            Message: clientNotificationMessage,
            Type: NotificationType.Proposal,
            Priority: NotificationPriority.Medium,
            ReferenceId: proposal.Id,
            ReferenceType: "ProposalCreated"
        );

        await _sender.Send(clientNotificationCommand, cancellationToken);
        _logger.LogInformation("Notificação de proposta criada enviada ao cliente {ClientId}", client.Id);

        // Enviar notificação para o agente
        // O property já tem o RealEstateAgent carregado via GetByIdWithAllDetailsAsync
        if (property.RealEstateAgent != null)
        {
            var agent = property.RealEstateAgent;
            var clientName = client.User?.Name?.FullName ?? "Cliente";
            var agentNotificationMessage = $"📋 Nova proposta recebida! Cliente {clientName} enviou uma proposta de €{request.ProposedValue:N2} para o imóvel '{property.Title}'. " +
                                          $"Tipo: {(request.Type == ProposalType.Purchase ? "Compra" : "Arrendamento")}.";

            var agentNotificationCommand = new SendNotificationCommand(
                SenderId: null, // System notification
                RecipientId: agent.UserId,
                Message: agentNotificationMessage,
                Type: NotificationType.Proposal,
                Priority: NotificationPriority.High,
                ReferenceId: proposal.Id,
                ReferenceType: "ProposalCreated"
            );

            await _sender.Send(agentNotificationCommand, cancellationToken);
            _logger.LogInformation("Notificação de proposta criada enviada ao agente {AgentId}", agent.Id);
        }

        return proposal.Id;
    }
}

