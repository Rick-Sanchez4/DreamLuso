namespace DreamLuso.Application.CQ.PropertyVisits.Common;

public class PropertyVisitResponse
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public Guid AgentId { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public DateOnly VisitDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? ClientFeedback { get; set; }
    public int? ClientRating { get; set; }
    public string ConfirmationToken { get; set; } = string.Empty;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
}

