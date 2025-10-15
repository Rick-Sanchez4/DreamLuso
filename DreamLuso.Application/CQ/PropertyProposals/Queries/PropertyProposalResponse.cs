using DreamLuso.Domain.Model;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries;

public record PropertyProposalResponse(
    Guid Id,
    string ProposalNumber,
    Guid PropertyId,
    string PropertyTitle,
    Guid ClientId,
    string ClientName,
    decimal ProposedValue,
    string Type,
    string Status,
    string? PaymentMethod,
    DateTime? IntendedMoveDate,
    string? AdditionalNotes,
    DateTime? ResponseDate,
    string? RejectionReason,
    DateTime CreatedAt,
    List<ProposalNegotiationResponse> Negotiations
);

public record ProposalNegotiationResponse(
    Guid Id,
    string SenderName,
    string Message,
    decimal? CounterOffer,
    string Status,
    DateTime SentAt,
    DateTime? ViewedAt,
    DateTime? RespondedAt
);

