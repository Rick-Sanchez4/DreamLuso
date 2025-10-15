using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries;

public record GetProposalsByAgentQuery(Guid AgentId) : IRequest<Result<IEnumerable<PropertyProposalResponse>, Success, Error>>;
