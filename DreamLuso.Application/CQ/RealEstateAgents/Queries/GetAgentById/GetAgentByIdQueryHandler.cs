using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.RealEstateAgents.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.RealEstateAgents.Queries.GetAgentById;

public class GetAgentByIdQueryHandler : IRequestHandler<GetAgentByIdQuery, Result<AgentResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAgentByIdQueryHandler> _logger;

    public GetAgentByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAgentByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AgentResponse, Success, Error>> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
    {
        var agentObj = await _unitOfWork.RealEstateAgentRepository.GetByIdAsync(request.Id);
        
        if (agentObj == null)
        {
            _logger.LogWarning("Agente não encontrado: {AgentId}", request.Id);
            return Error.AgentNotFound;
        }

        var agent = (RealEstateAgent)agentObj;

        var response = new AgentResponse
        {
            Id = agent.Id,
            UserId = agent.UserId,
            FullName = agent.User?.Name?.FullName ?? "Nome não disponível",
            Email = agent.User?.Email ?? "",
            Phone = agent.User?.Phone ?? "",
            LicenseNumber = agent.LicenseNumber,
            LicenseExpiry = agent.LicenseExpiry,
            OfficeEmail = agent.OfficeEmail,
            OfficePhone = agent.OfficePhone,
            CommissionRate = agent.CommissionRate,
            TotalSales = agent.TotalSales,
            TotalListings = agent.TotalListings,
            TotalRevenue = agent.TotalRevenue,
            Rating = agent.Rating,
            ReviewCount = agent.ReviewCount,
            IsActive = agent.IsActive,
            Specialization = agent.Specialization,
            Certifications = agent.Certifications,
            LanguagesSpoken = agent.LanguagesSpoken.Select(l => l.ToString()).ToList(),
            CreatedAt = agent.CreatedAt
        };

        _logger.LogInformation("Agente encontrado: {AgentId}", agent.Id);

        return response;
    }
}

