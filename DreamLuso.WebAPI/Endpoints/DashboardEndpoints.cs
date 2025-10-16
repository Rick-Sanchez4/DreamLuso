using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Dashboard.Queries.GetDashboardStats;
using DreamLuso.Application.CQ.Dashboard.Queries.GetAdminDashboardStats;
using DreamLuso.Application.CQ.Dashboard.Queries.GetAgentDashboardStats;
using DreamLuso.Application.CQ.Dashboard.Queries.GetClientDashboardStats;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DreamLuso.WebAPI.Endpoints;

public static class DashboardEndpoints
{
    public static void RegisterDashboardEndpoints(this IEndpointRouteBuilder routes)
    {
        var dashboard = routes.MapGroup("api/dashboard").WithTags("Dashboard");

        // GET /api/dashboard/stats - Obter estatísticas gerais
        dashboard.MapGet("/stats", Queries.GetDashboardStats)
            .WithName("GetDashboardStats")
            .Produces<GetDashboardStatsResponse>(200)
            .Produces<Error>(400);

        // GET /api/dashboard/admin - Obter estatísticas do admin
        dashboard.MapGet("/admin", Queries.GetAdminDashboardStats)
            .WithName("GetAdminDashboardStats")
            .Produces<GetAdminDashboardStatsResponse>(200)
            .Produces<Error>(400)
            .RequireAuthorization();

        // GET /api/dashboard/agent/{agentId} - Obter estatísticas do agente
        dashboard.MapGet("/agent/{agentId:guid}", Queries.GetAgentDashboardStats)
            .WithName("GetAgentDashboardStats")
            .Produces<GetAgentDashboardStatsResponse>(200)
            .Produces<Error>(400)
            .RequireAuthorization();

        // GET /api/dashboard/client/{clientId} - Obter estatísticas do cliente
        dashboard.MapGet("/client/{clientId:guid}", Queries.GetClientDashboardStats)
            .WithName("GetClientDashboardStats")
            .Produces<GetClientDashboardStatsResponse>(200)
            .Produces<Error>(400)
            .RequireAuthorization();
    }

    private static class Queries
    {
        public static async Task<Results<Ok<GetDashboardStatsResponse>, BadRequest<Error>>> GetDashboardStats(
            [FromServices] ISender sender,
            CancellationToken cancellationToken = default)
        {
            var query = new GetDashboardStatsQuery();
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }

        [Authorize]
        public static async Task<Results<Ok<GetAdminDashboardStatsResponse>, BadRequest<Error>>> GetAdminDashboardStats(
            [FromServices] ISender sender,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAdminDashboardStatsQuery();
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }

        [Authorize]
        public static async Task<Results<Ok<GetAgentDashboardStatsResponse>, BadRequest<Error>>> GetAgentDashboardStats(
            [FromServices] ISender sender,
            [FromRoute] Guid agentId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAgentDashboardStatsQuery { AgentId = agentId };
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }

        [Authorize]
        public static async Task<Results<Ok<GetClientDashboardStatsResponse>, BadRequest<Error>>> GetClientDashboardStats(
            [FromServices] ISender sender,
            [FromRoute] Guid clientId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetClientDashboardStatsQuery { ClientId = clientId };
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }
    }
}

