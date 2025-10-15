using FluentValidation;

namespace DreamLuso.Application.CQ.Accounts.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId é obrigatório");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Primeiro nome é obrigatório")
            .MaximumLength(100).WithMessage("Primeiro nome não pode exceder 100 caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Último nome é obrigatório")
            .MaximumLength(100).WithMessage("Último nome não pode exceder 100 caracteres");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .Matches(@"^[0-9]{9,15}$").WithMessage("Telefone inválido");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Now).WithMessage("Data de nascimento inválida")
            .When(x => x.DateOfBirth.HasValue);
    }
}

