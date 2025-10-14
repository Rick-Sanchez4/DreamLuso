using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Clients.Commands.UpdateClient;

public record UpdateClientCommand(
    Guid Id,
    string? Nif,
    decimal? MinBudget,
    decimal? MaxBudget,
    string? PreferredContactMethod,
    string? PropertyPreferences
) : IRequest<Result<UpdateClientResponse, Success, Error>>;

public record UpdateClientResponse(
    Guid Id,
    DateTime UpdatedAt
);

