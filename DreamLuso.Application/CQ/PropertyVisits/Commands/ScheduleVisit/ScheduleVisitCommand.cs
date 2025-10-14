using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyVisits.Commands.ScheduleVisit;

public record ScheduleVisitCommand(
    Guid PropertyId,
    Guid ClientId,
    Guid RealEstateAgentId,
    DateOnly VisitDate,
    TimeSlot TimeSlot,
    string? Notes
) : IRequest<Result<ScheduleVisitResponse, Success, Error>>;

public record ScheduleVisitResponse(
    Guid VisitId,
    Guid PropertyId,
    DateOnly VisitDate,
    TimeSlot TimeSlot,
    string ConfirmationToken,
    DateTime CreatedAt
);

