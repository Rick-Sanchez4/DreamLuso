using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;
using DreamLuso.Security.Interfaces;

namespace DreamLuso.Application.CQ.Accounts.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginUserResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginUserCommandHandler> _logger;

    public LoginUserCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<LoginUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<LoginUserResponse, Success, Error>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // Get user by email
        var userObj = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
        
        if (userObj == null)
        {
            _logger.LogWarning("Tentativa de login com email não existente: {Email}", request.Email);
            return Error.InvalidCredentials;
        }

        var user = (User)userObj;

        // Check if account is active
        if (!user.IsActive)
        {
            _logger.LogWarning("Tentativa de login numa conta inativa: {Email}", request.Email);
            return Error.UserInactive;
        }

        // Verify password
        if (user.PasswordHash == null || user.PasswordSalt == null)
        {
            _logger.LogError("Utilizador sem password definida: {Email}", request.Email);
            return Error.InvalidCredentials;
        }

        bool isPasswordValid = _passwordHasher.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);
        
        if (!isPasswordValid)
        {
            _logger.LogWarning("Password inválida para utilizador: {Email}", request.Email);
            return Error.InvalidCredentials;
        }

        // Update last login
        user.UpdateLastLogin();

        // Generate tokens
        var accessToken = _tokenService.GenerateToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddHours(1);

        // Set refresh token
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Utilizador autenticado com sucesso: {Email}", request.Email);

        // Get related profile IDs
        Guid? clientId = null;
        Guid? agentId = null;

        if (user.Role == UserRole.Client && user.Client != null)
            clientId = user.Client.Id;
        else if (user.Role == UserRole.RealEstateAgent && user.RealEstateAgent != null)
            agentId = user.RealEstateAgent.Id;

        var response = new LoginUserResponse(
            user.Id,
            user.Name.FullName,
            user.Email,
            user.Role,
            accessToken,
            refreshToken,
            expiresAt,
            clientId,
            agentId,
            user.ProfileImageUrl
        );

        return response;
    }
}

