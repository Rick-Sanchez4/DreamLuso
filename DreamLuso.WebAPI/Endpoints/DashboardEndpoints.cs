using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Dashboard.Queries.GetDashboardStats;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
    }
}

