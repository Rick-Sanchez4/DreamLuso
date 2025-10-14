using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.Accounts.Commands.RegisterUser;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role,
    string? Phone = null
) : IRequest<Result<RegisterUserResponse, Success, Error>>;

public record RegisterUserResponse(
    Guid UserId,
    string FullName,
    string Email,
    UserRole Role,
    DateTime CreatedAt
);

