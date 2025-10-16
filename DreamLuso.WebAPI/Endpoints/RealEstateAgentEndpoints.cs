using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.RealEstateAgents.Commands.CreateAgent;
using DreamLuso.Application.CQ.RealEstateAgents.Commands.UpdateAgent;
using DreamLuso.Application.CQ.RealEstateAgents.Commands.ApproveAgent;
using DreamLuso.Application.CQ.RealEstateAgents.Queries.GetAgents;
using DreamLuso.Application.CQ.RealEstateAgents.Queries.GetAgentById;
using DreamLuso.Application.CQ.RealEstateAgents.Queries.GetAgentByUserId;
using DreamLuso.Application.CQ.RealEstateAgents.Common;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DreamLuso.WebAPI.Endpoints;

public static class RealEstateAgentEndpoints
{
    public static void RegisterRealEstateAgentEndpoints(this IEndpointRouteBuilder routes)
    {
        var agents = routes.MapGroup("api/agents").WithTags("RealEstateAgents");

        // GET /api/agents - Listar agentes com paginação
        agents.MapGet("/", Queries.GetAgents)
            .WithName("GetAgents")
            .Produces<GetAgentsResponse>(200)
            .Produces<Error>(400);

        // GET /api/agents/{id} - Obter agente por ID
        agents.MapGet("/{id:guid}", Queries.GetAgentById)
            .WithName("GetAgentById")
            .Produces<AgentResponse>(200)
            .Produces<Error>(404);

        // GET /api/agents/user/{userId} - Obter agente por ID de usuário
        agents.MapGet("/user/{userId:guid}", Queries.GetAgentByUserId)
            .WithName("GetAgentByUserId")
            .Produces<AgentResponse>(200)
            .Produces<Error>(404);

        // POST /api/agents - Criar novo agente
        agents.MapPost("/", Commands.CreateAgent)
            .WithName("CreateAgent")
            .Produces<CreateAgentResponse>(201)
            .Produces<Error>(400);

        // PUT /api/agents/{id} - Atualizar agente
        agents.MapPut("/{id:guid}", Commands.UpdateAgent)
            .WithName("UpdateAgent")
            .Produces<UpdateAgentResponse>(200)
            .Produces<Error>(400);

        // PUT /api/agents/{id}/approve - Aprovar/Rejeitar agente
        agents.MapPut("/{id:guid}/approve", Commands.ApproveAgent)
            .WithName("ApproveAgent")
            .Produces<object>(200)
            .Produces<Error>(400)
            .RequireAuthorization();
    }

    private static class Commands
    {
        public static async Task<Results<Created<CreateAgentResponse>, BadRequest<Error>>> CreateAgent(
            [FromServices] ISender sender,
            [FromBody] CreateAgentRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new CreateAgentCommand(
                request.UserId,
                request.LicenseNumber,
                request.LicenseExpiry,
                request.OfficeEmail,
                request.OfficePhone,
                request.CommissionRate,
                request.Specialization,
                request.Certifications,
                request.LanguagesSpoken?.Select(l => (Language)l).ToList()
            );

            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Created($"/api/agents/{result.Value!.AgentId}", result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<UpdateAgentResponse>, BadRequest<Error>>> UpdateAgent(
            [FromServices] ISender sender,
            Guid id,
            [FromBody] UpdateAgentRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateAgentCommand(
                id,
                request.LicenseNumber,
                request.LicenseExpiry,
                request.OfficeEmail,
                request.OfficePhone,
                request.CommissionRate,
                request.Specialization,
                request.Bio
            );

            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<object>, BadRequest<Error>>> ApproveAgent(
            [FromServices] ISender sender,
            [FromRoute] Guid id,
            [FromBody] ApproveAgentRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new ApproveAgentCommand(id, request.IsApproved, request.RejectionReason);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Agente atualizado com sucesso" } as object)
                : TypedResults.BadRequest(result.Error!);
        }
    }

    private static class Queries
    {
        public static async Task<Results<Ok<GetAgentsResponse>, BadRequest<Error>>> GetAgents(
            [FromServices] ISender sender,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? specialization = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAgentsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                IsActive = isActive,
                Specialization = specialization
            };

            var result = await sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }

        public static async Task<Results<Ok<AgentResponse>, NotFound<Error>>> GetAgentById(
            [FromServices] ISender sender,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAgentByIdQuery { Id = id };
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.NotFound(result.Error);
        }

        public static async Task<Results<Ok<AgentResponse>, NotFound<Error>>> GetAgentByUserId(
            [FromServices] ISender sender,
            [FromRoute] Guid userId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAgentByUserIdQuery(userId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.NotFound(result.Error);
        }
    }
}

public record ApproveAgentRequest(bool IsApproved, string? RejectionReason = null);

