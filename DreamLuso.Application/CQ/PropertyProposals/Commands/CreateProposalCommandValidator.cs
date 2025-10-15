using FluentValidation;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands;

public class CreateProposalCommandValidator : AbstractValidator<CreateProposalCommand>
{
    public CreateProposalCommandValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty().WithMessage("O ID do imóvel é obrigatório");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("O ID do cliente é obrigatório");

        RuleFor(x => x.ProposedValue)
            .GreaterThan(0).WithMessage("O valor proposto deve ser maior que zero")
            .LessThan(100000000).WithMessage("O valor proposto não pode exceder 100 milhões");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Tipo de proposta inválido");

        RuleFor(x => x.PaymentMethod)
            .MaximumLength(100).WithMessage("O método de pagamento não pode exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.PaymentMethod));

        RuleFor(x => x.IntendedMoveDate)
            .GreaterThan(DateTime.UtcNow.AddDays(-1)).WithMessage("A data de mudança deve ser futura")
            .When(x => x.IntendedMoveDate.HasValue);

        RuleFor(x => x.AdditionalNotes)
            .MaximumLength(2000).WithMessage("As notas adicionais não podem exceder 2000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.AdditionalNotes));
    }
}

