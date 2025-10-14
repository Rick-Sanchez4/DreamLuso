using FluentValidation;

namespace DreamLuso.Application.CQ.Accounts.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("O primeiro nome é obrigatório")
            .MaximumLength(100).WithMessage("O primeiro nome não pode exceder 100 caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("O último nome é obrigatório")
            .MaximumLength(100).WithMessage("O último nome não pode exceder 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("Formato de email inválido")
            .MaximumLength(255).WithMessage("O email não pode exceder 255 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A password é obrigatória")
            .MinimumLength(6).WithMessage("A password deve ter pelo menos 6 caracteres")
            .MaximumLength(100).WithMessage("A password não pode exceder 100 caracteres");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("O telefone não pode exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }
}

