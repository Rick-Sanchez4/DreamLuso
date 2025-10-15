using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands;

public record RejectProposalCommand(
    Guid ProposalId,
    string RejectionReason
) : IRequest<Result<bool, Success, Error>>;

