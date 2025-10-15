using FluentValidation;

namespace DreamLuso.Application.CQ.Comments.Commands;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty().WithMessage("O ID do imóvel é obrigatório");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("O ID do utilizador é obrigatório");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("O comentário é obrigatório")
            .MinimumLength(3).WithMessage("O comentário deve ter pelo menos 3 caracteres")
            .MaximumLength(2000).WithMessage("O comentário não pode exceder 2000 caracteres");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("A avaliação deve ser entre 1 e 5 estrelas")
            .When(x => x.Rating.HasValue);

        RuleFor(x => x.ParentCommentId)
            .NotEqual(Guid.Empty).WithMessage("O ID do comentário pai não pode ser vazio")
            .When(x => x.ParentCommentId.HasValue);
    }
}

