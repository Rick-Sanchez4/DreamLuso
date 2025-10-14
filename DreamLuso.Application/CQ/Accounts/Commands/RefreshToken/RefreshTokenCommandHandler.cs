using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using DreamLuso.Security.Interfaces;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Accounts.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<RefreshTokenResponse, Success, Error>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Validate access token structure (without validating expiry)
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            _logger.LogWarning("Token de acesso inválido");
            return Error.InvalidToken;
        }

        // Get user by refresh token
        var userObj = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(request.RefreshToken);
        if (userObj == null)
        {
            _logger.LogWarning("Refresh token não encontrado");
            return Error.InvalidRefreshToken;
        }

        var user = (User)userObj;

        // Validate refresh token
        if (!user.IsRefreshTokenValid())
        {
            _logger.LogWarning("Refresh token expirado para utilizador: {Email}", user.Email);
            return Error.RefreshTokenExpired;
        }

        // Check if user is active
        if (!user.IsActive)
        {
            _logger.LogWarning("Tentativa de refresh token numa conta inativa: {Email}", user.Email);
            return Error.UserInactive;
        }

        // Generate new tokens
        var newAccessToken = _tokenService.GenerateToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddHours(1);

        // Update refresh token
        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Token renovado com sucesso para utilizador: {Email}", user.Email);

        var response = new RefreshTokenResponse(
            newAccessToken,
            newRefreshToken,
            expiresAt
        );

        return response;
    }
}

