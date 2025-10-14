using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyVisits.Commands.ScheduleVisit;

public class ScheduleVisitCommandHandler : IRequestHandler<ScheduleVisitCommand, Result<ScheduleVisitResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ScheduleVisitCommandHandler> _logger;

    public ScheduleVisitCommandHandler(IUnitOfWork unitOfWork, ILogger<ScheduleVisitCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ScheduleVisitResponse, Success, Error>> Handle(ScheduleVisitCommand request, CancellationToken cancellationToken)
    {
        // Verify property exists
        var property = await _unitOfWork.PropertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null)
        {
            _logger.LogWarning("Imóvel não encontrado: {PropertyId}", request.PropertyId);
            return Error.PropertyNotFound;
        }

        // Verify client exists
        var client = await _unitOfWork.ClientRepository.GetByIdAsync(request.ClientId);
        if (client == null)
        {
            _logger.LogWarning("Cliente não encontrado: {ClientId}", request.ClientId);
            return Error.ClientNotFound;
        }

        // Verify agent exists
        var agent = await _unitOfWork.RealEstateAgentRepository.GetByIdAsync(request.RealEstateAgentId);
        if (agent == null)
        {
            _logger.LogWarning("Agente não encontrado: {AgentId}", request.RealEstateAgentId);
            return Error.AgentNotFound;
        }

        // Check if time slot is available
        var isAvailable = await _unitOfWork.PropertyVisitRepository.IsTimeSlotAvailableAsync(
            request.PropertyId,
            request.VisitDate,
            request.TimeSlot);

        if (!isAvailable)
        {
            _logger.LogWarning("Horário não disponível: {PropertyId}, {Date}, {TimeSlot}", 
                request.PropertyId, request.VisitDate, request.TimeSlot);
            return Error.TimeSlotUnavailable;
        }

        // Create visit
        var visit = new PropertyVisit
        {
            PropertyId = request.PropertyId,
            Property = (Property)property,
            ClientId = request.ClientId,
            Client = (Client)client,
            RealEstateAgentId = request.RealEstateAgentId,
            RealEstateAgent = (RealEstateAgent)agent,
            VisitDate = request.VisitDate,
            TimeSlot = request.TimeSlot,
            Status = VisitStatus.Pending,
            Notes = request.Notes
        };

        // Save visit
        var savedVisit = await _unitOfWork.PropertyVisitRepository.SaveAsync(visit);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Visita agendada com sucesso: {VisitId}, Imóvel: {PropertyId}, Data: {Date}", 
            savedVisit.Id, request.PropertyId, request.VisitDate);

        var response = new ScheduleVisitResponse(
            savedVisit.Id,
            savedVisit.PropertyId,
            savedVisit.VisitDate,
            savedVisit.TimeSlot,
            savedVisit.ConfirmationToken,
            savedVisit.CreatedAt
        );

        return response;
    }
}

