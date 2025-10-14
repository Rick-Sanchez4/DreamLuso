using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.RealEstateAgents.Commands.UpdateAgent;

public class UpdateAgentCommandHandler : IRequestHandler<UpdateAgentCommand, Result<UpdateAgentResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAgentCommandHandler> _logger;

    public UpdateAgentCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateAgentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<UpdateAgentResponse, Success, Error>> Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
    {
        var agentObj = await _unitOfWork.RealEstateAgentRepository.GetByIdAsync(request.Id);
        if (agentObj == null)
        {
            _logger.LogWarning("Agente não encontrado: {AgentId}", request.Id);
            return Error.AgentNotFound;
        }

        var agent = (RealEstateAgent)agentObj;

        // Check if license number already exists (excluding current agent)
        if (!string.IsNullOrWhiteSpace(request.LicenseNumber) && request.LicenseNumber != agent.LicenseNumber)
        {
            var existingLicense = await _unitOfWork.RealEstateAgentRepository.GetByLicenseNumberAsync(request.LicenseNumber);
            if (existingLicense != null)
            {
                _logger.LogWarning("Número de licença já existe: {LicenseNumber}", request.LicenseNumber);
                return Error.InvalidLicense;
            }
        }

        // Update fields
        agent.LicenseNumber = request.LicenseNumber;
        agent.LicenseExpiry = request.LicenseExpiry;
        agent.OfficeEmail = request.OfficeEmail;
        agent.OfficePhone = request.OfficePhone;
        agent.CommissionRate = request.CommissionRate;
        agent.Specialization = request.Specialization;
        agent.Bio = request.Bio;

        await _unitOfWork.RealEstateAgentRepository.UpdateAsync(agent);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Agente atualizado: {AgentId}", agent.Id);

        var response = new UpdateAgentResponse(
            agent.Id,
            agent.UpdatedAt ?? DateTime.UtcNow
        );

        return response;
    }
}

