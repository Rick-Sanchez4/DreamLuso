using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.StartAnalysis;

public record StartAnalysisCommand(
    Guid ProposalId
) : IRequest<Result<bool, Success, Error>>;

