namespace DreamLuso.Application.CQ.Comments.Queries;

public record CommentResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    string Message,
    int? Rating,
    int HelpfulCount,
    DateTime CreatedAt,
    List<CommentResponse> Replies
);

public record PropertyRatingResponse(
    double AverageRating,
    int TotalComments,
    int FiveStars,
    int FourStars,
    int ThreeStars,
    int TwoStars,
    int OneStar
);

