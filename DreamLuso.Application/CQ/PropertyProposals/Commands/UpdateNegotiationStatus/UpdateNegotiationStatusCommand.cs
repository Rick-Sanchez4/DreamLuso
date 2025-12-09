using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.UpdateNegotiationStatus;

public record UpdateNegotiationStatusCommand(
    Guid NegotiationId,
    NegotiationStatus Status
) : IRequest<Result<bool, Success, Error>>;

