using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.CancelProposal;

public record CancelProposalCommand(
    Guid ProposalId
) : IRequest<Result<bool, Success, Error>>;

