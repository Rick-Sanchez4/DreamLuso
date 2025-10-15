using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries;

public class GetProposalByIdQueryHandler : IRequestHandler<GetProposalByIdQuery, Result<PropertyProposalResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProposalByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<PropertyProposalResponse, Success, Error>> Handle(GetProposalByIdQuery request, CancellationToken cancellationToken)
    {
        var proposal = await _unitOfWork.PropertyProposalRepository.GetByIdWithNegotiationsAsync(request.ProposalId);
        if (proposal == null)
            return Error.NotFound;

        var response = new PropertyProposalResponse(
            proposal.Id,
            proposal.ProposalNumber,
            proposal.PropertyId,
            proposal.Property?.Title ?? "N/A",
            proposal.ClientId,
            proposal.Client?.User?.Name.FirstName + " " + proposal.Client?.User?.Name.LastName ?? "N/A",
            proposal.ProposedValue,
            proposal.Type.ToString(),
            proposal.Status.ToString(),
            proposal.PaymentMethod,
            proposal.IntendedMoveDate,
            proposal.AdditionalNotes,
            proposal.ResponseDate,
            proposal.RejectionReason,
            proposal.CreatedAt,
            proposal.Negotiations.Select(n => new ProposalNegotiationResponse(
                n.Id,
                n.Sender?.Name.FirstName + " " + n.Sender?.Name.LastName ?? "N/A",
                n.Message,
                n.CounterOffer,
                n.Status.ToString(),
                n.SentAt,
                n.ViewedAt,
                n.RespondedAt
            )).ToList()
        );

        return response;
    }
}

