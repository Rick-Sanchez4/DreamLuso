namespace DreamLuso.Application.CQ.Properties.Common;

public class PropertyResponse
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public double Size { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public string? EnergyRating { get; set; }
    public int? YearBuilt { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    
    // Address
    public required string Street { get; set; }
    public required string Municipality { get; set; }
    public required string District { get; set; }
    public required string PostalCode { get; set; }
    
    // Agent
    public Guid AgentId { get; set; }
    public string AgentName { get; set; } = string.Empty;
    
    // Images
    public List<string> ImageUrls { get; set; } = [];
    
    public DateTime CreatedAt { get; set; }
}

public class PropertyDetailResponse : PropertyResponse
{
    public double? GrossArea { get; set; }
    public double? LandArea { get; set; }
    public int? WcCount { get; set; }
    public int? Floor { get; set; }
    public int? TotalFloors { get; set; }
    public int? ParkingSpaces { get; set; }
    public decimal? PricePerSqm { get; set; }
    public decimal? Condominium { get; set; }
    public string? Amenities { get; set; }
    public int? YearRenovated { get; set; }
    public string? Orientation { get; set; }
    public string? ViewType { get; set; }
    public bool HasElevator { get; set; }
    public bool HasGarage { get; set; }
    public bool HasGarden { get; set; }
    public bool HasPool { get; set; }
    public bool HasBalcony { get; set; }
    public bool HasTerrace { get; set; }
    public bool HasAirConditioning { get; set; }
    public bool HasCentralHeating { get; set; }
    public bool IsFurnished { get; set; }
    public bool PetsAllowed { get; set; }
    public string? VideoUrl { get; set; }
    public string? VirtualTourUrl { get; set; }
    public string? Parish { get; set; }
    public string? Complement { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

