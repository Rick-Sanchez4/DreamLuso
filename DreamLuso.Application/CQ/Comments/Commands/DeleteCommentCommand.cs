using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Commands;

public record DeleteCommentCommand(Guid CommentId) : IRequest<Result<bool, Success, Error>>;

