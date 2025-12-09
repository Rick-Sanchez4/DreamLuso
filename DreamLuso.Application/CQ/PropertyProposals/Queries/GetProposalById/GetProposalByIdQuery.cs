using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.PropertyProposals.Common;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries.GetProposalById;

public record GetProposalByIdQuery(Guid ProposalId) : IRequest<Result<PropertyProposalResponse, Success, Error>>;

