using FluentValidation;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.RejectProposal;

public class RejectProposalCommandValidator : AbstractValidator<RejectProposalCommand>
{
    public RejectProposalCommandValidator()
    {
        RuleFor(x => x.ProposalId)
            .NotEmpty().WithMessage("O ID da proposta é obrigatório");

        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("O motivo da rejeição é obrigatório")
            .MinimumLength(10).WithMessage("O motivo da rejeição deve ter pelo menos 10 caracteres")
            .MaximumLength(500).WithMessage("O motivo da rejeição não pode exceder 500 caracteres");
    }
}

