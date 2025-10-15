using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands;

public record ApproveProposalCommand(Guid ProposalId) : IRequest<Result<bool, Success, Error>>;

