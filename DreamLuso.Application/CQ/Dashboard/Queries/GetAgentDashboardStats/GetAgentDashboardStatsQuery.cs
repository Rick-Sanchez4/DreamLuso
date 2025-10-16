using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Dashboard.Queries.GetAgentDashboardStats;

public class GetAgentDashboardStatsQuery : IRequest<Result<GetAgentDashboardStatsResponse, Success, Error>>
{
    public Guid AgentId { get; set; }
}

public class GetAgentDashboardStatsResponse
{
    public int TotalProperties { get; set; }
    public int ActiveProperties { get; set; }
    public int TotalProposals { get; set; }
    public int PendingProposals { get; set; }
    public int ScheduledVisits { get; set; }
    public int CompletedVisits { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCommissions { get; set; }
    public int TotalSales { get; set; }
    public double AverageRating { get; set; }
}

