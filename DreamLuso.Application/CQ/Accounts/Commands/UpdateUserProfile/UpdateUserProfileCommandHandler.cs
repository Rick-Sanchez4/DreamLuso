using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.Accounts.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, Success, Error>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return Error.NotFound;

        // Update Name value object
        user.Name = new Name(request.FirstName, request.LastName);
        user.Phone = request.PhoneNumber;
        user.Address = request.Address;
        user.DateOfBirth = request.DateOfBirth;

        await _unitOfWork.CommitAsync(cancellationToken);

        return Success.Ok;
    }
}

