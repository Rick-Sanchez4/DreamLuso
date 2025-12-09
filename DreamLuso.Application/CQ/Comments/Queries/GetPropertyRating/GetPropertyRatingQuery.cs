using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Comments.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Queries.GetPropertyRating;

public record GetPropertyRatingQuery(Guid PropertyId) : IRequest<Result<PropertyRatingResponse, Success, Error>>;

