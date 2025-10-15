using DreamLuso.Domain.Model;

namespace DreamLuso.Application.CQ.Notifications.Queries;

public record NotificationResponse(
    Guid Id,
    string SenderName,
    string Message,
    string Status,
    string Type,
    string Priority,
    Guid? ReferenceId,
    string? ReferenceType,
    DateTime CreatedAt
);

public record UnreadNotificationCountResponse(int UnreadCount);

