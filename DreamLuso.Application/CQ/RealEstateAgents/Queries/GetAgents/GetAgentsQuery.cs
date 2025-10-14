using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.RealEstateAgents.Common;
using MediatR;

namespace DreamLuso.Application.CQ.RealEstateAgents.Queries.GetAgents;

public class GetAgentsQuery : IRequest<Result<GetAgentsResponse, Success, Error>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public string? Specialization { get; set; }
}

public class GetAgentsResponse
{
    public required IEnumerable<AgentResponse> Agents { get; init; }
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

