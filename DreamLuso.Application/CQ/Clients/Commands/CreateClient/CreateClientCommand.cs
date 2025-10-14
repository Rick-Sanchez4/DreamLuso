using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.Clients.Commands.CreateClient;

public record CreateClientCommand(
    Guid UserId,
    string? Nif,
    string? CitizenCard,
    ClientType Type,
    decimal? MinBudget,
    decimal? MaxBudget,
    string? PreferredContactMethod
) : IRequest<Result<CreateClientResponse, Success, Error>>;

public record CreateClientResponse(
    Guid ClientId,
    Guid UserId,
    string FullName,
    ClientType Type,
    DateTime CreatedAt
);

