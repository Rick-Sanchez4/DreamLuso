using FluentValidation;

namespace DreamLuso.Application.CQ.Notifications.Commands;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        // SenderId pode ser Guid.Empty para notificações do sistema
        
        RuleFor(x => x.RecipientId)
            .NotEmpty().WithMessage("O ID do destinatário é obrigatório");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("A mensagem é obrigatória")
            .MaximumLength(1000).WithMessage("A mensagem não pode exceder 1000 caracteres");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Tipo de notificação inválido");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Prioridade de notificação inválida");

        RuleFor(x => x.ReferenceId)
            .NotEqual(Guid.Empty).WithMessage("O ID de referência não pode ser vazio")
            .When(x => x.ReferenceId.HasValue);
    }
}

