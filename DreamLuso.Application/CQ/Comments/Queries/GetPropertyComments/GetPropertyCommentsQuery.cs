using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Comments.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Queries.GetPropertyComments;

public record GetPropertyCommentsQuery(Guid PropertyId) : IRequest<Result<IEnumerable<CommentResponse>, Success, Error>>;

