using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Notifications.Queries;

public record GetUnreadNotificationCountQuery(Guid UserId) : IRequest<Result<UnreadNotificationCountResponse, Success, Error>>;

