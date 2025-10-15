using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Commands;

public record FlagCommentCommand(Guid CommentId) : IRequest<Result<bool, Success, Error>>;

