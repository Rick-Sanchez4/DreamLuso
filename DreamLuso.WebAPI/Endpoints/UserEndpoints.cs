using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Accounts.Queries.GetAllUsers;
using DreamLuso.Application.CQ.Accounts.Commands.ToggleUserStatus;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DreamLuso.WebAPI.Endpoints;

public static class UserEndpoints
{
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var users = routes.MapGroup("api/users").WithTags("Users");

        // GET /api/users - Obter todos os usuários (Admin only)
        users.MapGet("/", Queries.GetAllUsers)
            .WithName("GetAllUsers")
            .Produces<List<UserDto>>(200)
            .Produces<Error>(400)
            .RequireAuthorization();

        // PUT /api/users/{userId}/toggle-status - Ativar/Desativar usuário
        users.MapPut("/{userId:guid}/toggle-status", Commands.ToggleUserStatus)
            .WithName("ToggleUserStatusAdmin")
            .Produces<object>(200)
            .Produces<Error>(400)
            .RequireAuthorization();
    }

    private static class Queries
    {
        [Authorize]
        public static async Task<Results<Ok<List<UserDto>>, BadRequest<Error>>> GetAllUsers(
            [FromServices] ISender sender,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllUsersQuery();
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }
    }

    private static class Commands
    {
        [Authorize]
        public static async Task<Results<Ok<object>, BadRequest<Error>>> ToggleUserStatus(
            [FromServices] ISender sender,
            [FromRoute] Guid userId,
            [FromBody] ToggleUserStatusCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(command with { UserId = userId }, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Status do usuário atualizado com sucesso" } as object)
                : TypedResults.BadRequest(result.Error!);
        }
    }
}

