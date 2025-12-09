using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.PropertyVisits.Commands.ScheduleVisit;
using DreamLuso.Application.CQ.PropertyVisits.Commands.ConfirmVisit;
using DreamLuso.Application.CQ.PropertyVisits.Commands.CancelVisit;
using DreamLuso.Application.CQ.PropertyVisits.Queries.GetAvailableTimeSlots;
using DreamLuso.Application.CQ.PropertyVisits.Queries.GetVisitsByClient;
using DreamLuso.Application.CQ.PropertyVisits.Queries.GetVisitsByAgent;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DreamLuso.WebAPI.Endpoints;

public static class PropertyVisitEndpoints
{
    public static void RegisterPropertyVisitEndpoints(this IEndpointRouteBuilder routes)
    {
        var visits = routes.MapGroup("api/visits").WithTags("PropertyVisits");

        // GET /api/visits/available-slots - Obter horários disponíveis
        visits.MapGet("/available-slots", Queries.GetAvailableTimeSlots)
            .WithName("GetAvailableTimeSlots")
            .Produces<GetAvailableTimeSlotsResponse>(200)
            .Produces<Error>(400);

        // GET /api/visits/client/{clientId} - Obter visitas do cliente
        visits.MapGet("/client/{clientId:guid}", Queries.GetVisitsByClient)
            .WithName("GetVisitsByClient")
            .Produces<GetVisitsByClientResponse>(200)
            .Produces<Error>(400);

        // GET /api/visits/agent/{agentId} - Obter visitas do agente
        visits.MapGet("/agent/{agentId:guid}", Queries.GetVisitsByAgent)
            .WithName("GetVisitsByAgent")
            .Produces<GetVisitsByAgentResponse>(200)
            .Produces<Error>(400);

        // POST /api/visits - Agendar visita
        visits.MapPost("/", Commands.ScheduleVisit)
            .WithName("ScheduleVisit")
            .Produces<ScheduleVisitResponse>(201)
            .Produces<Error>(400);

        // PUT /api/visits/confirm - Confirmar visita (por token)
        visits.MapPut("/confirm", Commands.ConfirmVisitByToken)
            .WithName("ConfirmVisitByToken")
            .Produces<ConfirmVisitResponse>(200)
            .Produces<Error>(400);

        // PUT /api/visits/{id}/confirm - Confirmar visita (por ID)
        visits.MapPut("/{id:guid}/confirm", Commands.ConfirmVisitById)
            .WithName("ConfirmVisitById")
            .Produces<ConfirmVisitResponse>(200)
            .Produces<Error>(400);

        // PUT /api/visits/{id}/cancel - Cancelar visita
        visits.MapPut("/{id:guid}/cancel", Commands.CancelVisit)
            .WithName("CancelVisit")
            .Produces<CancelVisitResponse>(200)
            .Produces<Error>(400);
    }

    private static class Commands
    {
        public static async Task<Results<Created<ScheduleVisitResponse>, BadRequest<Error>>> ScheduleVisit(
            [FromServices] ISender sender,
            [FromBody] ScheduleVisitRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new ScheduleVisitCommand(
                request.PropertyId,
                request.ClientId,
                request.RealEstateAgentId,
                request.VisitDate,
                (TimeSlot)request.TimeSlot,
                request.Notes
            );

            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Created($"/api/visits/{result.Value!.VisitId}", result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<ConfirmVisitResponse>, BadRequest<Error>>> ConfirmVisitByToken(
            [FromServices] ISender sender,
            [FromBody] ConfirmVisitRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new ConfirmVisitCommand(ConfirmationToken: request.ConfirmationToken);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<ConfirmVisitResponse>, BadRequest<Error>>> ConfirmVisitById(
            [FromServices] ISender sender,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var command = new ConfirmVisitCommand(VisitId: id);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<CancelVisitResponse>, BadRequest<Error>>> CancelVisit(
            [FromServices] ISender sender,
            Guid id,
            [FromBody] CancelVisitRequest? request,
            CancellationToken cancellationToken = default)
        {
            var command = new CancelVisitCommand(id, request?.CancellationReason);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }
    }

    private static class Queries
    {
        public static async Task<Results<Ok<GetAvailableTimeSlotsResponse>, BadRequest<Error>>> GetAvailableTimeSlots(
            [FromServices] ISender sender,
            [FromQuery] Guid propertyId,
            [FromQuery] DateOnly visitDate,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAvailableTimeSlotsQuery
            {
                PropertyId = propertyId,
                VisitDate = visitDate
            };

            var result = await sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }

        public static async Task<Results<Ok<GetVisitsByClientResponse>, BadRequest<Error>>> GetVisitsByClient(
            [FromServices] ISender sender,
            Guid clientId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetVisitsByClientQuery { ClientId = clientId };
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }

        public static async Task<Results<Ok<GetVisitsByAgentResponse>, BadRequest<Error>>> GetVisitsByAgent(
            [FromServices] ISender sender,
            Guid agentId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetVisitsByAgentQuery { AgentId = agentId };
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }
    }
}

// Request DTOs
public record ScheduleVisitRequest(
    Guid PropertyId,
    Guid ClientId,
    Guid RealEstateAgentId,
    DateOnly VisitDate,
    int TimeSlot,
    string? Notes
);

public record ConfirmVisitRequest(string ConfirmationToken);

public record CancelVisitRequest(string? CancellationReason);

