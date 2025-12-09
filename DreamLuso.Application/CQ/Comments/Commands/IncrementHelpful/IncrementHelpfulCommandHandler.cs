using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Commands.IncrementHelpful;

public class IncrementHelpfulCommandHandler : IRequestHandler<IncrementHelpfulCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public IncrementHelpfulCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<bool, Success, Error>> Handle(IncrementHelpfulCommand request, CancellationToken cancellationToken)
    {
        var comment = await _unitOfWork.CommentRepository.GetByIdAsync(request.CommentId);
        if (comment == null)
            return Error.NotFound;

        comment.IncrementHelpfulCount();
        await _unitOfWork.CommitAsync(cancellationToken);

        return Success.Ok;
    }
}

