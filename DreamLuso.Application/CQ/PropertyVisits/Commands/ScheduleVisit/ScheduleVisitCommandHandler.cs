using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyVisits.Commands.ScheduleVisit;

public class ScheduleVisitCommandHandler : IRequestHandler<ScheduleVisitCommand, Result<ScheduleVisitResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly ILogger<ScheduleVisitCommandHandler> _logger;

    public ScheduleVisitCommandHandler(
        IUnitOfWork unitOfWork,
        ISender sender,
        ILogger<ScheduleVisitCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _sender = sender;
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

        // Send notification to client
        var clientNotification = $"📅 Visita agendada! Sua visita ao imóvel '{((Property)property).Title}' foi agendada para {request.VisitDate:dd/MM/yyyy} às {request.TimeSlot}. " +
                                 $"O agente {((RealEstateAgent)agent).User.Name.FullName} entrará em contato.";
        
        await _sender.Send(new SendNotificationCommand(
            SenderId: Guid.Empty,
            RecipientId: ((Client)client).UserId,
            Message: clientNotification,
            Type: NotificationType.Visit,
            Priority: NotificationPriority.Medium,
            ReferenceId: savedVisit.Id,
            ReferenceType: "VisitScheduled"
        ), cancellationToken);

        // Send notification to agent
        var agentNotification = $"📅 Nova visita agendada! Cliente {((Client)client).User.Name.FullName} agendou visita ao imóvel '{((Property)property).Title}' para {request.VisitDate:dd/MM/yyyy} às {request.TimeSlot}.";
        
        await _sender.Send(new SendNotificationCommand(
            SenderId: Guid.Empty,
            RecipientId: ((RealEstateAgent)agent).UserId,
            Message: agentNotification,
            Type: NotificationType.Visit,
            Priority: NotificationPriority.Medium,
            ReferenceId: savedVisit.Id,
            ReferenceType: "VisitScheduled"
        ), cancellationToken);

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

