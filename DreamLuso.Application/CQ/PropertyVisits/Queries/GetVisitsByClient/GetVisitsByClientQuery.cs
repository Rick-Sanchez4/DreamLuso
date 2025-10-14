using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.PropertyVisits.Common;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyVisits.Queries.GetVisitsByClient;

public class GetVisitsByClientQuery : IRequest<Result<GetVisitsByClientResponse, Success, Error>>
{
    public Guid ClientId { get; set; }
}

public class GetVisitsByClientResponse
{
    public required IEnumerable<PropertyVisitResponse> Visits { get; init; }
    public int TotalCount { get; init; }
}

