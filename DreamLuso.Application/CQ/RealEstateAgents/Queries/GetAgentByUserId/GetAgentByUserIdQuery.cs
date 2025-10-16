using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.RealEstateAgents.Common;
using MediatR;

namespace DreamLuso.Application.CQ.RealEstateAgents.Queries.GetAgentByUserId;

public record GetAgentByUserIdQuery(Guid UserId) : IRequest<Result<AgentResponse, Success, Error>>;

