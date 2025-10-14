using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<GetDashboardStatsResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetDashboardStatsQueryHandler> _logger;

    public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetDashboardStatsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetDashboardStatsResponse, Success, Error>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        // Get all properties
        var allProperties = (await _unitOfWork.PropertyRepository.GetAllAsync()).Cast<Property>().ToList();
        var activeProperties = allProperties.Where(p => p.IsActive).ToList();

        // Get all clients
        var allClients = (await _unitOfWork.ClientRepository.GetAllAsync()).Cast<Client>().ToList();

        // Get all agents
        var allAgents = (await _unitOfWork.RealEstateAgentRepository.GetAllAsync()).Cast<RealEstateAgent>().ToList();

        // Get pending visits
        var allVisits = (await _unitOfWork.PropertyVisitRepository.GetAllAsync()).Cast<PropertyVisit>().ToList();
        var pendingVisits = allVisits.Count(v => v.Status == VisitStatus.Pending || v.Status == VisitStatus.Confirmed);

        // Get active contracts
        var activeContracts = (await _unitOfWork.ContractRepository.GetActiveContractsAsync()).Cast<Contract>().ToList();

        // Calculate stats
        var response = new GetDashboardStatsResponse
        {
            TotalProperties = allProperties.Count,
            ActiveProperties = activeProperties.Count,
            SoldProperties = allProperties.Count(p => p.Status == PropertyStatus.Sold),
            RentedProperties = allProperties.Count(p => p.Status == PropertyStatus.Rented),
            TotalClients = allClients.Count,
            TotalAgents = allAgents.Count,
            PendingVisits = pendingVisits,
            ActiveContracts = activeContracts.Count,
            TotalRevenue = activeContracts.Sum(c => c.Value),
            AveragePropertyPrice = activeProperties.Any() ? activeProperties.Average(p => p.Price) : 0
        };

        _logger.LogInformation("Dashboard stats calculados: {ActiveProperties} imóveis ativos, {TotalClients} clientes", 
            response.ActiveProperties, response.TotalClients);

        return response;
    }
}

