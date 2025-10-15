using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Queries;

public record GetPropertyRatingQuery(Guid PropertyId) : IRequest<Result<PropertyRatingResponse, Success, Error>>;

