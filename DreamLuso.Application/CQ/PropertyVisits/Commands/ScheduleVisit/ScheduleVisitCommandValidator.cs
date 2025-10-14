using FluentValidation;

namespace DreamLuso.Application.CQ.PropertyVisits.Commands.ScheduleVisit;

public class ScheduleVisitCommandValidator : AbstractValidator<ScheduleVisitCommand>
{
    public ScheduleVisitCommandValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty().WithMessage("O ID do imóvel é obrigatório");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("O ID do cliente é obrigatório");

        RuleFor(x => x.RealEstateAgentId)
            .NotEmpty().WithMessage("O ID do agente é obrigatório");

        RuleFor(x => x.VisitDate)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)))
            .WithMessage("A data da visita deve ser futura");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("As notas não podem exceder 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

