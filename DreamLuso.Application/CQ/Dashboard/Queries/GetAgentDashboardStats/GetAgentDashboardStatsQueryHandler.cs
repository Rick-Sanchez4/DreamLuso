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

        // Get all contracts for this agent (use GetByAgentIdAsync which includes related entities)
        var allContracts = (await _unitOfWork.ContractRepository.GetByAgentIdAsync(request.AgentId))
            .Cast<Contract>()
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

        // Calculate revenue and sales from contracts
        // Include both Active and Draft contracts (Draft contracts are created when proposal is approved)
        var activeContracts = allContracts.Where(c => 
            c.Status == ContractStatus.Active || 
            c.Status == ContractStatus.Draft).ToList();
        
        _logger.LogInformation(
            "Contratos encontrados para agente {AgentId}: Total={Total}, Active={Active}, Draft={Draft}",
            request.AgentId, allContracts.Count, 
            allContracts.Count(c => c.Status == ContractStatus.Active),
            allContracts.Count(c => c.Status == ContractStatus.Draft));
        
        // Total revenue: sum of all contract values (both Sale and Rent)
        var totalRevenue = activeContracts.Sum(c => c.Value);
        
        // Total commissions: sum of commission from contracts, or calculate 5% if not set
        var totalCommissions = activeContracts.Sum(c => 
            c.Commission ?? (c.Value * 0.05m));
        
        // Total sales: count of contracts (both Sale and Rent)
        var totalSales = activeContracts.Count;
        
        _logger.LogInformation(
            "Estatísticas calculadas para agente {AgentId}: TotalSales={TotalSales}, TotalRevenue={TotalRevenue}, TotalCommissions={TotalCommissions}",
            request.AgentId, totalSales, totalRevenue, totalCommissions);

        // Build response
        var response = new GetAgentDashboardStatsResponse
        {
            TotalProperties = allProperties.Count,
            ActiveProperties = activeProperties,
            TotalProposals = allProposals.Count,
            PendingProposals = pendingProposals,
            ScheduledVisits = scheduledVisits,
            CompletedVisits = completedVisits,
            TotalRevenue = totalRevenue,
            TotalCommissions = totalCommissions,
            TotalSales = totalSales,
            AverageRating = agent.Rating
        };

        _logger.LogInformation(
            "Agent Dashboard stats calculados para agente {AgentId}: {TotalProperties} imóveis, {TotalProposals} propostas, {ScheduledVisits} visitas agendadas",
            request.AgentId, response.TotalProperties, response.TotalProposals, response.ScheduledVisits);

        return response;
    }
}

