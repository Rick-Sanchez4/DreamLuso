using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries;

public record GetProposalsByClientQuery(Guid ClientId) : IRequest<Result<IEnumerable<PropertyProposalResponse>, Success, Error>>;

