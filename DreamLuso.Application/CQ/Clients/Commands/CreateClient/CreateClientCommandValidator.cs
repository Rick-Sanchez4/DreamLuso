using FluentValidation;

namespace DreamLuso.Application.CQ.Clients.Commands.CreateClient;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("O ID do utilizador é obrigatório");

        RuleFor(x => x.Nif)
            .Length(9).WithMessage("O NIF deve ter exatamente 9 caracteres")
            .Matches(@"^\d{9}$").WithMessage("O NIF deve conter apenas números")
            .When(x => !string.IsNullOrWhiteSpace(x.Nif));

        RuleFor(x => x.MinBudget)
            .GreaterThan(0).WithMessage("O orçamento mínimo deve ser maior que zero")
            .When(x => x.MinBudget.HasValue);

        RuleFor(x => x.MaxBudget)
            .GreaterThan(0).WithMessage("O orçamento máximo deve ser maior que zero")
            .GreaterThanOrEqualTo(x => x.MinBudget).WithMessage("O orçamento máximo deve ser maior ou igual ao mínimo")
            .When(x => x.MaxBudget.HasValue && x.MinBudget.HasValue);
    }
}

