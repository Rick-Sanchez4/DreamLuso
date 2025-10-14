using DreamLuso.Domain.Common;
using DreamLuso.Domain.Interfaces;

namespace DreamLuso.Domain.Model;

public class RealEstateAgent : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required User User { get; set; }
    public string? LicenseNumber { get; set; }      // Número da licença profissional (AMI)
    public DateTime? LicenseExpiry { get; set; }
    public string? OfficeEmail { get; set; }
    public string? OfficePhone { get; set; }
    public decimal? CommissionRate { get; set; }     // Taxa de comissão padrão (%)
    public int TotalSales { get; set; } = 0;
    public int TotalListings { get; set; } = 0;
    public decimal TotalRevenue { get; set; } = 0;
    public double Rating { get; set; } = 0;          // Avaliação média
    public int ReviewCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public string? Specialization { get; set; }      // Especialização (Residencial, Comercial, etc.)
    public string? Bio { get; set; }                 // Biografia profissional
    
    // Certificações e idiomas
    public List<string> Certifications { get; set; }
    public List<Language> LanguagesSpoken { get; set; }
    
    // Navigation Properties
    public List<Property> Properties { get; set; }
    public List<PropertyVisit> PropertyVisits { get; set; }
    public List<Contract> Contracts { get; set; }

    public RealEstateAgent()
    {
        Id = Guid.NewGuid();
        Certifications = [];
        LanguagesSpoken = [];
        Properties = [];
        PropertyVisits = [];
        Contracts = [];
    }

    public RealEstateAgent(Guid userId, User user, string? licenseNumber = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        User = user;
        LicenseNumber = licenseNumber;
        IsActive = true;
        Certifications = [];
        LanguagesSpoken = [];
        Properties = [];
        PropertyVisits = [];
        Contracts = [];
    }

    public void UpdateStats(int sales, int listings, decimal revenue)
    {
        TotalSales = sales;
        TotalListings = listings;
        TotalRevenue = revenue;
        UpdateTimestamp();
    }

    public void UpdateRating(double newRating, int reviewCount)
    {
        Rating = newRating;
        ReviewCount = reviewCount;
        UpdateTimestamp();
    }

    public void AddCertification(string certification)
    {
        if (!Certifications.Contains(certification))
        {
            Certifications.Add(certification);
            UpdateTimestamp();
        }
    }

    public bool HasActiveLicense()
    {
        return !string.IsNullOrEmpty(LicenseNumber) 
            && (!LicenseExpiry.HasValue || LicenseExpiry.Value > DateTime.UtcNow);
    }
}

public enum Language
{
    Portuguese,
    English,
    Spanish,
    French,
    German,
    Italian,
    Chinese,
    Arabic,
    Russian
}

