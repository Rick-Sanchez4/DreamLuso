using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Commands;

public record CreateCommentCommand(
    Guid PropertyId,
    Guid UserId,
    string Message,
    int? Rating = null,
    Guid? ParentCommentId = null
) : IRequest<Result<Guid, Success, Error>>;

