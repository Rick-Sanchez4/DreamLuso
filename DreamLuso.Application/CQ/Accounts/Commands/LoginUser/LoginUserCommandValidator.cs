using FluentValidation;

namespace DreamLuso.Application.CQ.Accounts.Commands.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("Formato de email inválido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A password é obrigatória");
    }
}

