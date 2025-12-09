using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands.UpdateNegotiationStatus;

public class UpdateNegotiationStatusCommandHandler : IRequestHandler<UpdateNegotiationStatusCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNegotiationStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<bool, Success, Error>> Handle(UpdateNegotiationStatusCommand request, CancellationToken cancellationToken)
    {
        // Get all proposals with negotiations loaded
        var allProposals = await _unitOfWork.PropertyProposalRepository.GetAllAsync();
        
        ProposalNegotiation? negotiation = null;
        PropertyProposal? parentProposal = null;
        
        // Search for the negotiation in all proposals
        foreach (var proposalObj in allProposals)
        {
            var proposal = await _unitOfWork.PropertyProposalRepository.GetByIdWithNegotiationsAsync(proposalObj.Id);
            if (proposal != null)
            {
                negotiation = proposal.Negotiations.FirstOrDefault(n => n.Id == request.NegotiationId);
                if (negotiation != null)
                {
                    parentProposal = proposal;
                    break;
                }
            }
        }

        if (negotiation == null || parentProposal == null)
            return Error.NotFound;

        // Update status based on the requested status
        switch (request.Status)
        {
            case NegotiationStatus.Viewed:
                negotiation.MarkAsViewed();
                break;
            case NegotiationStatus.Accepted:
                negotiation.Accept();
                break;
            case NegotiationStatus.Rejected:
                negotiation.Reject();
                break;
            case NegotiationStatus.Sent:
                // Cannot revert to Sent status
                return new Error("INVALID_STATUS", "Não é possível reverter o estado para 'Enviado'");
            default:
                return new Error("INVALID_STATUS", "Estado inválido");
        }

        // EF will automatically detect changes since the negotiation is tracked
        await _unitOfWork.CommitAsync(cancellationToken);

        return true;
    }
}

