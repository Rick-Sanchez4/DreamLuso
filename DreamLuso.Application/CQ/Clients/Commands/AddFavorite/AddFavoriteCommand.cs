using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Clients.Commands.AddFavorite;

public record AddFavoriteCommand(
    Guid ClientId,
    Guid PropertyId
) : IRequest<Result<AddFavoriteResponse, Success, Error>>;

public record AddFavoriteResponse(
    Guid ClientId,
    Guid PropertyId,
    DateTime AddedAt
);

