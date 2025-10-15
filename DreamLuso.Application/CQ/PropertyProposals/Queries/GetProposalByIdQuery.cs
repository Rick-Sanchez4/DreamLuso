using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries;

public record GetProposalByIdQuery(Guid ProposalId) : IRequest<Result<PropertyProposalResponse, Success, Error>>;

