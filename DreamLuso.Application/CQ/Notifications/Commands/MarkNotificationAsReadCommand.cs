using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Notifications.Commands;

public record MarkNotificationAsReadCommand(Guid NotificationId) : IRequest<Result<bool, Success, Error>>;

