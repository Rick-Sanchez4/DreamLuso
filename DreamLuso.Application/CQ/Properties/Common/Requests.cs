using Microsoft.AspNetCore.Http;

namespace DreamLuso.Application.CQ.Properties.Common;

public record CreatePropertyRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required Guid RealEstateAgentId { get; init; }
    public decimal Price { get; init; }
    public double Size { get; init; }
    public int Bedrooms { get; init; }
    public int Bathrooms { get; init; }
    public int Type { get; init; }
    public int Status { get; init; }
    public int TransactionType { get; init; }
    
    // Address
    public required string Street { get; init; }
    public required string Number { get; init; }
    public required string Parish { get; init; }
    public required string Municipality { get; init; }
    public required string District { get; init; }
    public required string PostalCode { get; init; }
    public string? Complement { get; init; }
    
    // Optional fields
    public double? GrossArea { get; init; }
    public double? LandArea { get; init; }
    public int? WcCount { get; init; }
    public int? Floor { get; init; }
    public int? ParkingSpaces { get; init; }
    public decimal? Condominium { get; init; }
    public string? Amenities { get; init; }
    public int? YearBuilt { get; init; }
    public string? EnergyRating { get; init; }
    public string? Orientation { get; init; }
    public bool HasElevator { get; init; }
    public bool HasGarage { get; init; }
    public bool HasPool { get; init; }
    public bool IsFurnished { get; init; }
    
    // Images
    public List<IFormFile>? Images { get; init; }
}

public record UpdatePropertyRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
    public int Status { get; init; }
    public string? Amenities { get; init; }
    public decimal? Condominium { get; init; }
    
    // Extended fields for complete property update
    public double? Size { get; init; }
    public int? Bedrooms { get; init; }
    public int? Bathrooms { get; init; }
    public int? Type { get; init; }
    public int? TransactionType { get; init; }
    
    // Address
    public string? Street { get; init; }
    public string? Number { get; init; }
    public string? Parish { get; init; }
    public string? Municipality { get; init; }
    public string? District { get; init; }
    public string? PostalCode { get; init; }
    public string? Complement { get; init; }
    
    // Additional fields
    public double? GrossArea { get; init; }
    public double? LandArea { get; init; }
    public int? WcCount { get; init; }
    public int? Floor { get; init; }
    public int? ParkingSpaces { get; init; }
    public int? YearBuilt { get; init; }
    public string? EnergyRating { get; init; }
    public string? Orientation { get; init; }
    public bool? HasElevator { get; init; }
    public bool? HasGarage { get; init; }
    public bool? HasPool { get; init; }
    public bool? IsFurnished { get; init; }
    
    // Images
    public List<IFormFile>? Images { get; init; }
}

