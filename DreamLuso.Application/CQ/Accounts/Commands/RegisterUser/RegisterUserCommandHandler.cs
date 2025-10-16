using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using DreamLuso.Security.Interfaces;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Accounts.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result<RegisterUserResponse, Success, Error>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        if (await _unitOfWork.UserRepository.EmailExistsAsync(request.Email))
        {
            _logger.LogWarning("Tentativa de registo com email existente: {Email}", request.Email);
            return Error.EmailAlreadyExists;
        }

        // Hash password
        _passwordHasher.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        // Create user
        // RealEstateAgents require admin approval, so they start as inactive
        var isActive = request.Role != UserRole.RealEstateAgent;
        
        var user = new User
        {
            Name = new Name(request.FirstName, request.LastName),
            Email = request.Email.ToLower().Trim(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Phone = request.Phone,
            Role = request.Role,
            IsActive = isActive
        };

        // Save user
        var savedUser = await _unitOfWork.UserRepository.SaveAsync(user);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Utilizador registado com sucesso: {Email}", request.Email);

        var response = new RegisterUserResponse(
            savedUser.Id,
            savedUser.Name.FullName,
            savedUser.Email,
            savedUser.Role,
            savedUser.CreatedAt
        );

        return response;
    }
}

