using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.PropertyVisits.Common;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyVisits.Queries.GetVisitsByAgent;

public class GetVisitsByAgentQuery : IRequest<Result<GetVisitsByAgentResponse, Success, Error>>
{
    public Guid AgentId { get; set; }
}

public class GetVisitsByAgentResponse
{
    public required IEnumerable<PropertyVisitResponse> Visits { get; init; }
    public int TotalCount { get; init; }
}

