using DreamLuso.Domain.Common;
using DreamLuso.Domain.Interfaces;

namespace DreamLuso.Domain.Model;

public class Client : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required User User { get; set; }
    public string? Nif { get; set; }
    public string? CitizenCard { get; set; }
    public ClientType Type { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    public string? PreferredContactMethod { get; set; } // Email, Phone, WhatsApp
    
    // Preferências de imóveis
    public string? PropertyPreferences { get; set; } // JSON ou string com preferências
    public decimal? MinBudget { get; set; }
    public decimal? MaxBudget { get; set; }
    
    // Navigation Properties
    public List<PropertyVisit> PropertyVisits { get; set; }
    public List<Contract> Contracts { get; set; }
    public List<Property> FavoriteProperties { get; set; }

    public Client()
    {
        Id = Guid.NewGuid();
        PropertyVisits = [];
        Contracts = [];
        FavoriteProperties = [];
    }

    public Client(Guid userId, User user, ClientType type)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        User = user;
        Type = type;
        IsActive = true;
        PropertyVisits = [];
        Contracts = [];
        FavoriteProperties = [];
    }

    public void AddNotes(string notes)
    {
        if (string.IsNullOrWhiteSpace(Notes))
            Notes = notes;
        else
            Notes += Environment.NewLine + $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm}] {notes}";
        
        UpdateTimestamp();
    }

    public void UpdateBudgetRange(decimal? minBudget, decimal? maxBudget)
    {
        MinBudget = minBudget;
        MaxBudget = maxBudget;
        UpdateTimestamp();
    }

    public bool IsCompany() => Type == ClientType.Company;
    public bool IsIndividual() => Type == ClientType.Individual;
}

public enum ClientType
{
    Individual,  // Particular
    Company      // Empresa
}

