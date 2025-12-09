using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.AddNegotiation;

public class AddNegotiationCommandHandler : IRequestHandler<AddNegotiationCommand, Result<Guid, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddNegotiationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Guid, Success, Error>> Handle(AddNegotiationCommand request, CancellationToken cancellationToken)
    {
        // Use GetByIdWithNegotiationsAsync to ensure negotiations are loaded and tracked
        var proposal = await _unitOfWork.PropertyProposalRepository.GetByIdWithNegotiationsAsync(request.ProposalId);
        if (proposal == null)
            return Error.NotFound;

        // Validar se a proposta permite negociação
        if (proposal.Status == ProposalStatus.Approved)
            return new Error("ProposalAlreadyApproved", "Não é possível negociar uma proposta que foi aprovada.");
        
        if (proposal.Status == ProposalStatus.Rejected)
            return new Error("ProposalAlreadyRejected", "Não é possível negociar uma proposta que foi rejeitada.");
        
        if (proposal.Status == ProposalStatus.Cancelled)
            return new Error("ProposalCancelled", "Não é possível negociar uma proposta cancelada.");
        
        if (proposal.Status == ProposalStatus.Completed)
            return new Error("ProposalCompleted", "Esta proposta já foi concluída.");

        // Use the domain method to add negotiation - this ensures proper domain logic
        // The negotiation will be added to the Negotiations collection which EF tracks
        var negotiation = proposal.AddNegotiation(request.SenderId, request.Message, request.CounterOffer);
        
        // Since proposal is already being tracked from GetByIdWithNegotiationsAsync,
        // EF will automatically detect changes (Status, UpdatedAt, and new Negotiation)
        // Don't call UpdateAsync to avoid optimistic concurrency issues
        await _unitOfWork.CommitAsync(cancellationToken);

        return negotiation.Id;
    }
}

