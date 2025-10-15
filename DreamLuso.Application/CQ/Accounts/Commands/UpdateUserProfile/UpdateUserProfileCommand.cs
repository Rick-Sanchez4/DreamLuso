using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Accounts.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    DateTime? DateOfBirth
) : IRequest<Result<bool, Success, Error>>;

