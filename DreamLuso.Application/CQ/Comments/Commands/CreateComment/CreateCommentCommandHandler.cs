using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<Guid, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Guid, Success, Error>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        // Verificar se a propriedade existe
        var property = await _unitOfWork.PropertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null)
            return Error.NotFound;

        // Se for uma resposta, verificar se o comentário pai existe
        if (request.ParentCommentId.HasValue)
        {
            var parentComment = await _unitOfWork.CommentRepository.GetByIdAsync(request.ParentCommentId.Value);
            if (parentComment == null)
                return Error.NotFound;
        }

        var comment = new Comment(
            request.PropertyId,
            request.UserId,
            request.Message,
            request.Rating,
            request.ParentCommentId
        );

        await _unitOfWork.CommentRepository.SaveAsync(comment);
        await _unitOfWork.CommitAsync(cancellationToken);

        return comment.Id;
    }
}

