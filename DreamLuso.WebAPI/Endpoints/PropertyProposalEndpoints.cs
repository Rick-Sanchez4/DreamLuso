using DreamLuso.Application.CQ.PropertyProposals.Commands;
using DreamLuso.Application.CQ.PropertyProposals.Queries;
using DreamLuso.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DreamLuso.WebAPI.Endpoints;

public static class PropertyProposalEndpoints
{
    public static void RegisterPropertyProposalEndpoints(this IEndpointRouteBuilder routes)
    {
        var proposals = routes.MapGroup("api/proposals").WithTags("PropertyProposals");

        proposals.MapPost("/", Commands.CreateProposal)
            .WithName("CreateProposal")
            .RequireAuthorization();

        proposals.MapGet("/{proposalId:guid}", Commands.GetProposalById)
            .WithName("GetProposalById")
            .RequireAuthorization();

        proposals.MapGet("/client/{clientId:guid}", Commands.GetProposalsByClient)
            .WithName("GetProposalsByClient")
            .RequireAuthorization();

        proposals.MapGet("/agent/{agentId:guid}", Commands.GetProposalsByAgent)
            .WithName("GetProposalsByAgent")
            .RequireAuthorization();

        proposals.MapPut("/{proposalId:guid}/approve", Commands.ApproveProposal)
            .WithName("ApproveProposal")
            .RequireAuthorization();

        proposals.MapPut("/{proposalId:guid}/reject", Commands.RejectProposal)
            .WithName("RejectProposal")
            .RequireAuthorization();

        proposals.MapPost("/{proposalId:guid}/negotiate", Commands.AddNegotiation)
            .WithName("AddNegotiation")
            .RequireAuthorization();
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
    }
}

