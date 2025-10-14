namespace DreamLuso.Application.CQ.Clients.Common;

public class ClientResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public string? Nif { get; set; }
    public string? CitizenCard { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal? MinBudget { get; set; }
    public decimal? MaxBudget { get; set; }
    public string? PreferredContactMethod { get; set; }
    public DateTime CreatedAt { get; set; }
}

