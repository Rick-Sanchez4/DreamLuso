using DreamLuso.Application.CQ.PropertyProposals.Commands.ApproveProposal;
using DreamLuso.Application.CQ.PropertyProposals.Commands.CreateProposal;
using DreamLuso.Application.CQ.PropertyProposals.Commands.RejectProposal;
using DreamLuso.Application.CQ.PropertyProposals.Commands.AddNegotiation;
using DreamLuso.Application.CQ.PropertyProposals.Commands.UpdateNegotiationStatus;
using DreamLuso.Application.CQ.PropertyProposals.Commands.CancelProposal;
using DreamLuso.Application.CQ.PropertyProposals.Commands.StartAnalysis;
using DreamLuso.Application.CQ.PropertyProposals.Queries.GetProposals;
using DreamLuso.Application.CQ.PropertyProposals.Queries.GetProposalById;
using DreamLuso.Application.CQ.PropertyProposals.Queries.GetProposalsByClient;
using DreamLuso.Application.CQ.PropertyProposals.Queries.GetProposalsByAgent;
using DreamLuso.Application.CQ.PropertyProposals.Common;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DreamLuso.WebAPI.Endpoints;

public record UpdateNegotiationStatusRequest(NegotiationStatus Status);

public static class PropertyProposalEndpoints
{
    public static void RegisterPropertyProposalEndpoints(this IEndpointRouteBuilder routes)
    {
        var proposals = routes.MapGroup("api/proposals").WithTags("PropertyProposals");

        // GET /api/proposals - Listar todas as propostas (Admin)
        proposals.MapGet("/", Queries.GetProposals)
            .WithName("GetProposals")
            .RequireAuthorization();

        proposals.MapPost("/", Commands.CreateProposal)
            .WithName("CreateProposal")
            .RequireAuthorization();

        // Specific routes must come before generic {proposalId:guid} route
        proposals.MapGet("/client/{clientId:guid}", Commands.GetProposalsByClient)
            .WithName("GetProposalsByClient")
            .RequireAuthorization();

        proposals.MapGet("/agent/{agentId:guid}", Commands.GetProposalsByAgent)
            .WithName("GetProposalsByAgent")
            .RequireAuthorization();

        proposals.MapPut("/negotiations/{negotiationId:guid}/status", Commands.UpdateNegotiationStatus)
            .WithName("UpdateNegotiationStatus")
            .RequireAuthorization();

        // Action routes for specific proposal
        proposals.MapPut("/{proposalId:guid}/approve", Commands.ApproveProposal)
            .WithName("ApproveProposal")
            .RequireAuthorization();

        proposals.MapPut("/{proposalId:guid}/reject", Commands.RejectProposal)
            .WithName("RejectProposal")
            .RequireAuthorization();

        proposals.MapPut("/{proposalId:guid}/cancel", Commands.CancelProposal)
            .WithName("CancelProposal")
            .RequireAuthorization();

        proposals.MapPut("/{proposalId:guid}/start-analysis", Commands.StartAnalysis)
            .WithName("StartAnalysis")
            .RequireAuthorization();

        proposals.MapPost("/{proposalId:guid}/negotiate", Commands.AddNegotiation)
            .WithName("AddNegotiation")
            .RequireAuthorization();

        // Generic route must come last
        proposals.MapGet("/{proposalId:guid}", Commands.GetProposalById)
            .WithName("GetProposalById")
            .RequireAuthorization();
    }

    private static class Queries
    {
        public static async Task<Results<Ok<GetProposalsResponse>, BadRequest<Error>>> GetProposals(
            [FromServices] ISender sender,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetProposalsQuery(pageNumber, pageSize, status);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }
    }

    private static class Commands
    {
        public static async Task<Results<Ok<Guid>, BadRequest<Error>>> CreateProposal(
            [FromServices] ISender sender,
            [FromBody] CreateProposalCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<PropertyProposalResponse>, NotFound<Error>>> GetProposalById(
            [FromServices] ISender sender,
            [FromRoute] Guid proposalId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetProposalByIdQuery(proposalId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.NotFound(result.Error!);
        }

        public static async Task<Results<Ok<IEnumerable<PropertyProposalResponse>>, NotFound<Error>>> GetProposalsByClient(
            [FromServices] ISender sender,
            [FromRoute] Guid clientId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetProposalsByClientQuery(clientId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.NotFound(result.Error!);
        }

        public static async Task<Results<Ok<IEnumerable<PropertyProposalResponse>>, NotFound<Error>>> GetProposalsByAgent(
            [FromServices] ISender sender,
            [FromRoute] Guid agentId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetProposalsByAgentQuery(agentId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.NotFound(result.Error!);
        }

        public static async Task<Results<Ok<object>, BadRequest<Error>>> ApproveProposal(
            [FromServices] ISender sender,
            [FromRoute] Guid proposalId,
            CancellationToken cancellationToken = default)
        {
            var command = new ApproveProposalCommand(proposalId);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Proposta aprovada com sucesso" } as object)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<object>, BadRequest<Error>>> RejectProposal(
            [FromServices] ISender sender,
            [FromBody] RejectProposalCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Proposta rejeitada" } as object)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<Guid>, BadRequest<Error>>> AddNegotiation(
            [FromServices] ISender sender,
            [FromBody] AddNegotiationCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<object>, BadRequest<Error>>> UpdateNegotiationStatus(
            [FromServices] ISender sender,
            [FromRoute] Guid negotiationId,
            [FromBody] UpdateNegotiationStatusRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateNegotiationStatusCommand(negotiationId, request.Status);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Estado da negociação atualizado" } as object)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<object>, BadRequest<Error>>> CancelProposal(
            [FromServices] ISender sender,
            [FromRoute] Guid proposalId,
            CancellationToken cancellationToken = default)
        {
            var command = new CancelProposalCommand(proposalId);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Proposta cancelada com sucesso" } as object)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<object>, BadRequest<Error>>> StartAnalysis(
            [FromServices] ISender sender,
            [FromRoute] Guid proposalId,
            CancellationToken cancellationToken = default)
        {
            var command = new StartAnalysisCommand(proposalId);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Análise iniciada" } as object)
                : TypedResults.BadRequest(result.Error!);
        }
    }
}

