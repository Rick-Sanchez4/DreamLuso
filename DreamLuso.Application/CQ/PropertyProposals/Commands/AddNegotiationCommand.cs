using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands;

public record AddNegotiationCommand(
    Guid ProposalId,
    Guid SenderId,
    string Message,
    decimal? CounterOffer = null
) : IRequest<Result<Guid, Success, Error>>;

