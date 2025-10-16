using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands;

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
        
        // Get property and client info for notification
        // var property = await _unitOfWork.PropertyRepository.GetByIdAsync(proposal.PropertyId);
        // var client = await _unitOfWork.ClientRepository.GetByIdAsync(proposal.ClientId);
        
        // TODO: Re-enable notifications after creating migration to allow NULL SenderId
        // if (property != null && client != null)
        // {
        //     // Send notification to client
        //     var notificationMessage = $"🎉 Ótimas notícias! Sua proposta de €{proposal.ProposedValue:N2} para o imóvel '{property.Title}' foi APROVADA! " +
        //                              $"Entraremos em contato em breve para os próximos passos.";
        //     
        //     var notificationCommand = new SendNotificationCommand(
        //         SenderId: null, // System notification
        //         RecipientId: client.UserId,
        //         Message: notificationMessage,
        //         Type: NotificationType.Proposal,
        //         Priority: NotificationPriority.High,
        //         ReferenceId: proposal.Id,
        //         ReferenceType: "ProposalApproved"
        //     );
        //
        //     await _sender.Send(notificationCommand, cancellationToken);
        //     _logger.LogInformation("Proposta {ProposalId} aprovada e notificação enviada ao cliente {ClientId}", 
        //         request.ProposalId, client.Id);
        // }

        proposal.Approve();
        await _unitOfWork.CommitAsync(cancellationToken);

        return Success.Ok;
    }
}

