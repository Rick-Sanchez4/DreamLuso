using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Dashboard.Queries.GetAgentDashboardStats;

public class GetAgentDashboardStatsQueryHandler : IRequestHandler<GetAgentDashboardStatsQuery, Result<GetAgentDashboardStatsResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAgentDashboardStatsQueryHandler> _logger;

    public GetAgentDashboardStatsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAgentDashboardStatsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetAgentDashboardStatsResponse, Success, Error>> Handle(GetAgentDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        // Verify agent exists
        var agentObj = await _unitOfWork.RealEstateAgentRepository.GetByIdAsync(request.AgentId);
        if (agentObj == null)
        {
            _logger.LogWarning("Agente não encontrado: {AgentId}", request.AgentId);
            return Error.AgentNotFound;
        }

        var agent = (RealEstateAgent)agentObj;

        // Get all properties for this agent
        var allProperties = (await _unitOfWork.PropertyRepository.GetAllAsync())
            .Cast<Property>()
            .Where(p => p.RealEstateAgentId == request.AgentId)
            .ToList();

        // Get all proposals for agent's properties
        var allProposals = (await _unitOfWork.PropertyProposalRepository.GetAllAsync())
            .Cast<PropertyProposal>()
            .Where(p => allProperties.Any(prop => prop.Id == p.PropertyId))
            .ToList();

        // Get all visits for agent's properties
        var allVisits = (await _unitOfWork.PropertyVisitRepository.GetAllAsync())
            .Cast<PropertyVisit>()
            .Where(v => v.RealEstateAgentId == request.AgentId)
            .ToList();

        // Calculate stats
        var activeProperties = allProperties.Count(p => p.IsActive && p.Status == PropertyStatus.Available);
        var pendingProposals = allProposals.Count(p => 
            p.Status == ProposalStatus.Pending || 
            p.Status == ProposalStatus.UnderAnalysis || 
            p.Status == ProposalStatus.InNegotiation);
        var scheduledVisits = allVisits.Count(v => 
            v.Status == VisitStatus.Pending || 
            v.Status == VisitStatus.Confirmed);
        var completedVisits = allVisits.Count(v => v.Status == VisitStatus.Completed);

        // Build response
        var response = new GetAgentDashboardStatsResponse
        {
            TotalProperties = allProperties.Count,
            ActiveProperties = activeProperties,
            TotalProposals = allProposals.Count,
            PendingProposals = pendingProposals,
            ScheduledVisits = scheduledVisits,
            CompletedVisits = completedVisits,
            TotalRevenue = agent.TotalRevenue,
            TotalCommissions = agent.TotalRevenue * (agent.CommissionRate ?? 0),
            TotalSales = agent.TotalSales,
            AverageRating = agent.Rating
        };

        _logger.LogInformation(
            "Agent Dashboard stats calculados para agente {AgentId}: {TotalProperties} imóveis, {TotalProposals} propostas, {ScheduledVisits} visitas agendadas",
            request.AgentId, response.TotalProperties, response.TotalProposals, response.ScheduledVisits);

        return response;
    }
}

