using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.PropertyProposals.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries.GetProposals;

public class GetProposalsQueryHandler : IRequestHandler<GetProposalsQuery, Result<GetProposalsResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProposalsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetProposalsResponse, Success, Error>> Handle(GetProposalsQuery request, CancellationToken cancellationToken)
    {
        var allProposals = await _unitOfWork.PropertyProposalRepository.GetAllAsync();
        var proposals = allProposals.Cast<PropertyProposal>().AsQueryable();

        // Apply filters
        if (request.Status.HasValue)
        {
            var statusEnum = (ProposalStatus)request.Status.Value;
            proposals = proposals.Where(p => p.Status == statusEnum);
        }

        var totalCount = proposals.Count();

        // Apply pagination
        // Get IDs for pagination
        var paginatedIds = proposals
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => p.Id)
            .ToList();

        // Load proposals with negotiations
        var paginatedProposals = new List<PropertyProposal>();
        foreach (var id in paginatedIds)
        {
            var proposal = await _unitOfWork.PropertyProposalRepository.GetByIdWithNegotiationsAsync(id);
            if (proposal != null)
                paginatedProposals.Add(proposal);
        }

        var response = new GetProposalsResponse
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Proposals = paginatedProposals.Select(p => new PropertyProposalResponse(
                Id: p.Id,
                ProposalNumber: p.ProposalNumber,
                PropertyId: p.PropertyId,
                PropertyTitle: p.Property?.Title ?? "N/A",
                ClientId: p.ClientId,
                ClientName: p.Client?.User?.Name?.FullName ?? "N/A",
                ProposedValue: p.ProposedValue,
                Type: p.Type.ToString(),
                Status: p.Status.ToString(),
                PaymentMethod: p.PaymentMethod?.ToString(),
                IntendedMoveDate: p.IntendedMoveDate,
                AdditionalNotes: p.AdditionalNotes,
                ResponseDate: p.ResponseDate,
                RejectionReason: p.RejectionReason,
                CreatedAt: p.CreatedAt,
                Negotiations: p.Negotiations.Select(n => 
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
            )).ToList()
        };

        return response;
    }
}

