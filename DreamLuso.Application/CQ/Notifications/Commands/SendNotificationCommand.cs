using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.Notifications.Commands;

public record SendNotificationCommand(
    Guid SenderId,
    Guid RecipientId,
    string Message,
    NotificationType Type,
    NotificationPriority Priority = NotificationPriority.Medium,
    Guid? ReferenceId = null,
    string? ReferenceType = null,
    bool IsTransient = false
) : IRequest<Result<Guid, Success, Error>>;

