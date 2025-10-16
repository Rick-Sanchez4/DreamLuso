using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.RealEstateAgents.Commands.ApproveAgent;

public class ApproveAgentCommandHandler : IRequestHandler<ApproveAgentCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly ILogger<ApproveAgentCommandHandler> _logger;

    public ApproveAgentCommandHandler(
        IUnitOfWork _unitOfWork,
        ISender sender,
        ILogger<ApproveAgentCommandHandler> logger)
    {
        this._unitOfWork = _unitOfWork;
        _sender = sender;
        _logger = logger;
    }

    public async Task<Result<bool, Success, Error>> Handle(ApproveAgentCommand request, CancellationToken cancellationToken)
    {
        var agentObj = await _unitOfWork.RealEstateAgentRepository.GetByIdAsync(request.AgentId);
        if (agentObj == null)
            return Error.AgentNotFound;

        var agent = (RealEstateAgent)agentObj;
        
        // Get the user associated with this agent
        var user = await _unitOfWork.UserRepository.GetByIdAsync(agent.UserId);
        if (user == null)
            return Error.UserNotFound;

        // Update agent and user status
        if (request.IsApproved)
        {
            agent.IsActive = true;
            user.IsActive = true; // Activate user account when agent is approved
            
            // Send approval notification
            var approvalMessage = $"🎉 Parabéns! Sua solicitação para se tornar Agente Imobiliário na DreamLuso foi aprovada! " +
                                 $"Agora você pode começar a listar propriedades e atender clientes.";
            
            var notificationCommand = new SendNotificationCommand(
                SenderId: Guid.Empty, // System notification
                RecipientId: user.Id,
                Message: approvalMessage,
                Type: NotificationType.SystemAlert,
                Priority: NotificationPriority.High,
                ReferenceId: agent.Id,
                ReferenceType: "AgentApproval"
            );

            await _sender.Send(notificationCommand, cancellationToken);
            
            _logger.LogInformation("Agente {AgentId} aprovado e notificação enviada ao usuário {UserId}", 
                request.AgentId, user.Id);
        }
        else
        {
            agent.IsActive = false;
            user.IsActive = false; // Keep user inactive if rejected
            
            // Send rejection notification
            var rejectionMessage = $"❌ Sua solicitação para se tornar Agente Imobiliário foi recusada. ";
            if (!string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                rejectionMessage += $"Motivo: {request.RejectionReason}. ";
            }
            rejectionMessage += "Entre em contato com o suporte para mais informações.";
            
            var notificationCommand = new SendNotificationCommand(
                SenderId: Guid.Empty, // System notification
                RecipientId: user.Id,
                Message: rejectionMessage,
                Type: NotificationType.SystemAlert,
                Priority: NotificationPriority.High,
                ReferenceId: agent.Id,
                ReferenceType: "AgentRejection"
            );

            await _sender.Send(notificationCommand, cancellationToken);
            
            _logger.LogInformation("Agente {AgentId} rejeitado e notificação enviada ao usuário {UserId}", 
                request.AgentId, user.Id);
        }

        await _unitOfWork.RealEstateAgentRepository.UpdateAsync(agent);
        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Success.Ok;
    }
}

