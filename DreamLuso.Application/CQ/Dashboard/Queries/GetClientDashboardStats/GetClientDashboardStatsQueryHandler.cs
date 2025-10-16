using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Dashboard.Queries.GetClientDashboardStats;

public class GetClientDashboardStatsQueryHandler : IRequestHandler<GetClientDashboardStatsQuery, Result<GetClientDashboardStatsResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetClientDashboardStatsQueryHandler> _logger;

    public GetClientDashboardStatsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetClientDashboardStatsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetClientDashboardStatsResponse, Success, Error>> Handle(GetClientDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        // Verify client exists
        var clientObj = await _unitOfWork.ClientRepository.GetByIdAsync(request.ClientId);
        if (clientObj == null)
        {
            _logger.LogWarning("Cliente não encontrado: {ClientId}", request.ClientId);
            return Error.ClientNotFound;
        }

        var client = (Client)clientObj;

        // Get all proposals for this client
        var allProposals = (await _unitOfWork.PropertyProposalRepository.GetAllAsync())
            .Cast<PropertyProposal>()
            .Where(p => p.ClientId == request.ClientId)
            .ToList();

        // Get all visits for this client
        var allVisits = (await _unitOfWork.PropertyVisitRepository.GetAllAsync())
            .Cast<PropertyVisit>()
            .Where(v => v.ClientId == request.ClientId)
            .ToList();

        // Get all contracts for this client
        var allContracts = (await _unitOfWork.ContractRepository.GetAllAsync())
            .Cast<Contract>()
            .Where(c => c.ClientId == request.ClientId)
            .ToList();

        // Get all properties (to count favorites)
        var allProperties = (await _unitOfWork.PropertyRepository.GetAllAsync())
            .Cast<Property>()
            .ToList();

        // Calculate stats
        var pendingProposals = allProposals.Count(p => 
            p.Status == ProposalStatus.Pending || 
            p.Status == ProposalStatus.UnderAnalysis || 
            p.Status == ProposalStatus.InNegotiation);
        var approvedProposals = allProposals.Count(p => p.Status == ProposalStatus.Approved);
        var rejectedProposals = allProposals.Count(p => p.Status == ProposalStatus.Rejected);
        
        var scheduledVisits = allVisits.Count(v => 
            v.Status == VisitStatus.Pending || 
            v.Status == VisitStatus.Confirmed);
        var completedVisits = allVisits.Count(v => v.Status == VisitStatus.Completed);

        var activeContracts = allContracts.Count(c => c.Status == ContractStatus.Active);

        // Count favorites - this would need a Favorites table/relationship
        var totalFavorites = 0; // TODO: Implement favorites functionality

        // Build response
        var response = new GetClientDashboardStatsResponse
        {
            TotalProposals = allProposals.Count,
            PendingProposals = pendingProposals,
            ApprovedProposals = approvedProposals,
            RejectedProposals = rejectedProposals,
            TotalVisits = allVisits.Count,
            ScheduledVisits = scheduledVisits,
            CompletedVisits = completedVisits,
            TotalFavorites = totalFavorites,
            TotalContracts = allContracts.Count,
            ActiveContracts = activeContracts
        };

        _logger.LogInformation(
            "Client Dashboard stats calculados para cliente {ClientId}: {TotalProposals} propostas, {TotalVisits} visitas, {TotalContracts} contratos",
            request.ClientId, response.TotalProposals, response.TotalVisits, response.TotalContracts);

        return response;
    }
}

