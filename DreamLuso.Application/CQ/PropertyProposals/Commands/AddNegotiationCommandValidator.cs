using FluentValidation;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands;

public class AddNegotiationCommandValidator : AbstractValidator<AddNegotiationCommand>
{
    public AddNegotiationCommandValidator()
    {
        RuleFor(x => x.ProposalId)
            .NotEmpty().WithMessage("O ID da proposta é obrigatório");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("O ID do remetente é obrigatório");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("A mensagem é obrigatória")
            .MinimumLength(5).WithMessage("A mensagem deve ter pelo menos 5 caracteres")
            .MaximumLength(2000).WithMessage("A mensagem não pode exceder 2000 caracteres");

        RuleFor(x => x.CounterOffer)
            .GreaterThan(0).WithMessage("A contraoferta deve ser maior que zero")
            .LessThan(100000000).WithMessage("A contraoferta não pode exceder 100 milhões")
            .When(x => x.CounterOffer.HasValue);
    }
}

