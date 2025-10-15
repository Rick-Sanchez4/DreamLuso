using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Queries;

public record GetPropertyCommentsQuery(Guid PropertyId) : IRequest<Result<IEnumerable<CommentResponse>, Success, Error>>;

