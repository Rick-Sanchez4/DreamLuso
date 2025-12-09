using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands.SendNotification;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.RejectProposal;

public class RejectProposalCommandHandler : IRequestHandler<RejectProposalCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly ILogger<RejectProposalCommandHandler> _logger;

    public RejectProposalCommandHandler(
        IUnitOfWork unitOfWork,
        ISender sender,
        ILogger<RejectProposalCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _sender = sender;
        _logger = logger;
    }

    public async Task<Result<bool, Success, Error>> Handle(RejectProposalCommand request, CancellationToken cancellationToken)
    {
        var proposalObj = await _unitOfWork.PropertyProposalRepository.GetByIdAsync(request.ProposalId);
        if (proposalObj == null)
            return Error.NotFound;

        var proposal = (PropertyProposal)proposalObj;
        
        // Validar se a proposta pode ser rejeitada
        if (proposal.Status == ProposalStatus.Rejected)
            return new Error("ProposalAlreadyRejected", "Esta proposta já foi rejeitada.");
        
        if (proposal.Status == ProposalStatus.Approved)
            return new Error("ProposalAlreadyApproved", "Não é possível rejeitar uma proposta que foi aprovada.");
        
        if (proposal.Status == ProposalStatus.Cancelled)
            return new Error("ProposalCancelled", "Não é possível rejeitar uma proposta cancelada.");
        
        if (proposal.Status == ProposalStatus.Completed)
            return new Error("ProposalCompleted", "Esta proposta já foi concluída.");
        
        // Get property and client info for notification
        var property = await _unitOfWork.PropertyRepository.GetByIdAsync(proposal.PropertyId);
        var client = await _unitOfWork.ClientRepository.GetByIdAsync(proposal.ClientId);
        
        proposal.Reject(request.RejectionReason);
        await _unitOfWork.CommitAsync(cancellationToken);

        if (property != null && client != null)
        {
            // Send notification to client
            var notificationMessage = $"❌ Sua proposta de €{proposal.ProposedValue:N2} para o imóvel '{property.Title}' foi recusada. ";
            if (!string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                notificationMessage += $"Motivo: {request.RejectionReason}. ";
            }
            notificationMessage += "Sinta-se à vontade para fazer uma nova proposta ou explorar outros imóveis.";
            
            var notificationCommand = new SendNotificationCommand(
                SenderId: null, // System notification
                RecipientId: client.UserId,
                Message: notificationMessage,
                Type: NotificationType.Proposal,
                Priority: NotificationPriority.Medium,
                ReferenceId: proposal.Id,
                ReferenceType: "ProposalRejected"
            );

            await _sender.Send(notificationCommand, cancellationToken);
            _logger.LogInformation("Proposta {ProposalId} rejeitada e notificação enviada ao cliente {ClientId}", 
                request.ProposalId, client.Id);
        }

        return Success.Ok;
    }
}

