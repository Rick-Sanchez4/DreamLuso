using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Notifications.Queries.GetUnreadNotificationCount;

public record GetUnreadNotificationCountQuery(Guid UserId) : IRequest<Result<UnreadNotificationCountResponse, Success, Error>>;

