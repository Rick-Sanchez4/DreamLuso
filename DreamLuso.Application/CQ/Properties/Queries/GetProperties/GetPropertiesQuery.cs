using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Properties.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Properties.Queries.GetProperties;

public class GetPropertiesQuery : IRequest<Result<GetPropertiesResponse, Success, Error>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
    public int? Type { get; set; }
    public int? Status { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Municipality { get; set; }
    public int? MinBedrooms { get; set; }
    public int? MinBathrooms { get; set; }
    public bool? FeaturedOnly { get; set; }
    public int? TransactionType { get; set; }
    public Guid? AgentId { get; set; }
}

public class GetPropertiesResponse
{
    public required IEnumerable<PropertyResponse> Properties { get; init; }
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

