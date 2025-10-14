using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyVisits.Commands.CancelVisit;

public record CancelVisitCommand(
    Guid VisitId,
    string? CancellationReason
) : IRequest<Result<CancelVisitResponse, Success, Error>>;

public record CancelVisitResponse(
    Guid VisitId,
    bool Cancelled,
    DateTime CancelledAt
);

