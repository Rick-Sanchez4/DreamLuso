using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Properties.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Properties.Queries.GetPropertyById;

public class GetPropertyByIdQuery : IRequest<Result<PropertyDetailResponse, Success, Error>>
{
    public Guid Id { get; set; }
}

