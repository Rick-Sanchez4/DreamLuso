using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries;

public class GetProposalsByAgentQueryHandler : IRequestHandler<GetProposalsByAgentQuery, Result<IEnumerable<PropertyProposalResponse>, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProposalsByAgentQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<IEnumerable<PropertyProposalResponse>, Success, Error>> Handle(GetProposalsByAgentQuery request, CancellationToken cancellationToken)
    {
        var proposals = await _unitOfWork.PropertyProposalRepository.GetByAgentAsync(request.AgentId);

        var response = proposals.Select(p => new PropertyProposalResponse(
            p.Id,
            p.ProposalNumber,
            p.PropertyId,
            p.Property?.Title ?? "N/A",
            p.ClientId,
            p.Client?.User?.Name.FirstName + " " + p.Client?.User?.Name.LastName ?? "N/A",
            p.ProposedValue,
            p.Type.ToString(),
            p.Status.ToString(),
            p.PaymentMethod,
            p.IntendedMoveDate,
            p.AdditionalNotes,
            p.ResponseDate,
            p.RejectionReason,
            p.CreatedAt,
            p.Negotiations.Select(n => new ProposalNegotiationResponse(
                n.Id,
                n.Sender?.Name.FirstName + " " + n.Sender?.Name.LastName ?? "N/A",
                n.Message,
                n.CounterOffer,
                n.Status.ToString(),
                n.SentAt,
                n.ViewedAt,
                n.RespondedAt
            )).ToList()
        ));

        return response.ToList();
    }
}
