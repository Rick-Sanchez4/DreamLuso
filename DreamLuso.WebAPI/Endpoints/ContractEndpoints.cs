using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Contracts.Commands.CreateContract;
using DreamLuso.Application.CQ.Contracts.Commands.ActivateContract;
using DreamLuso.Application.CQ.Contracts.Queries.GetContracts;
using DreamLuso.Application.CQ.Contracts.Queries.GetContractById;
using DreamLuso.Application.CQ.Contracts.Common;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DreamLuso.WebAPI.Endpoints;

public static class ContractEndpoints
{
    public static void RegisterContractEndpoints(this IEndpointRouteBuilder routes)
    {
        var contracts = routes.MapGroup("api/contracts").WithTags("Contracts");

        // GET /api/contracts - Listar contratos
        contracts.MapGet("/", Queries.GetContracts)
            .WithName("GetContracts")
            .Produces<GetContractsResponse>(200)
            .Produces<Error>(400);

        // GET /api/contracts/{id} - Obter contrato por ID
        contracts.MapGet("/{id:guid}", Queries.GetContractById)
            .WithName("GetContractById")
            .Produces<ContractResponse>(200)
            .Produces<Error>(404);

        // POST /api/contracts - Criar contrato
        contracts.MapPost("/", Commands.CreateContract)
            .WithName("CreateContract")
            .Produces<CreateContractResponse>(201)
            .Produces<Error>(400);

        // PUT /api/contracts/{id}/activate - Ativar contrato
        contracts.MapPut("/{id:guid}/activate", Commands.ActivateContract)
            .WithName("ActivateContract")
            .Produces<ActivateContractResponse>(200)
            .Produces<Error>(400);
    }

    private static class Commands
    {
        public static async Task<Results<Created<CreateContractResponse>, BadRequest<Error>>> CreateContract(
            [FromServices] ISender sender,
            [FromBody] CreateContractRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new CreateContractCommand(
                request.PropertyId,
                request.ClientId,
                request.RealEstateAgentId,
                (ContractType)request.Type,
                request.Value,
                request.StartDate,
                request.EndDate,
                request.MonthlyRent,
                request.SecurityDeposit,
                request.Commission,
                request.PaymentFrequency.HasValue ? (PaymentFrequency)request.PaymentFrequency.Value : null,
                request.PaymentDay,
                request.AutoRenewal,
                request.TermsAndConditions
            );

            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Created($"/api/contracts/{result.Value!.ContractId}", result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<ActivateContractResponse>, BadRequest<Error>>> ActivateContract(
            [FromServices] ISender sender,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var command = new ActivateContractCommand(id);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }
    }

    private static class Queries
    {
        public static async Task<Results<Ok<GetContractsResponse>, BadRequest<Error>>> GetContracts(
            [FromServices] ISender sender,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? clientId = null,
            [FromQuery] Guid? agentId = null,
            [FromQuery] int? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetContractsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                ClientId = clientId,
                AgentId = agentId,
                Status = status
            };

            var result = await sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }

        public static async Task<Results<Ok<ContractResponse>, NotFound<Error>>> GetContractById(
            [FromServices] ISender sender,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var query = new GetContractByIdQuery { Id = id };
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.NotFound(result.Error);
        }
    }
}

// Request DTOs
public record CreateContractRequest(
    Guid PropertyId,
    Guid ClientId,
    Guid RealEstateAgentId,
    int Type,
    decimal Value,
    DateTime StartDate,
    DateTime? EndDate,
    decimal? MonthlyRent,
    decimal? SecurityDeposit,
    decimal? Commission,
    int? PaymentFrequency,
    int? PaymentDay,
    bool AutoRenewal,
    string? TermsAndConditions
);

