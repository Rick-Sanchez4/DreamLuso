using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Accounts.Commands.ChangePassword;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<Result<bool, Success, Error>>;

