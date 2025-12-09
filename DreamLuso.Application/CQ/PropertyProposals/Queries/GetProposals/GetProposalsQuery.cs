using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.PropertyProposals.Common;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries.GetProposals;

public record GetProposalsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    int? Status = null
) : IRequest<Result<GetProposalsResponse, Success, Error>>;

public record GetProposalsResponse
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public IEnumerable<PropertyProposalResponse> Proposals { get; init; } = [];
}

