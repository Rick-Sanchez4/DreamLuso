using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Dashboard.Queries.GetClientDashboardStats;

public class GetClientDashboardStatsQuery : IRequest<Result<GetClientDashboardStatsResponse, Success, Error>>
{
    public Guid ClientId { get; set; }
}

public class GetClientDashboardStatsResponse
{
    public int TotalProposals { get; set; }
    public int PendingProposals { get; set; }
    public int ApprovedProposals { get; set; }
    public int RejectedProposals { get; set; }
    public int TotalVisits { get; set; }
    public int ScheduledVisits { get; set; }
    public int CompletedVisits { get; set; }
    public int TotalFavorites { get; set; }
    public int TotalContracts { get; set; }
    public int ActiveContracts { get; set; }
}

