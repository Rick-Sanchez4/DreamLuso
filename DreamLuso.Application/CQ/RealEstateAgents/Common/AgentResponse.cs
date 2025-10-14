namespace DreamLuso.Application.CQ.RealEstateAgents.Common;

public class AgentResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public string? LicenseNumber { get; set; }
    public DateTime? LicenseExpiry { get; set; }
    public string? OfficeEmail { get; set; }
    public string? OfficePhone { get; set; }
    public decimal? CommissionRate { get; set; }
    public int TotalSales { get; set; }
    public int TotalListings { get; set; }
    public decimal TotalRevenue { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsActive { get; set; }
    public string? Specialization { get; set; }
    public List<string> Certifications { get; set; } = [];
    public List<string> LanguagesSpoken { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

