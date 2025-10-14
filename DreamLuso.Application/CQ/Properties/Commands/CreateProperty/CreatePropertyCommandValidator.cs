using FluentValidation;

namespace DreamLuso.Application.CQ.Properties.Commands.CreateProperty;

public class CreatePropertyCommandValidator : AbstractValidator<CreatePropertyCommand>
{
    public CreatePropertyCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório")
            .MaximumLength(200).WithMessage("O título não pode exceder 200 caracteres");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória")
            .MaximumLength(2000).WithMessage("A descrição não pode exceder 2000 caracteres");

        RuleFor(x => x.RealEstateAgentId)
            .NotEmpty().WithMessage("O agente imobiliário é obrigatório");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero");

        RuleFor(x => x.Size)
            .GreaterThan(0).WithMessage("A área deve ser maior que zero");

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo(0).WithMessage("O número de quartos não pode ser negativo");

        RuleFor(x => x.Bathrooms)
            .GreaterThanOrEqualTo(0).WithMessage("O número de casas de banho não pode ser negativo");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("A rua é obrigatória")
            .MaximumLength(200).WithMessage("A rua não pode exceder 200 caracteres");

        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("O número é obrigatório")
            .MaximumLength(20).WithMessage("O número não pode exceder 20 caracteres");

        RuleFor(x => x.Parish)
            .NotEmpty().WithMessage("A freguesia é obrigatória")
            .MaximumLength(100).WithMessage("A freguesia não pode exceder 100 caracteres");

        RuleFor(x => x.Municipality)
            .NotEmpty().WithMessage("O concelho é obrigatório")
            .MaximumLength(100).WithMessage("O concelho não pode exceder 100 caracteres");

        RuleFor(x => x.District)
            .NotEmpty().WithMessage("O distrito é obrigatório")
            .MaximumLength(100).WithMessage("O distrito não pode exceder 100 caracteres");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("O código postal é obrigatório")
            .Matches(@"^\d{4}-\d{3}$").WithMessage("O código postal deve ter o formato XXXX-XXX");
    }
}

