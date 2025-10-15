using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Notifications.Queries;

public record GetUserNotificationsQuery(Guid UserId) : IRequest<Result<IEnumerable<NotificationResponse>, Success, Error>>;

