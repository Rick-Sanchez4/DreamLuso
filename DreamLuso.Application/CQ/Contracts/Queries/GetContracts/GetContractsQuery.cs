using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Contracts.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Contracts.Queries.GetContracts;

public class GetContractsQuery : IRequest<Result<GetContractsResponse, Success, Error>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? ClientId { get; set; }
    public Guid? AgentId { get; set; }
    public int? Status { get; set; }
}

public class GetContractsResponse
{
    public required IEnumerable<ContractResponse> Contracts { get; init; }
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

