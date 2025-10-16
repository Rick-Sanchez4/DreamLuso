using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Accounts.Commands.ToggleUserStatus;

public record ToggleUserStatusCommand(Guid UserId, bool IsActive) : IRequest<Result<bool, Success, Error>>;

