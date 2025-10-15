using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Accounts.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserProfileResponse, Success, Error>>;

public record UserProfileResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    DateTime? DateOfBirth,
    string? ImageUrl,
    string Account,
    string Access,
    DateTime CreatedAt
);

