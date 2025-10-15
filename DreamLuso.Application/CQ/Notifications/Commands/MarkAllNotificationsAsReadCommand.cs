using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Notifications.Commands;

public record MarkAllNotificationsAsReadCommand(Guid UserId) : IRequest<Result<bool, Success, Error>>;

