using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Clients.Commands.CreateClient;
using DreamLuso.Application.CQ.Clients.Commands.UpdateClient;
using DreamLuso.Application.CQ.Clients.Queries.GetClients;
using DreamLuso.Application.CQ.Clients.Queries.GetClientById;
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
    }
}

