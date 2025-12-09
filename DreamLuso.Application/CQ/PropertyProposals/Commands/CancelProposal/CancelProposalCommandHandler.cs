using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands.SendNotification;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.CancelProposal;

public class CancelProposalCommandHandler : IRequestHandler<CancelProposalCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly ILogger<CancelProposalCommandHandler> _logger;

    public CancelProposalCommandHandler(
        IUnitOfWork unitOfWork,
        ISender sender,
        ILogger<CancelProposalCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _sender = sender;
        _logger = logger;
    }

    public async Task<Result<bool, Success, Error>> Handle(CancelProposalCommand request, CancellationToken cancellationToken)
    {
        var proposalObj = await _unitOfWork.PropertyProposalRepository.GetByIdAsync(request.ProposalId);
        if (proposalObj == null)
            return Error.NotFound;

        var proposal = (PropertyProposal)proposalObj;
        
        // Validar se a proposta pode ser cancelada
        if (proposal.Status == ProposalStatus.Cancelled)
            return new Error("ProposalAlreadyCancelled", "Esta proposta já foi cancelada.");
        
        if (proposal.Status == ProposalStatus.Approved)
            return new Error("ProposalAlreadyApproved", "Não é possível cancelar uma proposta que foi aprovada.");
        
        if (proposal.Status == ProposalStatus.Completed)
            return new Error("ProposalCompleted", "Esta proposta já foi concluída.");
        
        // Cancel proposal first
        proposal.Cancel();
        await _unitOfWork.CommitAsync(cancellationToken);

        // Get property with agent info for notification (usar método que inclui o agente)
        var propertyObj = await _unitOfWork.PropertyRepository.GetByIdWithAllDetailsAsync(proposal.PropertyId);
        if (propertyObj != null)
        {
            var property = (Property)propertyObj;
            var agent = property.RealEstateAgent;

            // Send notification to agent
            if (agent != null && agent.UserId != Guid.Empty)
            {
                var notificationMessage = $"ℹ️ A proposta de €{proposal.ProposedValue:N2} para o imóvel '{property.Title}' foi cancelada pelo cliente.";
                
                var notificationCommand = new SendNotificationCommand(
                    SenderId: null, // System notification
                    RecipientId: agent.UserId,
                    Message: notificationMessage,
                    Type: NotificationType.Proposal,
                    Priority: NotificationPriority.Low,
                    ReferenceId: proposal.Id,
                    ReferenceType: "ProposalCancelled"
                );

                await _sender.Send(notificationCommand, cancellationToken);
                _logger.LogInformation("Proposta {ProposalId} cancelada e notificação enviada ao agente {AgentId} (UserId: {UserId})", 
                    request.ProposalId, agent.Id, agent.UserId);
            }
            else
            {
                _logger.LogWarning("Agente não encontrado ou sem UserId para a propriedade {PropertyId} da proposta {ProposalId}", 
                    proposal.PropertyId, request.ProposalId);
            }
        }
        else
        {
            _logger.LogWarning("Propriedade {PropertyId} não encontrada para a proposta {ProposalId}. Proposta cancelada mas notificação não enviada.", 
                proposal.PropertyId, request.ProposalId);
        }

        return Success.Ok;
    }
}

