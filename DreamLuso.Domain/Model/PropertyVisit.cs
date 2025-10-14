using DreamLuso.Domain.Common;
using DreamLuso.Domain.Interfaces;

namespace DreamLuso.Domain.Model;

public class PropertyVisit : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public required Property Property { get; set; }
    public Guid ClientId { get; set; }
    public required Client Client { get; set; }
    public Guid RealEstateAgentId { get; set; }
    public required RealEstateAgent RealEstateAgent { get; set; }
    
    public DateOnly VisitDate { get; set; }
    public TimeSlot TimeSlot { get; set; }
    public VisitStatus Status { get; set; }
    
    public string? Notes { get; set; }
    public string? ClientFeedback { get; set; }
    public int? ClientRating { get; set; }           // Avaliação do cliente (1-5)
    
    // Confirmação
    public string ConfirmationToken { get; private set; } = string.Empty;
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; set; }
    
    // Lembrete
    public bool ReminderSent { get; set; } = false;
    public DateTime? ReminderSentAt { get; set; }

    public PropertyVisit()
    {
        Id = Guid.NewGuid();
        ConfirmationToken = string.Empty;
        GenerateConfirmationToken();
    }

    public PropertyVisit(Guid propertyId, Guid clientId, Guid realEstateAgentId, 
                        DateOnly visitDate, TimeSlot timeSlot)
    {
        Id = Guid.NewGuid();
        PropertyId = propertyId;
        ClientId = clientId;
        RealEstateAgentId = realEstateAgentId;
        VisitDate = visitDate;
        TimeSlot = timeSlot;
        Status = VisitStatus.Pending;
        ConfirmationToken = string.Empty;
        GenerateConfirmationToken();
    }

    private void GenerateConfirmationToken()
    {
        ConfirmationToken = $"VISIT-{Guid.NewGuid():N}";
    }

    public void Confirm()
    {
        Status = VisitStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Cancel(string? reason = null)
    {
        Status = VisitStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        UpdateTimestamp();
    }

    public void Complete(string? feedback = null, int? rating = null)
    {
        Status = VisitStatus.Completed;
        ClientFeedback = feedback;
        ClientRating = rating;
        UpdateTimestamp();
    }

    public void MarkReminderSent()
    {
        ReminderSent = true;
        ReminderSentAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Reschedule(DateOnly newDate, TimeSlot newTimeSlot)
    {
        VisitDate = newDate;
        TimeSlot = newTimeSlot;
        Status = VisitStatus.Rescheduled;
        UpdateTimestamp();
    }
}

public enum TimeSlot
{
    Morning_9AM_11AM,
    Morning_11AM_1PM,
    Afternoon_2PM_4PM,
    Afternoon_4PM_6PM,
    Evening_6PM_8PM
}

public enum VisitStatus
{
    Pending,
    Confirmed,
    Completed,
    Cancelled,
    Rescheduled,
    NoShow
}

