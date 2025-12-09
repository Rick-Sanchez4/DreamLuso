using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands.SendNotification;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyVisits.Commands.CancelVisit;

public class CancelVisitCommandHandler : IRequestHandler<CancelVisitCommand, Result<CancelVisitResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly ILogger<CancelVisitCommandHandler> _logger;

    public CancelVisitCommandHandler(
        IUnitOfWork unitOfWork,
        ISender sender,
        ILogger<CancelVisitCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _sender = sender;
        _logger = logger;
    }

    public async Task<Result<CancelVisitResponse, Success, Error>> Handle(CancelVisitCommand request, CancellationToken cancellationToken)
    {
        // Get visit
        var visitObj = await _unitOfWork.PropertyVisitRepository.GetByIdAsync(request.VisitId);
        if (visitObj == null)
        {
            _logger.LogWarning("Visita não encontrada: {VisitId}", request.VisitId);
            return Error.VisitNotFound;
        }

        var visit = (PropertyVisit)visitObj;

        // Check if already cancelled
        if (visit.Status == VisitStatus.Cancelled)
        {
            _logger.LogWarning("Visita já cancelada: {VisitId}", visit.Id);
            return Error.VisitAlreadyCancelled;
        }

        // Cancel visit
        visit.Cancel(request.CancellationReason);

        await _unitOfWork.PropertyVisitRepository.UpdateAsync(visit);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Visita cancelada: {VisitId}, Razão: {Reason}", visit.Id, request.CancellationReason);

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

            var cancellationMsg = string.IsNullOrWhiteSpace(request.CancellationReason) 
                ? "" 
                : $" Motivo: {request.CancellationReason}";

            // Notify client
            var clientNotification = $"❌ Visita cancelada. Sua visita ao imóvel '{((Property)property).Title}' agendada para {visit.VisitDate:dd/MM/yyyy} foi cancelada.{cancellationMsg}";
            await _sender.Send(new SendNotificationCommand(
                SenderId: null,
                RecipientId: client.UserId,
                Message: clientNotification,
                Type: NotificationType.Visit,
                Priority: NotificationPriority.Medium,
                ReferenceId: visit.Id,
                ReferenceType: "VisitCancelled"
            ), cancellationToken);

            // Notify agent
            var clientName = client.User?.Name?.FullName ?? "Cliente";
            var agentNotification = $"❌ Visita cancelada. A visita do cliente {clientName} ao imóvel '{((Property)property).Title}' foi cancelada.{cancellationMsg}";
            await _sender.Send(new SendNotificationCommand(
                SenderId: null,
                RecipientId: agent.UserId,
                Message: agentNotification,
                Type: NotificationType.Visit,
                Priority: NotificationPriority.Medium,
                ReferenceId: visit.Id,
                ReferenceType: "VisitCancelled"
            ), cancellationToken);
        }

        var response = new CancelVisitResponse(
            visit.Id,
            true,
            visit.CancelledAt!.Value
        );

        return response;
    }
}

