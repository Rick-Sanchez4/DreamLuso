using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.PropertyProposals.Common;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries.GetProposalsByAgent;

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

        var response = proposals.Select(p => 
        {
            string clientName = "N/A";
            if (p.Client != null && p.Client.User != null && p.Client.User.Name != null)
            {
                var firstName = p.Client.User.Name.FirstName ?? "";
                var lastName = p.Client.User.Name.LastName ?? "";
                clientName = $"{firstName} {lastName}".Trim();
                if (string.IsNullOrEmpty(clientName))
                    clientName = "N/A";
            }
            
            return new PropertyProposalResponse(
                p.Id,
                p.ProposalNumber,
                p.PropertyId,
                p.Property?.Title ?? "N/A",
                p.ClientId,
                clientName,
                p.ProposedValue,
                p.Type.ToString(),
                p.Status.ToString(),
                p.PaymentMethod,
                p.IntendedMoveDate,
                p.AdditionalNotes,
                p.ResponseDate,
                p.RejectionReason,
                p.CreatedAt,
                p.Negotiations.Select(n => 
                {
                    string senderName = "N/A";
                    if (n.Sender != null && n.Sender.Name != null)
                    {
                        var firstName = n.Sender.Name.FirstName ?? "";
                        var lastName = n.Sender.Name.LastName ?? "";
                        senderName = $"{firstName} {lastName}".Trim();
                        if (string.IsNullOrEmpty(senderName))
                            senderName = "N/A";
                    }
                    
                    return new ProposalNegotiationResponse(
                        n.Id,
                        senderName,
                        n.Message,
                        n.CounterOffer,
                        n.Status.ToString(),
                        n.SentAt,
                        n.ViewedAt,
                        n.RespondedAt
                    );
                }).ToList()
            );
        });

        return response.ToList();
    }
}

