using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Queries;

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
        var paginatedProposals = proposals
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

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
                Type: ((int)p.Type).ToString(),
                Status: p.Status.ToString(),
                PaymentMethod: p.PaymentMethod?.ToString(),
                IntendedMoveDate: p.IntendedMoveDate,
                AdditionalNotes: p.AdditionalNotes,
                ResponseDate: p.ResponseDate,
                RejectionReason: p.RejectionReason,
                CreatedAt: p.CreatedAt,
                Negotiations: new List<ProposalNegotiationResponse>()
            )).ToList()
        };

        return response;
    }
}

