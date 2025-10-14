using DreamLuso.Domain.Common;
using DreamLuso.Domain.Interfaces;

namespace DreamLuso.Domain.Model;

public class User : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public required Name Name { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ProfileImageUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? LastLogin { get; set; }
    
    // Refresh Token properties
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    // Navigation Properties
    public Client? Client { get; set; }
    public RealEstateAgent? RealEstateAgent { get; set; }

    public User()
    {
        Id = Guid.NewGuid();
    }

    public User(string firstName, string lastName, string email, UserRole role)
    {
        Id = Guid.NewGuid();
        Name = new Name(firstName, lastName);
        Email = email.ToLower().Trim();
        Role = role;
        IsActive = true;
    }

    public void UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void SetRefreshToken(string token, DateTime expiry)
    {
        RefreshToken = token;
        RefreshTokenExpiry = expiry;
        UpdateTimestamp();
    }

    public bool IsRefreshTokenValid()
    {
        return !string.IsNullOrEmpty(RefreshToken) 
            && RefreshTokenExpiry.HasValue 
            && RefreshTokenExpiry.Value > DateTime.UtcNow;
    }
}

public enum UserRole
{
    Guest = 0,
    Client = 1,
    RealEstateAgent = 2,
    Admin = 3
}

