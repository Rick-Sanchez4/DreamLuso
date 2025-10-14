using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Properties.Commands.DeleteProperty;

public record DeletePropertyCommand(
    Guid Id
) : IRequest<Result<DeletePropertyResponse, Success, Error>>;

public record DeletePropertyResponse(
    Guid Id,
    bool Deleted
);

