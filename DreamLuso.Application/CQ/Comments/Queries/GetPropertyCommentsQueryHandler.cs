using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Queries;

public class GetPropertyCommentsQueryHandler : IRequestHandler<GetPropertyCommentsQuery, Result<IEnumerable<CommentResponse>, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPropertyCommentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<IEnumerable<CommentResponse>, Success, Error>> Handle(GetPropertyCommentsQuery request, CancellationToken cancellationToken)
    {
        var comments = await _unitOfWork.CommentRepository.GetByPropertyAsync(request.PropertyId);

        var response = comments.Select(c => MapCommentToResponse(c));

        return response.ToList();
    }

    private static CommentResponse MapCommentToResponse(Comment comment)
    {
        return new CommentResponse(
            comment.Id,
            comment.UserId,
            comment.User?.Name.FirstName + " " + comment.User?.Name.LastName ?? "Anónimo",
            comment.Message,
            comment.Rating,
            comment.HelpfulCount,
            comment.CreatedAt,
            comment.Replies.Select(r => MapCommentToResponse(r)).ToList()
        );
    }
}

