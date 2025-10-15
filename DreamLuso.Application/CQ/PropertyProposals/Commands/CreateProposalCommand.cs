using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands;

public record CreateProposalCommand(
    Guid PropertyId,
    Guid ClientId,
    decimal ProposedValue,
    ProposalType Type,
    string? PaymentMethod = null,
    DateTime? IntendedMoveDate = null,
    string? AdditionalNotes = null
) : IRequest<Result<Guid, Success, Error>>;

