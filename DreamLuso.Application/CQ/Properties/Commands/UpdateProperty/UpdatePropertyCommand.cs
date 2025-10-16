using DreamLuso.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DreamLuso.Application.CQ.Properties.Commands.UpdateProperty;

public record UpdatePropertyCommand(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    int Status,
    string? Amenities,
    decimal? Condominium,
    double? Size,
    int? Bedrooms,
    int? Bathrooms,
    int? Type,
    int? TransactionType,
    string? Street,
    string? Number,
    string? Parish,
    string? Municipality,
    string? District,
    string? PostalCode,
    string? Complement,
    double? GrossArea,
    double? LandArea,
    int? WcCount,
    int? Floor,
    int? ParkingSpaces,
    int? YearBuilt,
    string? EnergyRating,
    string? Orientation,
    bool? HasElevator,
    bool? HasGarage,
    bool? HasPool,
    bool? IsFurnished,
    List<IFormFile>? Images
) : IRequest<Result<UpdatePropertyResponse, Success, Error>>;

public record UpdatePropertyResponse(
    Guid Id,
    string Title,
    decimal Price,
    string Status,
    DateTime UpdatedAt
);

