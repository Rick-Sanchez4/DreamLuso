using DreamLuso.Domain.Common;
using DreamLuso.Domain.Interfaces;

namespace DreamLuso.Domain.Model;

public class Property : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public Guid RealEstateAgentId { get; set; }
    public required RealEstateAgent RealEstateAgent { get; set; }
    
    // Localização
    public required Address Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    // Tipo e Status
    public PropertyType Type { get; set; }
    public PropertyStatus Status { get; set; }
    public TransactionType TransactionType { get; set; }
    
    // Características básicas
    public double Size { get; set; }                 // Área útil em m²
    public double? GrossArea { get; set; }           // Área bruta em m²
    public double? LandArea { get; set; }            // Área do terreno em m²
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int? WcCount { get; set; }                // Número de WCs
    public int? Floor { get; set; }                  // Andar (para apartamentos)
    public int? TotalFloors { get; set; }            // Total de andares do edifício
    public int? ParkingSpaces { get; set; }          // Vagas de garagem
    
    // Preços e valores
    public decimal Price { get; set; }
    public decimal? PricePerSqm { get; set; }        // Preço por m²
    public decimal? Condominium { get; set; }        // Valor do condomínio
    public decimal? Imt { get; set; }                // IMT (Imposto Municipal sobre Transações)
    public decimal? Imi { get; set; }                // IMI (Imposto Municipal sobre Imóveis)
    
    // Características adicionais
    public string? Amenities { get; set; }           // Comodidades (JSON)
    public int? YearBuilt { get; set; }
    public int? YearRenovated { get; set; }
    public string? EnergyRating { get; set; }        // Certificação energética (A+, A, B, etc.)
    public string? Orientation { get; set; }         // Orientação solar
    public string? ViewType { get; set; }            // Tipo de vista
    public string? NearbyPoints { get; set; }        // Pontos de interesse próximos (JSON)
    
    // Características do edifício
    public bool HasElevator { get; set; }
    public bool HasGarage { get; set; }
    public bool HasGarden { get; set; }
    public bool HasPool { get; set; }
    public bool HasBalcony { get; set; }
    public bool HasTerrace { get; set; }
    public bool HasAirConditioning { get; set; }
    public bool HasCentralHeating { get; set; }
    public bool HasFireplace { get; set; }
    public bool HasStorageRoom { get; set; }
    public bool IsFurnished { get; set; }
    public bool PetsAllowed { get; set; }
    
    // Documentação e legal
    public string? LicenseNumber { get; set; }       // Licença de utilização
    public string? RegistryNumber { get; set; }      // Número de registo predial
    public string? TaxId { get; set; }               // Artigo matricial
    
    // Marketing e visibilidade
    public DateTime DateListed { get; set; }
    public DateTime? DateSold { get; set; }
    public int ViewCount { get; set; } = 0;
    public int FavoriteCount { get; set; } = 0;
    public double Rating { get; set; } = 0;
    public bool IsFeatured { get; set; } = false;    // Destaque
    public bool IsActive { get; set; } = true;
    
    // Imagens e mídia
    public List<PropertyImage> Images { get; set; }
    public string? VideoUrl { get; set; }
    public string? VirtualTourUrl { get; set; }
    
    // Navigation Properties
    public List<PropertyVisit> PropertyVisits { get; set; }
    public List<Contract> Contracts { get; set; }
    public List<Client> InterestedClients { get; set; }

    public Property()
    {
        Id = Guid.NewGuid();
        Images = [];
        PropertyVisits = [];
        Contracts = [];
        InterestedClients = [];
        DateListed = DateTime.UtcNow;
    }

    public void IncrementViewCount()
    {
        ViewCount++;
        UpdateTimestamp();
    }

    public void UpdatePrice(decimal newPrice)
    {
        Price = newPrice;
        PricePerSqm = Size > 0 ? newPrice / (decimal)Size : null;
        UpdateTimestamp();
    }

    public void MarkAsSold()
    {
        Status = PropertyStatus.Sold;
        DateSold = DateTime.UtcNow;
        IsActive = false;
        UpdateTimestamp();
    }

    public void MarkAsRented()
    {
        Status = PropertyStatus.Rented;
        IsActive = false;
        UpdateTimestamp();
    }

    public void UpdateStatus(PropertyStatus newStatus)
    {
        Status = newStatus;
        
        if (newStatus == PropertyStatus.Sold || newStatus == PropertyStatus.Rented)
        {
            IsActive = false;
            DateSold = DateTime.UtcNow;
        }
        
        UpdateTimestamp();
    }
}

public enum PropertyType
{
    House,          // Moradia
    Apartment,      // Apartamento
    Condo,          // Condomínio fechado
    Townhouse,      // Casa geminada
    Land,           // Terreno
    Commercial,     // Comercial
    Office,         // Escritório
    Warehouse,      // Armazém
    Farm,           // Quinta
    Villa,          // Vivenda
    Studio,         // Estúdio
    Penthouse,      // Penthouse
    Other
}

public enum PropertyStatus
{
    Available,      // Disponível
    Reserved,       // Reservado
    UnderContract,  // Em contrato
    Sold,           // Vendido
    Rented,         // Arrendado
    Unavailable,    // Indisponível
    InNegotiation   // Em negociação
}

public enum TransactionType
{
    Sale,           // Venda
    Rent,           // Arrendamento
    Both            // Venda ou Arrendamento
}

