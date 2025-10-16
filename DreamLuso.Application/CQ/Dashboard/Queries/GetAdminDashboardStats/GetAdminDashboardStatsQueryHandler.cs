using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Dashboard.Queries.GetAdminDashboardStats;

public class GetAdminDashboardStatsQueryHandler : IRequestHandler<GetAdminDashboardStatsQuery, Result<GetAdminDashboardStatsResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAdminDashboardStatsQueryHandler> _logger;

    public GetAdminDashboardStatsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAdminDashboardStatsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetAdminDashboardStatsResponse, Success, Error>> Handle(GetAdminDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        // Get all users
        var allUsers = (await _unitOfWork.UserRepository.GetAllAsync()).Cast<User>().ToList();

        // Get all properties
        var allProperties = (await _unitOfWork.PropertyRepository.GetAllAsync()).Cast<Property>().ToList();
        var activeProperties = allProperties.Where(p => p.IsActive).ToList();

        // Get all clients
        var allClients = (await _unitOfWork.ClientRepository.GetAllAsync()).Cast<Client>().ToList();

        // Get all agents
        var allAgents = (await _unitOfWork.RealEstateAgentRepository.GetAllAsync()).Cast<RealEstateAgent>().ToList();

        // Get all proposals
        var allProposals = (await _unitOfWork.PropertyProposalRepository.GetAllAsync()).Cast<PropertyProposal>().ToList();
        var pendingProposals = allProposals.Count(p => p.Status == ProposalStatus.Pending);

        // Get active contracts
        var activeContracts = (await _unitOfWork.ContractRepository.GetActiveContractsAsync()).Cast<Contract>().ToList();

        // Calculate monthly revenue (contracts from this month)
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        var monthlyContracts = activeContracts.Where(c => c.CreatedAt.Month == currentMonth && c.CreatedAt.Year == currentYear).ToList();
        var monthlyRevenue = monthlyContracts.Sum(c => c.Value);

        // Calculate average contract value
        var averageContractValue = activeContracts.Any() ? activeContracts.Average(c => c.Value) : 0;

        // Calculate conversion rate (approved proposals / total proposals * 100)
        var approvedProposals = allProposals.Count(p => p.Status == ProposalStatus.Approved);
        var conversionRate = allProposals.Count > 0 ? (double)approvedProposals / allProposals.Count * 100 : 0;

        // Build response
        var response = new GetAdminDashboardStatsResponse
        {
            TotalUsers = allUsers.Count,
            TotalClients = allClients.Count,
            TotalAgents = allAgents.Count,
            TotalProperties = allProperties.Count,
            ActiveProperties = activeProperties.Count,
            TotalProposals = allProposals.Count,
            PendingProposals = pendingProposals,
            TotalContracts = activeContracts.Count,
            MonthlyRevenue = monthlyRevenue,
            AverageContractValue = averageContractValue,
            ConversionRate = conversionRate
        };

        _logger.LogInformation("Admin dashboard stats calculados: {TotalUsers} usuários, {TotalProperties} imóveis, {TotalProposals} propostas", 
            response.TotalUsers, response.TotalProperties, response.TotalProposals);

        return response;
    }
}

