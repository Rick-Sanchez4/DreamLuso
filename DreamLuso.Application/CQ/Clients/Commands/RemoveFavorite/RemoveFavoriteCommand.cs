using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Clients.Commands.RemoveFavorite;

public record RemoveFavoriteCommand(
    Guid ClientId,
    Guid PropertyId
) : IRequest<Result<RemoveFavoriteResponse, Success, Error>>;

public record RemoveFavoriteResponse(
    Guid ClientId,
    Guid PropertyId,
    DateTime RemovedAt
);

