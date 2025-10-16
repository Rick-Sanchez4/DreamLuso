using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Dashboard.Queries.GetAdminDashboardStats;

public class GetAdminDashboardStatsQuery : IRequest<Result<GetAdminDashboardStatsResponse, Success, Error>>
{
}

public class GetAdminDashboardStatsResponse
{
    public int TotalUsers { get; set; }
    public int TotalClients { get; set; }
    public int TotalAgents { get; set; }
    public int TotalProperties { get; set; }
    public int ActiveProperties { get; set; }
    public int TotalProposals { get; set; }
    public int PendingProposals { get; set; }
    public int TotalContracts { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal AverageContractValue { get; set; }
    public double ConversionRate { get; set; }
}

