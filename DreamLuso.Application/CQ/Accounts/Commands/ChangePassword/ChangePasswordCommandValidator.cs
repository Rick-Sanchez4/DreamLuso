using FluentValidation;

namespace DreamLuso.Application.CQ.Accounts.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId é obrigatório");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Senha atual é obrigatória");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Nova senha é obrigatória")
            .MinimumLength(8).WithMessage("Nova senha deve ter no mínimo 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("Nova senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]").WithMessage("Nova senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]").WithMessage("Nova senha deve conter pelo menos um número")
            .Matches(@"[\W_]").WithMessage("Nova senha deve conter pelo menos um caractere especial");

        RuleFor(x => x.NewPassword)
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("A nova senha deve ser diferente da senha atual");
    }
}

