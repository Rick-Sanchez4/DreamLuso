namespace DreamLuso.Application.CQ.Contracts.Common;

public class ContractResponse
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public Guid AgentId { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Value { get; set; }
    public decimal? MonthlyRent { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public decimal? Commission { get; set; }
    public string? PaymentFrequency { get; set; }
    public bool AutoRenewal { get; set; }
    public DateTime SignatureDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

