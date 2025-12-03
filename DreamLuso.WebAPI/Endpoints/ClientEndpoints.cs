using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Clients.Commands.CreateClient;
using DreamLuso.Application.CQ.Clients.Commands.UpdateClient;
using DreamLuso.Application.CQ.Clients.Commands.AddFavorite;
using DreamLuso.Application.CQ.Clients.Commands.RemoveFavorite;
using DreamLuso.Application.CQ.Clients.Queries.GetClients;
using DreamLuso.Application.CQ.Clients.Queries.GetClientById;
using DreamLuso.Application.CQ.Clients.Queries.GetClientByUserId;
using DreamLuso.Application.CQ.Clients.Queries.GetFavorites;
using DreamLuso.Application.CQ.Clients.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DreamLuso.WebAPI.Endpoints;

public static class ClientEndpoints
{
    public static void RegisterClientEndpoints(this IEndpointRouteBuilder routes)
    {
        var clients = routes.MapGroup("api/clients").WithTags("Clients");

        // GET /api/clients - Listar clientes com paginação
        clients.MapGet("/", Queries.GetClients)
            .WithName("GetClients")
            .Produces<GetClientsResponse>(200)
            .Produces<Error>(400);

        // GET /api/clients/{id} - Obter cliente por ID
        clients.MapGet("/{id:guid}", Queries.GetClientById)
            .WithName("GetClientById")
            .Produces<ClientResponse>(200)
            .Produces<Error>(404);

        // GET /api/clients/user/{userId} - Obter cliente por ID de usuário
        clients.MapGet("/user/{userId:guid}", Queries.GetClientByUserId)
            .WithName("GetClientByUserId")
            .Produces<ClientResponse>(200)
            .Produces<Error>(404);

        // POST /api/clients - Criar novo cliente
        clients.MapPost("/", Commands.CreateClient)
            .WithName("CreateClient")
            .Produces<CreateClientResponse>(201)
            .Produces<Error>(400);

        // PUT /api/clients/{id} - Atualizar cliente
        clients.MapPut("/{id:guid}", Commands.UpdateClient)
            .WithName("UpdateClient")
            .Produces<UpdateClientResponse>(200)
            .Produces<Error>(400);

        // Favorites endpoints
        var favorites = clients.MapGroup("/{clientId:guid}/favorites").WithTags("Favorites");

        // GET /api/clients/{clientId}/favorites - Listar favoritos do cliente
        favorites.MapGet("/", Queries.GetFavorites)
            .WithName("GetFavorites")
            .Produces<GetFavoritesResponse>(200)
            .Produces<Error>(404);

        // POST /api/clients/{clientId}/favorites - Adicionar favorito
        favorites.MapPost("/", Commands.AddFavorite)
            .WithName("AddFavorite")
            .Produces<AddFavoriteResponse>(201)
            .Produces<Error>(400);

        // DELETE /api/clients/{clientId}/favorites/{propertyId} - Remover favorito
        favorites.MapDelete("/{propertyId:guid}", Commands.RemoveFavorite)
            .WithName("RemoveFavorite")
            .Produces<RemoveFavoriteResponse>(200)
            .Produces<Error>(400);
    }

    private static class Commands
    {
        public static async Task<Results<Created<CreateClientResponse>, BadRequest<Error>>> CreateClient(
            [FromServices] ISender sender,
            [FromBody] CreateClientRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new CreateClientCommand(
                request.UserId,
                request.Nif,
                request.CitizenCard,
                (Domain.Model.ClientType)request.Type,
                request.MinBudget,
                request.MaxBudget,
                request.PreferredContactMethod
            );

            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Created($"/api/clients/{result.Value!.ClientId}", result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<UpdateClientResponse>, BadRequest<Error>>> UpdateClient(
            [FromServices] ISender sender,
            Guid id,
            [FromBody] UpdateClientRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateClientCommand(
                id,
                request.Nif,
                request.MinBudget,
                request.MaxBudget,
                request.PreferredContactMethod,
                request.PropertyPreferences
            );

            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Created<AddFavoriteResponse>, BadRequest<Error>>> AddFavorite(
            [FromServices] ISender sender,
            Guid clientId,
            [FromBody] AddFavoriteRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new AddFavoriteCommand(
                clientId,
                request.PropertyId
            );

            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Created($"/api/clients/{clientId}/favorites/{request.PropertyId}", result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<RemoveFavoriteResponse>, BadRequest<Error>>> RemoveFavorite(
            [FromServices] ISender sender,
            Guid clientId,
            Guid propertyId,
            CancellationToken cancellationToken = default)
        {
            var command = new RemoveFavoriteCommand(
                clientId,
                propertyId
            );

            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }
    }

    private static class Queries
    {
        public static async Task<Results<Ok<GetClientsResponse>, BadRequest<Error>>> GetClients(
            [FromServices] ISender sender,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetClientsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                IsActive = isActive
            };

            var result = await sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }

        public static async Task<Results<Ok<ClientResponse>, NotFound<Error>>> GetClientById(
            [FromServices] ISender sender,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var query = new GetClientByIdQuery { Id = id };
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.NotFound(result.Error);
        }

        public static async Task<Results<Ok<ClientResponse>, NotFound<Error>>> GetClientByUserId(
            [FromServices] ISender sender,
            [FromRoute] Guid userId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetClientByUserIdQuery(userId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.NotFound(result.Error);
        }

        public static async Task<Results<Ok<GetFavoritesResponse>, NotFound<Error>>> GetFavorites(
            [FromServices] ISender sender,
            [FromRoute] Guid clientId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetFavoritesQuery(clientId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.NotFound(result.Error);
        }
    }
}

// Request DTOs for favorites
public record AddFavoriteRequest(Guid PropertyId);

