using DreamLuso.Application.CQ.Accounts.Commands.RegisterUser;
using DreamLuso.Application.CQ.Accounts.Commands.LoginUser;
using DreamLuso.Application.CQ.Accounts.Commands.RefreshToken;
using DreamLuso.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
    }
}

