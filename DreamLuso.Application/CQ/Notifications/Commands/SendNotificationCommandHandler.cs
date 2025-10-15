using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.Notifications.Commands;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Result<Guid, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SendNotificationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Guid, Success, Error>> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new Notification(
            request.SenderId,
            request.RecipientId,
            request.Message,
            request.Type,
            request.Priority,
            request.ReferenceId,
            request.ReferenceType,
            request.IsTransient
        );

        await _unitOfWork.NotificationRepository.SaveAsync(notification);
        await _unitOfWork.CommitAsync(cancellationToken);

        return notification.Id;
    }
}

