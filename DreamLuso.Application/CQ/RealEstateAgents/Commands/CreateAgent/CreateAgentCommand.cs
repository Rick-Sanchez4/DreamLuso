using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.RealEstateAgents.Commands.CreateAgent;

public record CreateAgentCommand(
    Guid UserId,
    string? LicenseNumber,
    DateTime? LicenseExpiry,
    string? OfficeEmail,
    string? OfficePhone,
    decimal? CommissionRate,
    string? Specialization,
    List<string>? Certifications,
    List<Language>? LanguagesSpoken
) : IRequest<Result<CreateAgentResponse, Success, Error>>;

public record CreateAgentResponse(
    Guid AgentId,
    Guid UserId,
    string FullName,
    string? LicenseNumber,
    DateTime CreatedAt
);

