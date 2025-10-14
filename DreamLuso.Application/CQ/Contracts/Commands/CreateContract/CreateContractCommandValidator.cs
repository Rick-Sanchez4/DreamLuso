using FluentValidation;

namespace DreamLuso.Application.CQ.Contracts.Commands.CreateContract;

public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty().WithMessage("O ID do imóvel é obrigatório");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("O ID do cliente é obrigatório");

        RuleFor(x => x.RealEstateAgentId)
            .NotEmpty().WithMessage("O ID do agente é obrigatório");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("O valor do contrato deve ser maior que zero");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("A data de início é obrigatória");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("A data de fim deve ser posterior à data de início")
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.MonthlyRent)
            .GreaterThan(0).WithMessage("A renda mensal deve ser maior que zero")
            .When(x => x.MonthlyRent.HasValue);

        RuleFor(x => x.PaymentDay)
            .InclusiveBetween(1, 31).WithMessage("O dia de pagamento deve estar entre 1 e 31")
            .When(x => x.PaymentDay.HasValue);
    }
}

