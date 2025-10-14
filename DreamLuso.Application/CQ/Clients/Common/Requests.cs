namespace DreamLuso.Application.CQ.Clients.Common;

public record CreateClientRequest(
    Guid UserId,
    string? Nif,
    string? CitizenCard,
    int Type,
    decimal? MinBudget,
    decimal? MaxBudget,
    string? PreferredContactMethod
);

public record UpdateClientRequest(
    string? Nif,
    string? CitizenCard,
    decimal? MinBudget,
    decimal? MaxBudget,
    string? PreferredContactMethod,
    string? PropertyPreferences
);

