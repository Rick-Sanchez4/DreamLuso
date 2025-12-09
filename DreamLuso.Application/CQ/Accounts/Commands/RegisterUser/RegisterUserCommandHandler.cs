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
        _logger.LogInformation("RegisterUserCommandHandler - Processing registration: Email={Email}, Role={Role}, FirstName={FirstName}, LastName={LastName}", 
            request.Email, request.Role, request.FirstName, request.LastName);
        
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

        // If user is a Client, create Client profile automatically
        if (request.Role == UserRole.Client)
        {
            // Check if client already exists (shouldn't, but just in case)
            var existingClient = await _unitOfWork.ClientRepository.GetByUserIdAsync(savedUser.Id);
            if (existingClient == null)
            {
                var client = new Client
                {
                    UserId = savedUser.Id,
                    User = savedUser,
                    Type = ClientType.Individual, // Default to Individual
                    IsActive = true
                };

                await _unitOfWork.ClientRepository.SaveAsync(client);
                await _unitOfWork.CommitAsync(cancellationToken);
                
                _logger.LogInformation("Perfil de cliente criado automaticamente para userId: {UserId}", savedUser.Id);
            }
        }
        // If user is a RealEstateAgent, create RealEstateAgent profile automatically (pending approval)
        else if (request.Role == UserRole.RealEstateAgent)
        {
            // Check if agent already exists (shouldn't, but just in case)
            var existingAgent = await _unitOfWork.RealEstateAgentRepository.GetByUserIdAsync(savedUser.Id);
            if (existingAgent == null)
            {
                var agent = new RealEstateAgent
                {
                    UserId = savedUser.Id,
                    User = savedUser,
                    IsActive = false, // Start as inactive, requires admin approval
                    LicenseNumber = null, // Will be filled later or during approval
                    CommissionRate = 0,
                    TotalSales = 0,
                    TotalRevenue = 0,
                    Rating = 0,
                    ReviewCount = 0
                };

                await _unitOfWork.RealEstateAgentRepository.SaveAsync(agent);
                await _unitOfWork.CommitAsync(cancellationToken);
                
                _logger.LogInformation("Perfil de agente imobiliário criado automaticamente para userId: {UserId} (pendente de aprovação)", savedUser.Id);
            }
        }

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

