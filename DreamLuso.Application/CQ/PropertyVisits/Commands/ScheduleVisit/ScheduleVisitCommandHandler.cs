using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Commands.SendNotification;
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

        // Verify client exists (carregar com User para notificações)
        var clientObj = await _unitOfWork.ClientRepository.GetByIdWithFavoritesAsync(request.ClientId);
        if (clientObj == null)
        {
            _logger.LogWarning("Cliente não encontrado: {ClientId}", request.ClientId);
            return Error.ClientNotFound;
        }
        var client = (Client)clientObj;

        // Verificar se o User do cliente está carregado
        if (client.User == null)
        {
            _logger.LogWarning("User do cliente não encontrado para ClientId: {ClientId}, UserId: {UserId}", 
                request.ClientId, client.UserId);
            // Tentar carregar o User separadamente
            if (client.UserId != Guid.Empty)
            {
                var clientUser = await _unitOfWork.UserRepository.GetByIdAsync(client.UserId);
                if (clientUser != null)
                {
                    client.User = (User)clientUser;
                    _logger.LogInformation("User do cliente carregado separadamente: {UserId}", client.UserId);
                }
                else
                {
                    _logger.LogError("Não foi possível carregar User do cliente: {UserId}", client.UserId);
                }
            }
        }

        // Verify agent exists e carregar com User para notificações
        // Usar GetByUserIdAsync se tivermos o UserId, ou carregar com Include
        RealEstateAgent? agent = null;
        
        // Primeiro, tentar obter o agente pelo ID e carregar User
        var agentDirect = await _unitOfWork.RealEstateAgentRepository.GetByIdAsync(request.RealEstateAgentId);
        if (agentDirect == null)
        {
            _logger.LogWarning("Agente não encontrado: {AgentId}", request.RealEstateAgentId);
            return Error.AgentNotFound;
        }
        agent = (RealEstateAgent)agentDirect;
        
        // Se o User não foi carregado, tentar carregar separadamente
        if (agent.User == null && agent.UserId != Guid.Empty)
        {
            _logger.LogWarning("User do agente não carregado automaticamente. Tentando carregar separadamente. AgentId: {AgentId}, UserId: {UserId}", 
                request.RealEstateAgentId, agent.UserId);
            
            // Tentar usar GetByUserIdAsync que carrega o User
            var agentWithUser = await _unitOfWork.RealEstateAgentRepository.GetByUserIdAsync(agent.UserId);
            if (agentWithUser != null && agentWithUser.User != null)
            {
                agent = (RealEstateAgent)agentWithUser;
                _logger.LogInformation("User do agente carregado via GetByUserIdAsync: {UserId}", agent.UserId);
            }
            else
            {
                // Última tentativa: carregar User diretamente
                var agentUser = await _unitOfWork.UserRepository.GetByIdAsync(agent.UserId);
                if (agentUser != null)
                {
                    agent.User = (User)agentUser;
                    _logger.LogInformation("User do agente carregado diretamente: {UserId}", agent.UserId);
                }
                else
                {
                    _logger.LogError("Não foi possível carregar User do agente: {UserId}", agent.UserId);
                }
            }
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
        if (client.User != null && client.UserId != Guid.Empty)
        {
            var agentName = agent.User?.Name?.FullName ?? "o agente";
            var clientNotification = $"📅 Visita agendada! Sua visita ao imóvel '{((Property)property).Title}' foi agendada para {request.VisitDate:dd/MM/yyyy} às {GetTimeSlotLabel(request.TimeSlot)}. " +
                                     $"O agente {agentName} entrará em contato.";
            
            _logger.LogInformation("Enviando notificação para cliente: {ClientUserId}, Mensagem: {Message}", 
                client.UserId, clientNotification);
            
            var clientNotificationResult = await _sender.Send(new SendNotificationCommand(
                SenderId: null,
                RecipientId: client.UserId,
                Message: clientNotification,
                Type: NotificationType.Visit,
                Priority: NotificationPriority.Medium,
                ReferenceId: savedVisit.Id,
                ReferenceType: "VisitScheduled"
            ), cancellationToken);

            if (clientNotificationResult.IsSuccess)
            {
                _logger.LogInformation("Notificação enviada com sucesso para cliente: {ClientUserId}, NotificationId: {NotificationId}", 
                    client.UserId, clientNotificationResult.Value);
            }
            else
            {
                _logger.LogError("Erro ao enviar notificação para cliente: {ClientUserId}, Erro: {Error}", 
                    client.UserId, clientNotificationResult.Error?.Description);
            }
        }
        else
        {
            _logger.LogWarning("Não foi possível enviar notificação para cliente: User é null ou UserId é vazio. ClientId: {ClientId}, UserId: {UserId}", 
                request.ClientId, client.UserId);
        }

        // Send notification to agent
        if (agent.User != null && agent.UserId != Guid.Empty)
        {
            var clientName = client.User?.Name?.FullName ?? "Cliente";
            var agentNotification = $"📅 Nova visita agendada! Cliente {clientName} agendou visita ao imóvel '{((Property)property).Title}' para {request.VisitDate:dd/MM/yyyy} às {GetTimeSlotLabel(request.TimeSlot)}.";
            
            _logger.LogInformation("Enviando notificação para agente: {AgentUserId}, Mensagem: {Message}", 
                agent.UserId, agentNotification);
            
            var agentNotificationResult = await _sender.Send(new SendNotificationCommand(
                SenderId: null,
                RecipientId: agent.UserId,
                Message: agentNotification,
                Type: NotificationType.Visit,
                Priority: NotificationPriority.Medium,
                ReferenceId: savedVisit.Id,
                ReferenceType: "VisitScheduled"
            ), cancellationToken);

            if (agentNotificationResult.IsSuccess)
            {
                _logger.LogInformation("Notificação enviada com sucesso para agente: {AgentUserId}, NotificationId: {NotificationId}", 
                    agent.UserId, agentNotificationResult.Value);
            }
            else
            {
                _logger.LogError("Erro ao enviar notificação para agente: {AgentUserId}, Erro: {Error}", 
                    agent.UserId, agentNotificationResult.Error?.Description);
            }
        }
        else
        {
            _logger.LogWarning("Não foi possível enviar notificação para agente: User é null ou UserId é vazio. AgentId: {AgentId}, UserId: {UserId}", 
                request.RealEstateAgentId, agent.UserId);
        }

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

