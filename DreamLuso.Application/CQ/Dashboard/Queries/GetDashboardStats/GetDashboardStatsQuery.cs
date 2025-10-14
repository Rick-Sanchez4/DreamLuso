using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQuery : IRequest<Result<GetDashboardStatsResponse, Success, Error>>
{
}

public class GetDashboardStatsResponse
{
    public int TotalProperties { get; set; }
    public int ActiveProperties { get; set; }
    public int SoldProperties { get; set; }
    public int RentedProperties { get; set; }
    public int TotalClients { get; set; }
    public int TotalAgents { get; set; }
    public int PendingVisits { get; set; }
    public int ActiveContracts { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AveragePropertyPrice { get; set; }
}

