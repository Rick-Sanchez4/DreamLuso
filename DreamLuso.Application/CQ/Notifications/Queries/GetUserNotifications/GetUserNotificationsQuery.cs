using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Notifications.Queries.GetUserNotifications;

public record GetUserNotificationsQuery(Guid UserId) : IRequest<Result<IEnumerable<NotificationResponse>, Success, Error>>;

