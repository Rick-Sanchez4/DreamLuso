using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.PropertyProposals.Common;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries.GetProposalsByAgent;

public record GetProposalsByAgentQuery(Guid AgentId) : IRequest<Result<IEnumerable<PropertyProposalResponse>, Success, Error>>;

