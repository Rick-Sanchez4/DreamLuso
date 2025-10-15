using DreamLuso.Domain.Common;
using DreamLuso.Domain.Interfaces;

namespace DreamLuso.Domain.Model;

public class ProposalNegotiation : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid ProposalId { get; set; }
    public PropertyProposal? Proposal { get; set; }
    public Guid SenderId { get; set; }
    public User? Sender { get; set; }
    public required string Message { get; set; }
    public decimal? CounterOffer { get; set; }
    public NegotiationStatus Status { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ViewedAt { get; set; }
    public DateTime? RespondedAt { get; set; }

    public ProposalNegotiation()
    {
        Id = Guid.NewGuid();
        Status = NegotiationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsViewed()
    {
        Status = NegotiationStatus.Viewed;
        ViewedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Accept()
    {
        Status = NegotiationStatus.Accepted;
        RespondedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Reject()
    {
        Status = NegotiationStatus.Rejected;
        RespondedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }
}

public enum NegotiationStatus
{
    Sent,
    Viewed,
    Accepted,
    Rejected
}

