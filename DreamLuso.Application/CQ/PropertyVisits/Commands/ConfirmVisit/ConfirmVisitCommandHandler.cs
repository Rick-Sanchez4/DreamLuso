using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands.SendNotification;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyVisits.Commands.ConfirmVisit;

public class ConfirmVisitCommandHandler : IRequestHandler<ConfirmVisitCommand, Result<ConfirmVisitResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly ILogger<ConfirmVisitCommandHandler> _logger;

    public ConfirmVisitCommandHandler(
        IUnitOfWork unitOfWork, 
        ISender sender,
        ILogger<ConfirmVisitCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _sender = sender;
        _logger = logger;
    }

    public async Task<Result<ConfirmVisitResponse, Success, Error>> Handle(ConfirmVisitCommand request, CancellationToken cancellationToken)
    {
        PropertyVisit? visit = null;

        // Get visit by VisitId or ConfirmationToken
        if (request.VisitId.HasValue)
        {
            var visitObj = await _unitOfWork.PropertyVisitRepository.GetByIdAsync(request.VisitId.Value);
            if (visitObj == null)
            {
                _logger.LogWarning("Visita não encontrada com ID: {VisitId}", request.VisitId.Value);
                return Error.VisitNotFound;
            }
            visit = (PropertyVisit)visitObj;
        }
        else if (!string.IsNullOrEmpty(request.ConfirmationToken))
        {
            var visitObj = await _unitOfWork.PropertyVisitRepository.GetByConfirmationTokenAsync(request.ConfirmationToken);
            if (visitObj == null)
            {
                _logger.LogWarning("Visita não encontrada com token: {Token}", request.ConfirmationToken);
                return Error.InvalidConfirmationToken;
            }
            visit = (PropertyVisit)visitObj;
        }
        else
        {
            return new Error("InvalidRequest", "É necessário fornecer VisitId ou ConfirmationToken");
        }

        if (visit == null)
        {
            return Error.VisitNotFound;
        }

        // Check if already confirmed
        if (visit.Status == VisitStatus.Confirmed)
        {
            _logger.LogWarning("Visita já confirmada: {VisitId}", visit.Id);
            return Error.VisitAlreadyConfirmed;
        }

        // Check if already cancelled
        if (visit.Status == VisitStatus.Cancelled)
        {
            _logger.LogWarning("Visita já cancelada: {VisitId}", visit.Id);
            return Error.VisitAlreadyCancelled;
        }

        // Confirm visit
        visit.Confirm();

        await _unitOfWork.PropertyVisitRepository.UpdateAsync(visit);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Visita confirmada com sucesso: {VisitId}", visit.Id);

        // Get property, client and agent for notifications (carregar com User)
        var property = await _unitOfWork.PropertyRepository.GetByIdAsync(visit.PropertyId);
        var clientObj = await _unitOfWork.ClientRepository.GetByIdWithFavoritesAsync(visit.ClientId);
        var agentDirect = await _unitOfWork.RealEstateAgentRepository.GetByIdAsync(visit.RealEstateAgentId);

        if (property != null && clientObj != null && agentDirect != null)
        {
            var client = (Client)clientObj;
            var agent = (RealEstateAgent)agentDirect;
            
            // Carregar User do agente se não estiver carregado
            if (agent.User == null && agent.UserId != Guid.Empty)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(agent.UserId);
                if (user != null)
                {
                    agent.User = (User)user;
                }
            }

            var timeSlotLabel = GetTimeSlotLabel(visit.TimeSlot);

            // Notify client
            var clientNotification = $"✅ Visita confirmada! Sua visita ao imóvel '{((Property)property).Title}' foi confirmada para {visit.VisitDate:dd/MM/yyyy} às {timeSlotLabel}.";
            await _sender.Send(new SendNotificationCommand(
                SenderId: null,
                RecipientId: client.UserId,
                Message: clientNotification,
                Type: NotificationType.Visit,
                Priority: NotificationPriority.Medium,
                ReferenceId: visit.Id,
                ReferenceType: "VisitConfirmed"
            ), cancellationToken);

            // Notify agent
            var clientName = client.User?.Name?.FullName ?? "Cliente";
            var agentNotification = $"✅ Visita confirmada! A visita do cliente {clientName} ao imóvel '{((Property)property).Title}' para {visit.VisitDate:dd/MM/yyyy} às {timeSlotLabel} foi confirmada.";
            await _sender.Send(new SendNotificationCommand(
                SenderId: null,
                RecipientId: agent.UserId,
                Message: agentNotification,
                Type: NotificationType.Visit,
                Priority: NotificationPriority.Medium,
                ReferenceId: visit.Id,
                ReferenceType: "VisitConfirmed"
            ), cancellationToken);
        }

        var response = new ConfirmVisitResponse(
            visit.Id,
            true,
            visit.ConfirmedAt!.Value
        );

        return response;
    }

    private string GetTimeSlotLabel(TimeSlot timeSlot)
    {
        return timeSlot switch
        {
            TimeSlot.Morning_9AM_11AM => "09:00 - 11:00",
            TimeSlot.Morning_11AM_1PM => "11:00 - 13:00",
            TimeSlot.Afternoon_2PM_4PM => "14:00 - 16:00",
            TimeSlot.Afternoon_4PM_6PM => "16:00 - 18:00",
            TimeSlot.Evening_6PM_8PM => "18:00 - 20:00",
            _ => "Horário não especificado"
        };
    }
}

