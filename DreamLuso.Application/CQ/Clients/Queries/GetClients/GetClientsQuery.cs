using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Clients.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Clients.Queries.GetClients;

public class GetClientsQuery : IRequest<Result<GetClientsResponse, Success, Error>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
}

public class GetClientsResponse
{
    public required IEnumerable<ClientResponse> Clients { get; init; }
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

