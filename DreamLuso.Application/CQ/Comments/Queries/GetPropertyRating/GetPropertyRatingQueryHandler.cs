using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Comments.Common;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Queries.GetPropertyRating;

public class GetPropertyRatingQueryHandler : IRequestHandler<GetPropertyRatingQuery, Result<PropertyRatingResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPropertyRatingQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<PropertyRatingResponse, Success, Error>> Handle(GetPropertyRatingQuery request, CancellationToken cancellationToken)
    {
        var averageRating = await _unitOfWork.CommentRepository.GetAverageRatingAsync(request.PropertyId);
        var totalComments = await _unitOfWork.CommentRepository.GetCommentCountAsync(request.PropertyId);

        // Get all comments to calculate distribution
        var allComments = await _unitOfWork.CommentRepository.GetByPropertyAsync(request.PropertyId);
        var ratings = allComments.Where(c => c.Rating.HasValue).Select(c => c.Rating!.Value).ToList();

        var response = new PropertyRatingResponse(
            averageRating,
            totalComments,
            ratings.Count(r => r == 5),
            ratings.Count(r => r == 4),
            ratings.Count(r => r == 3),
            ratings.Count(r => r == 2),
            ratings.Count(r => r == 1)
        );

        return response;
    }
}

