using DreamLuso.Application.CQ.Accounts.Commands.RegisterUser;
using DreamLuso.Application.CQ.Accounts.Commands.LoginUser;
using DreamLuso.Application.CQ.Accounts.Commands.RefreshToken;
using DreamLuso.Application.CQ.Accounts.Commands.ChangePassword;
using DreamLuso.Application.CQ.Accounts.Commands.UpdateUserProfile;
using DreamLuso.Application.CQ.Accounts.Commands.UploadProfileImage;
using DreamLuso.Application.CQ.Accounts.Commands.ToggleUserStatus;
using DreamLuso.Application.CQ.Accounts.Queries.GetUserProfile;
using DreamLuso.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DreamLuso.WebAPI.Endpoints;

public static class AccountEndpoints
{
    public static void RegisterAccountEndpoints(this IEndpointRouteBuilder routes)
    {
        var accounts = routes.MapGroup("api/accounts").WithTags("Accounts");

        // POST /api/accounts/register - Registar novo utilizador
        accounts.MapPost("/register", Commands.RegisterUser)
                .WithName("RegisterUser")
                .Produces<RegisterUserResponse>(201)
                .Produces<Error>(400);

        // POST /api/accounts/login - Fazer login
        accounts.MapPost("/login", Commands.LoginUser)
                .WithName("LoginUser")
                .Produces<LoginUserResponse>(200)
                .Produces<Error>(400);

        // POST /api/accounts/refresh-token - Renovar tokens
        accounts.MapPost("/refresh-token", Commands.RefreshToken)
                .WithName("RefreshToken")
                .Produces<RefreshTokenResponse>(200)
                .Produces<Error>(400);

        // GET /api/accounts/profile/{userId} - Obter perfil do utilizador
        accounts.MapGet("/profile/{userId:guid}", Commands.GetUserProfile)
                .WithName("GetUserProfile")
                .Produces<UserProfileResponse>(200)
                .Produces<Error>(404)
                .RequireAuthorization();

        // PUT /api/accounts/profile - Atualizar perfil do utilizador
        accounts.MapPut("/profile", Commands.UpdateUserProfile)
                .WithName("UpdateUserProfile")
                .Produces<object>(200)
                .Produces<Error>(400)
                .RequireAuthorization();

        // POST /api/accounts/change-password - Alterar senha
        accounts.MapPost("/change-password", Commands.ChangePassword)
                .WithName("ChangePassword")
                .Produces<object>(200)
                .Produces<Error>(400)
                .RequireAuthorization();

        // PUT /api/accounts/{userId}/toggle-status - Ativar/Desativar conta (Admin only)
        accounts.MapPut("/{userId:guid}/toggle-status", Commands.ToggleUserStatus)
                .WithName("ToggleUserStatus")
                .Produces<object>(200)
                .Produces<Error>(400)
                .RequireAuthorization();
    }

    private static class Commands
    {
        public static async Task<Results<Created<RegisterUserResponse>, BadRequest<Error>>> RegisterUser(
            [FromServices] ISender sender,
            [FromBody] RegisterUserCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Created($"/api/accounts/profile/{result.Value!.UserId}", result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<LoginUserResponse>, BadRequest<Error>>> LoginUser(
            [FromServices] ISender sender,
            [FromBody] LoginUserCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<RefreshTokenResponse>, BadRequest<Error>>> RefreshToken(
            [FromServices] ISender sender,
            [FromBody] RefreshTokenCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        [Authorize]
        public static async Task<Results<Ok<UserProfileResponse>, NotFound<Error>>> GetUserProfile(
            [FromServices] ISender sender,
            [FromRoute] Guid userId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUserProfileQuery(userId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.NotFound(result.Error!);
        }

        [Authorize]
        public static async Task<Results<Ok<object>, BadRequest<Error>>> UpdateUserProfile(
            [FromServices] ISender sender,
            [FromBody] UpdateUserProfileCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Perfil atualizado com sucesso" } as object)
                : TypedResults.BadRequest(result.Error!);
        }

        [Authorize]
        public static async Task<Results<Ok<object>, BadRequest<Error>>> ChangePassword(
            [FromServices] ISender sender,
            [FromBody] ChangePasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Senha alterada com sucesso" } as object)
                : TypedResults.BadRequest(result.Error!);
        }

        [Authorize]
        public static async Task<Results<Ok<object>, BadRequest<Error>>> ToggleUserStatus(
            [FromServices] ISender sender,
            [FromRoute] Guid userId,
            [FromBody] ToggleUserStatusRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new ToggleUserStatusCommand(userId, request.IsActive);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Status do usuário atualizado com sucesso" } as object)
                : TypedResults.BadRequest(result.Error!);
        }
    }
}

public record ToggleUserStatusRequest(bool IsActive);
