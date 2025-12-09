using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.PropertyProposals.Common;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries.GetProposalsByClient;

public record GetProposalsByClientQuery(Guid ClientId) : IRequest<Result<IEnumerable<PropertyProposalResponse>, Success, Error>>;

