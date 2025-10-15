using DreamLuso.Domain.Common;
using DreamLuso.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace DreamLuso.Domain.Model;

public class Notification : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public User? Sender { get; set; }
    public Guid RecipientId { get; set; }
    public User? Recipient { get; set; }
    public required string Message { get; set; }
    public NotificationStatus Status { get; set; }
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public bool IsTransient { get; set; }
    public DateTime ExpirationDate { get; set; }

    public Notification()
    {
        Id = Guid.NewGuid();
        Status = NotificationStatus.Unread;
    }

    [SetsRequiredMembers]
    public Notification(
        Guid senderId,
        Guid recipientId,
        string message,
        NotificationType type,
        NotificationPriority priority,
        Guid? referenceId = null,
        string? referenceType = null,
        bool isTransient = false)
    {
        Id = Guid.NewGuid();
        SenderId = senderId;
        RecipientId = recipientId;
        Message = message;
        Type = type;
        Priority = priority;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        IsTransient = isTransient;
        Status = NotificationStatus.Unread;
        ExpirationDate = CalculateExpirationDate(type, priority);
    }

    public void MarkAsRead()
    {
        Status = NotificationStatus.Read;
        UpdateTimestamp();
    }

    public void MarkAsArchived()
    {
        Status = NotificationStatus.Archived;
        UpdateTimestamp();
    }

    public bool IsExpired()
    {
        return ExpirationDate < DateTime.UtcNow;
    }

    private DateTime CalculateExpirationDate(NotificationType type, NotificationPriority priority)
    {
        return type switch
        {
            NotificationType.Payment => DateTime.UtcNow.AddMonths(6),
            NotificationType.ContractUpdate => DateTime.UtcNow.AddMonths(3),
            NotificationType.PropertyUpdate => DateTime.UtcNow.AddDays(30),
            NotificationType.Visit => DateTime.UtcNow.AddDays(7),
            NotificationType.Proposal => DateTime.UtcNow.AddDays(14),
            _ => DateTime.UtcNow.AddDays(7)
        };
    }
}

public enum NotificationStatus
{
    Unread,
    Read,
    Archived,
    Deleted
}

public enum NotificationType
{
    Payment,
    Contract,
    ContractUpdate,
    PropertyUpdate,
    PropertyViewing,
    NewListing,
    PriceChange,
    DocumentUpload,
    SystemAlert,
    Message,
    Favorite,
    Visit,
    Proposal,
    Negotiation,
    ProposalAccepted,
    PropertyReactivated
}

public enum NotificationPriority
{
    Low,
    Medium,
    High
}

