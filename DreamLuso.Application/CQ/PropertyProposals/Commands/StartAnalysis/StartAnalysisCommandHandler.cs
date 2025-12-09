using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands.SendNotification;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.StartAnalysis;

public class StartAnalysisCommandHandler : IRequestHandler<StartAnalysisCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly ILogger<StartAnalysisCommandHandler> _logger;

    public StartAnalysisCommandHandler(
        IUnitOfWork unitOfWork,
        ISender sender,
        ILogger<StartAnalysisCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _sender = sender;
        _logger = logger;
    }

    public async Task<Result<bool, Success, Error>> Handle(StartAnalysisCommand request, CancellationToken cancellationToken)
    {
        var proposalObj = await _unitOfWork.PropertyProposalRepository.GetByIdAsync(request.ProposalId);
        if (proposalObj == null)
            return Error.NotFound;

        var proposal = (PropertyProposal)proposalObj;
        
        // Validar se a proposta pode iniciar análise
        if (proposal.Status == ProposalStatus.UnderAnalysis)
            return new Error("ProposalAlreadyUnderAnalysis", "Esta proposta já está em análise.");
        
        if (proposal.Status == ProposalStatus.Approved)
            return new Error("ProposalAlreadyApproved", "Não é possível iniciar análise de uma proposta aprovada.");
        
        if (proposal.Status == ProposalStatus.Rejected)
            return new Error("ProposalAlreadyRejected", "Não é possível iniciar análise de uma proposta rejeitada.");
        
        if (proposal.Status == ProposalStatus.Cancelled)
            return new Error("ProposalCancelled", "Não é possível iniciar análise de uma proposta cancelada.");
        
        if (proposal.Status == ProposalStatus.Completed)
            return new Error("ProposalCompleted", "Esta proposta já foi concluída.");
        
        // Get property and client info for notification
        var property = await _unitOfWork.PropertyRepository.GetByIdAsync(proposal.PropertyId);
        var client = await _unitOfWork.ClientRepository.GetByIdAsync(proposal.ClientId);
        
        proposal.StartAnalysis();
        await _unitOfWork.CommitAsync(cancellationToken);

        // Send notification to client
        if (property != null && client != null)
        {
            var notificationMessage = $"📋 Sua proposta de €{proposal.ProposedValue:N2} para o imóvel '{property.Title}' está agora em análise. " +
                                     $"Entraremos em contato em breve.";
            
            var notificationCommand = new SendNotificationCommand(
                SenderId: null, // System notification
                RecipientId: client.UserId,
                Message: notificationMessage,
                Type: NotificationType.Proposal,
                Priority: NotificationPriority.Medium,
                ReferenceId: proposal.Id,
                ReferenceType: "ProposalUnderAnalysis"
            );

            await _sender.Send(notificationCommand, cancellationToken);
            _logger.LogInformation("Proposta {ProposalId} iniciou análise e notificação enviada ao cliente {ClientId}", 
                request.ProposalId, client.Id);
        }

        return Success.Ok;
    }
}

