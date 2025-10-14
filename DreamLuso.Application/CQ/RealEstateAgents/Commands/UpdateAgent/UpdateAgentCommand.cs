using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.RealEstateAgents.Commands.UpdateAgent;

public record UpdateAgentCommand(
    Guid Id,
    string? LicenseNumber,
    DateTime? LicenseExpiry,
    string? OfficeEmail,
    string? OfficePhone,
    decimal? CommissionRate,
    string? Specialization,
    string? Bio
) : IRequest<Result<UpdateAgentResponse, Success, Error>>;

public record UpdateAgentResponse(
    Guid Id,
    DateTime UpdatedAt
);

