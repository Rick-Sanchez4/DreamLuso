using DreamLuso.Application.Common.Responses;
using MediatR;
using DreamLuso.Domain.Model;

namespace DreamLuso.Application.CQ.Accounts.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<Result<List<UserDto>, Success, Error>>;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string? Phone,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastLogin,
    Guid? ClientId,
    Guid? AgentId
);

