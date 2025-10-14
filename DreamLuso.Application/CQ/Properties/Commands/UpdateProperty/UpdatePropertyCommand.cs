using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Properties.Commands.UpdateProperty;

public record UpdatePropertyCommand(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    int Status,
    string? Amenities
) : IRequest<Result<UpdatePropertyResponse, Success, Error>>;

public record UpdatePropertyResponse(
    Guid Id,
    string Title,
    decimal Price,
    string Status,
    DateTime UpdatedAt
);

