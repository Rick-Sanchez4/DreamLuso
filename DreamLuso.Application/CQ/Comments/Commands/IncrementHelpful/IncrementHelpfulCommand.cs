using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Commands.IncrementHelpful;

public record IncrementHelpfulCommand(Guid CommentId) : IRequest<Result<bool, Success, Error>>;

