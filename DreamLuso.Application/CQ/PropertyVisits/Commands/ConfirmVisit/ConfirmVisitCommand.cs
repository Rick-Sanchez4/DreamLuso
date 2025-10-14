using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyVisits.Commands.ConfirmVisit;

public record ConfirmVisitCommand(
    string ConfirmationToken
) : IRequest<Result<ConfirmVisitResponse, Success, Error>>;

public record ConfirmVisitResponse(
    Guid VisitId,
    bool Confirmed,
    DateTime ConfirmedAt
);

