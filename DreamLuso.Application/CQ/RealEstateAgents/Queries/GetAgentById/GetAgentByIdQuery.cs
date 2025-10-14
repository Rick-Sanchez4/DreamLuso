using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.RealEstateAgents.Common;
using MediatR;

namespace DreamLuso.Application.CQ.RealEstateAgents.Queries.GetAgentById;

public class GetAgentByIdQuery : IRequest<Result<AgentResponse, Success, Error>>
{
    public Guid Id { get; set; }
}

