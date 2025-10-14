using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.PropertyVisits.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyVisits.Queries.GetVisitsByAgent;

public class GetVisitsByAgentQueryHandler : IRequestHandler<GetVisitsByAgentQuery, Result<GetVisitsByAgentResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetVisitsByAgentQueryHandler> _logger;

    public GetVisitsByAgentQueryHandler(IUnitOfWork unitOfWork, ILogger<GetVisitsByAgentQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetVisitsByAgentResponse, Success, Error>> Handle(GetVisitsByAgentQuery request, CancellationToken cancellationToken)
    {
        var visits = await _unitOfWork.PropertyVisitRepository.GetByAgentIdAsync(request.AgentId);
        var visitsList = visits.Cast<PropertyVisit>().ToList();

        var response = new GetVisitsByAgentResponse
        {
            Visits = visitsList.Select(v => new PropertyVisitResponse
            {
                Id = v.Id,
                PropertyId = v.PropertyId,
                PropertyTitle = v.Property.Title,
                PropertyAddress = v.Property.Address.FullAddress,
                ClientId = v.ClientId,
                ClientName = v.Client.User.Name.FullName,
                AgentId = v.RealEstateAgentId,
                AgentName = v.RealEstateAgent.User.Name.FullName,
                VisitDate = v.VisitDate,
                TimeSlot = v.TimeSlot.ToString(),
                Status = v.Status.ToString(),
                Notes = v.Notes,
                ClientFeedback = v.ClientFeedback,
                ClientRating = v.ClientRating,
                ConfirmationToken = v.ConfirmationToken,
                ConfirmedAt = v.ConfirmedAt,
                CreatedAt = v.CreatedAt
            }),
            TotalCount = visitsList.Count
        };

        _logger.LogInformation("Visitas do agente {AgentId}: {Count}", request.AgentId, visitsList.Count);

        return response;
    }
}

