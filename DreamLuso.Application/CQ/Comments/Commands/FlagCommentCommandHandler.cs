using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Commands;

public class FlagCommentCommandHandler : IRequestHandler<FlagCommentCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public FlagCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<bool, Success, Error>> Handle(FlagCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _unitOfWork.CommentRepository.GetByIdAsync(request.CommentId);
        if (comment == null)
            return Error.NotFound;

        comment.Flag();
        await _unitOfWork.CommitAsync(cancellationToken);

        return Success.Ok;
    }
}

