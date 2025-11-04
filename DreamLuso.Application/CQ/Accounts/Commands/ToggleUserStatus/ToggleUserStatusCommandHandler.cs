using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Accounts.Commands.ToggleUserStatus;

public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly ILogger<ToggleUserStatusCommandHandler> _logger;

    public ToggleUserStatusCommandHandler(
        IUnitOfWork unitOfWork,
        ISender sender,
        ILogger<ToggleUserStatusCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _sender = sender;
        _logger = logger;
    }

    public async Task<Result<bool, Success, Error>> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return Error.NotFound;

        var previousStatus = user.IsActive;
        user.IsActive = request.IsActive;
        
        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Send notification to user about account status change
        if (previousStatus != request.IsActive)
        {
            var notificationMessage = request.IsActive 
                ? $"✅ Sua conta foi ativada! Bem-vindo de volta ao DreamLuso."
                : $"⚠️ Sua conta foi desativada. Entre em contato com o suporte para mais informações.";

            // System notification (SenderId = null for system messages)
            var notificationCommand = new SendNotificationCommand(
                SenderId: null, // System notification
                RecipientId: user.Id,
                Message: notificationMessage,
                Type: NotificationType.SystemAlert,
                Priority: NotificationPriority.High,
                ReferenceId: user.Id,
                ReferenceType: "UserAccount"
            );

            await _sender.Send(notificationCommand, cancellationToken);
            
            _logger.LogInformation("Usuário {UserId} teve status alterado para {IsActive} e notificação enviada", 
                request.UserId, request.IsActive);
        }

        return Success.Ok;
    }
}

