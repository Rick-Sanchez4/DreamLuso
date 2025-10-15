using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.Common.Services;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.Accounts.Commands.UploadProfileImage;

public class UploadProfileImageCommandHandler : IRequestHandler<UploadProfileImageCommand, Result<string, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileUploadService _fileUploadService;

    public UploadProfileImageCommandHandler(
        IUnitOfWork unitOfWork,
        IFileUploadService fileUploadService)
    {
        _unitOfWork = unitOfWork;
        _fileUploadService = fileUploadService;
    }

    public async Task<Result<string, Success, Error>> Handle(UploadProfileImageCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return Error.NotFound;

        // Upload image
        var imagePath = await _fileUploadService.UploadFileAsync(request.Image, "profiles");
        if (string.IsNullOrEmpty(imagePath))
            return Error.InvalidInput;

        // Delete old image if exists
        if (!string.IsNullOrEmpty(user.ProfileImageUrl))
        {
            await _fileUploadService.DeleteFileAsync(user.ProfileImageUrl);
        }

        // Update user profile image
        user.ProfileImageUrl = imagePath;
        
        await _unitOfWork.CommitAsync(cancellationToken);

        return imagePath;
    }
}

