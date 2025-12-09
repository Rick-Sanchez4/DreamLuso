using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.AddNegotiation;

public record AddNegotiationCommand(
    Guid ProposalId,
    Guid SenderId,
    string Message,
    decimal? CounterOffer = null
) : IRequest<Result<Guid, Success, Error>>;

