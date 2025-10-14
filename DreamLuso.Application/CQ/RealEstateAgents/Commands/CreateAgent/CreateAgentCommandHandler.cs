using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.RealEstateAgents.Commands.CreateAgent;

public class CreateAgentCommandHandler : IRequestHandler<CreateAgentCommand, Result<CreateAgentResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAgentCommandHandler> _logger;

    public CreateAgentCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateAgentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateAgentResponse, Success, Error>> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var userObj = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (userObj == null)
        {
            _logger.LogWarning("Utilizador não encontrado: {UserId}", request.UserId);
            return Error.UserNotFound;
        }

        var user = (User)userObj;

        // Check if agent profile already exists for this user
        var existingAgent = await _unitOfWork.RealEstateAgentRepository.GetByUserIdAsync(request.UserId);
        if (existingAgent != null)
        {
            _logger.LogWarning("Perfil de agente já existe para o utilizador: {UserId}", request.UserId);
            return Error.AgentExists;
        }

        // Check if license number already exists
        if (!string.IsNullOrWhiteSpace(request.LicenseNumber))
        {
            var existingLicense = await _unitOfWork.RealEstateAgentRepository.GetByLicenseNumberAsync(request.LicenseNumber);
            if (existingLicense != null)
            {
                _logger.LogWarning("Número de licença já existe: {LicenseNumber}", request.LicenseNumber);
                return Error.InvalidLicense;
            }
        }

        // Create agent
        var agent = new RealEstateAgent
        {
            UserId = request.UserId,
            User = user,
            LicenseNumber = request.LicenseNumber,
            LicenseExpiry = request.LicenseExpiry,
            OfficeEmail = request.OfficeEmail,
            OfficePhone = request.OfficePhone,
            CommissionRate = request.CommissionRate,
            Specialization = request.Specialization,
            Certifications = request.Certifications ?? new List<string>(),
            LanguagesSpoken = request.LanguagesSpoken ?? new List<Language>(),
            IsActive = true
        };

        // Save agent
        var savedAgent = await _unitOfWork.RealEstateAgentRepository.SaveAsync(agent);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Perfil de agente criado com sucesso: {AgentId}, User: {UserId}", savedAgent.Id, request.UserId);

        var response = new CreateAgentResponse(
            savedAgent.Id,
            savedAgent.UserId,
            user.Name.FullName,
            savedAgent.LicenseNumber,
            savedAgent.CreatedAt
        );

        return response;
    }
}

