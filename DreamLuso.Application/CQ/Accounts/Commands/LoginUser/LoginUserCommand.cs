using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.Accounts.Commands.LoginUser;

public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<Result<LoginUserResponse, Success, Error>>;

public record LoginUserResponse(
    Guid UserId,
    string FullName,
    string Email,
    UserRole Role,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    Guid? ClientId,
    Guid? AgentId,
    string? ProfileImageUrl
);

