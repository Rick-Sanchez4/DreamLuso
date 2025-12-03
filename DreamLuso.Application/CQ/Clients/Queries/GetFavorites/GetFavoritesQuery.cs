using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Properties.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Clients.Queries.GetFavorites;

public record GetFavoritesQuery(
    Guid ClientId
) : IRequest<Result<GetFavoritesResponse, Success, Error>>;

public class GetFavoritesResponse
{
    public required IEnumerable<PropertyResponse> Properties { get; init; }
    public int TotalCount { get; init; }
}

