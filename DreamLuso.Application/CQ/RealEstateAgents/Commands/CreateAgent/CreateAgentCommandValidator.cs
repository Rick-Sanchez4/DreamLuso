using FluentValidation;

namespace DreamLuso.Application.CQ.RealEstateAgents.Commands.CreateAgent;

public class CreateAgentCommandValidator : AbstractValidator<CreateAgentCommand>
{
    public CreateAgentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("O ID do utilizador é obrigatório");

        RuleFor(x => x.LicenseNumber)
            .MaximumLength(50).WithMessage("O número de licença não pode exceder 50 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.LicenseNumber));

        RuleFor(x => x.OfficeEmail)
            .EmailAddress().WithMessage("Email do escritório inválido")
            .MaximumLength(255).WithMessage("O email não pode exceder 255 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.OfficeEmail));

        RuleFor(x => x.OfficePhone)
            .MaximumLength(20).WithMessage("O telefone não pode exceder 20 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.OfficePhone));

        RuleFor(x => x.CommissionRate)
            .InclusiveBetween(0, 100).WithMessage("A taxa de comissão deve estar entre 0 e 100%")
            .When(x => x.CommissionRate.HasValue);
    }
}

