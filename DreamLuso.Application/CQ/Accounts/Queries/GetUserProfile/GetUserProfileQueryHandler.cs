using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.Accounts.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserProfileQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserProfileResponse, Success, Error>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return Error.NotFound;

        var response = new UserProfileResponse(
            user.Id,
            user.Email,
            user.Name.FirstName,
            user.Name.LastName,
            user.Phone ?? string.Empty,
            user.DateOfBirth,
            user.ProfileImageUrl,
            user.Role.ToString(),
            user.IsActive ? "Full" : "Limited",
            user.CreatedAt
        );

        return response;
    }
}

