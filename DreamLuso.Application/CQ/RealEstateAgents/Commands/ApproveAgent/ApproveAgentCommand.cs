using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.RealEstateAgents.Commands.ApproveAgent;

public record ApproveAgentCommand(Guid AgentId, bool IsApproved, string? RejectionReason = null) : IRequest<Result<bool, Success, Error>>;

