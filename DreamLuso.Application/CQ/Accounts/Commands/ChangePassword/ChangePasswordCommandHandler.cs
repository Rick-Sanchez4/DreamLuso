using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Security.Interfaces;
using MediatR;

namespace DreamLuso.Application.CQ.Accounts.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<bool, Success, Error>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return Error.NotFound;

        // Verify current password
        var isCurrentPasswordValid = _passwordHasher.VerifyPasswordHash(
            request.CurrentPassword, 
            user.PasswordHash, 
            user.PasswordSalt);

        if (!isCurrentPasswordValid)
            return Error.InvalidCredentials;

        // Hash new password
        _passwordHasher.CreatePasswordHash(request.NewPassword, out byte[] newPasswordHash, out byte[] newPasswordSalt);
        
        user.PasswordHash = newPasswordHash;
        user.PasswordSalt = newPasswordSalt;
        
        await _unitOfWork.CommitAsync(cancellationToken);

        return Success.Ok;
    }
}

