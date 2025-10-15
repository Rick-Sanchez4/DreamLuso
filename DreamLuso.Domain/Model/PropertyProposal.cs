using DreamLuso.Domain.Common;
using DreamLuso.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace DreamLuso.Domain.Model;

public class PropertyProposal : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public required string ProposalNumber { get; set; }
    public Guid PropertyId { get; set; }
    public Property? Property { get; set; }
    public Guid ClientId { get; set; }
    public Client? Client { get; set; }
    public decimal ProposedValue { get; set; }
    public ProposalType Type { get; set; }
    public ProposalStatus Status { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime? IntendedMoveDate { get; set; }
    public string? AdditionalNotes { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string? RejectionReason { get; set; }

    // Navigation Properties
    public List<ProposalNegotiation> Negotiations { get; set; }

    public PropertyProposal()
    {
        Id = Guid.NewGuid();
        ProposalNumber = GenerateProposalNumber();
        Negotiations = [];
        Status = ProposalStatus.Pending;
    }

    [SetsRequiredMembers]
    public PropertyProposal(
        Guid propertyId,
        Guid clientId,
        decimal proposedValue,
        ProposalType type,
        string? paymentMethod = null,
        DateTime? intendedMoveDate = null)
    {
        Id = Guid.NewGuid();
        PropertyId = propertyId;
        ClientId = clientId;
        ProposalNumber = GenerateProposalNumber();
        ProposedValue = proposedValue;
        Type = type;
        PaymentMethod = paymentMethod;
        IntendedMoveDate = intendedMoveDate;
        Status = ProposalStatus.Pending;
        Negotiations = [];
    }

    private static string GenerateProposalNumber()
    {
        var year = DateTime.UtcNow.Year;
        var randomNumber = Random.Shared.Next(100, 999);
        var randomSuffix = Guid.NewGuid().ToString()[..3].ToUpper();
        return $"PROP-{year}-{randomNumber}-{randomSuffix}";
    }

    public void Submit()
    {
        Status = ProposalStatus.Pending;
        UpdateTimestamp();
    }

    public void StartAnalysis()
    {
        Status = ProposalStatus.UnderAnalysis;
        UpdateTimestamp();
    }

    public void Approve()
    {
        Status = ProposalStatus.Approved;
        ResponseDate = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Reject(string reason)
    {
        Status = ProposalStatus.Rejected;
        RejectionReason = reason;
        ResponseDate = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void StartNegotiation()
    {
        Status = ProposalStatus.InNegotiation;
        UpdateTimestamp();
    }

    public ProposalNegotiation AddNegotiation(Guid senderId, string message, decimal? counterOffer = null)
    {
        var negotiation = new ProposalNegotiation
        {
            ProposalId = Id,
            SenderId = senderId,
            Message = message,
            CounterOffer = counterOffer,
            Status = NegotiationStatus.Sent,
            SentAt = DateTime.UtcNow
        };

        Negotiations.Add(negotiation);
        Status = ProposalStatus.InNegotiation;
        UpdateTimestamp();

        return negotiation;
    }

    public void Cancel()
    {
        Status = ProposalStatus.Cancelled;
        UpdateTimestamp();
    }
}

public enum ProposalType
{
    Purchase,
    Rent
}

public enum ProposalStatus
{
    Pending,
    UnderAnalysis,
    InNegotiation,
    Approved,
    Rejected,
    Cancelled,
    Completed
}

