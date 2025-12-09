using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Commands.FlagComment;

public record FlagCommentCommand(Guid CommentId) : IRequest<Result<bool, Success, Error>>;

