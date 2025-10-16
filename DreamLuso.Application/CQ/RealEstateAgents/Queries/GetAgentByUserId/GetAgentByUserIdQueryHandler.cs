using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.RealEstateAgents.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.RealEstateAgents.Queries.GetAgentByUserId;

public class GetAgentByUserIdQueryHandler : IRequestHandler<GetAgentByUserIdQuery, Result<AgentResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAgentByUserIdQueryHandler> _logger;

    public GetAgentByUserIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAgentByUserIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AgentResponse, Success, Error>> Handle(GetAgentByUserIdQuery request, CancellationToken cancellationToken)
    {
        var agents = (await _unitOfWork.RealEstateAgentRepository.GetAllAsync()).Cast<RealEstateAgent>();
        var agent = agents.FirstOrDefault(a => a.UserId == request.UserId);

        if (agent == null)
        {
            _logger.LogWarning("Agente não encontrado para userId: {UserId}", request.UserId);
            return Error.AgentNotFound;
        }

        var response = new AgentResponse
        {
            Id = agent.Id,
            UserId = agent.UserId,
            FullName = agent.User.Name.FullName,
            Email = agent.User.Email,
            Phone = agent.User.Phone,
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
            CreatedAt = agent.CreatedAt,
            ApprovalStatus = agent.IsActive ? "Approved" : "Pending"
        };

        _logger.LogInformation("Agente encontrado por userId: {UserId} -> AgentId: {AgentId}", request.UserId, agent.Id);

        return response;
    }
}

