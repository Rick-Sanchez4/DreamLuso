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
            Visits = visitsList.Select(v =>
            {
                // Validação defensiva para evitar NullReferenceException
                string clientName = "Cliente não encontrado";
                if (v.Client != null && v.Client.User != null && v.Client.User.Name != null)
                {
                    clientName = v.Client.User.Name.FullName;
                }

                string agentName = "Agente não encontrado";
                if (v.RealEstateAgent != null && v.RealEstateAgent.User != null && v.RealEstateAgent.User.Name != null)
                {
                    agentName = v.RealEstateAgent.User.Name.FullName;
                }

                return new PropertyVisitResponse
                {
                    Id = v.Id,
                    PropertyId = v.PropertyId,
                    PropertyTitle = v.Property?.Title ?? "Imóvel não encontrado",
                    PropertyAddress = v.Property?.Address?.FullAddress ?? "Endereço não disponível",
                    ClientId = v.ClientId,
                    ClientName = clientName,
                    AgentId = v.RealEstateAgentId,
                    AgentName = agentName,
                    VisitDate = v.VisitDate,
                    TimeSlot = v.TimeSlot.ToString(),
                    Status = v.Status.ToString(),
                    Notes = v.Notes,
                    ClientFeedback = v.ClientFeedback,
                    ClientRating = v.ClientRating,
                    ConfirmationToken = v.ConfirmationToken,
                    ConfirmedAt = v.ConfirmedAt,
                    CancelledAt = v.CancelledAt,
                    CancellationReason = v.CancellationReason,
                    CreatedAt = v.CreatedAt
                };
            }),
            TotalCount = visitsList.Count
        };

        _logger.LogInformation("Visitas do agente {AgentId}: {Count}", request.AgentId, visitsList.Count);

        return response;
    }
}

