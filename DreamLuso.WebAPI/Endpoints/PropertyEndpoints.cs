using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Properties.Commands.CreateProperty;
using DreamLuso.Application.CQ.Properties.Commands.UpdateProperty;
using DreamLuso.Application.CQ.Properties.Commands.DeleteProperty;
using DreamLuso.Application.CQ.Properties.Queries.GetProperties;
using DreamLuso.Application.CQ.Properties.Queries.GetPropertyById;
using DreamLuso.Application.CQ.Properties.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DreamLuso.WebAPI.Endpoints;

public static class PropertyEndpoints
{
    public static void RegisterPropertyEndpoints(this IEndpointRouteBuilder routes)
    {
        var properties = routes.MapGroup("api/properties").WithTags("Properties");

        // GET /api/properties - Listar imóveis com paginação e filtros
        properties.MapGet("/", Queries.GetProperties)
            .WithName("GetProperties")
            .Produces<GetPropertiesResponse>(200)
            .Produces<Error>(400);

        // GET /api/properties/{id} - Obter imóvel por ID
        properties.MapGet("/{id:guid}", Queries.GetPropertyById)
            .WithName("GetPropertyById")
            .Produces<PropertyDetailResponse>(200)
            .Produces<Error>(404);

        // POST /api/properties - Criar novo imóvel
        properties.MapPost("/", Commands.CreateProperty)
            .WithName("CreateProperty")
            .Produces<CreatePropertyResponse>(201)
            .Produces<Error>(400);

        // PUT /api/properties/{id} - Atualizar imóvel
        properties.MapPut("/{id:guid}", Commands.UpdateProperty)
            .WithName("UpdateProperty")
            .Produces<UpdatePropertyResponse>(200)
            .Produces<Error>(400);

        // DELETE /api/properties/{id} - Desativar imóvel (soft delete)
        properties.MapDelete("/{id:guid}", Commands.DeleteProperty)
            .WithName("DeleteProperty")
            .Produces<DeletePropertyResponse>(200)
            .Produces<Error>(404);
    }

    private static class Commands
    {
        public static async Task<Results<CreatedAtRoute<CreatePropertyResponse>, BadRequest<Error>>> CreateProperty(
            [FromServices] ISender sender,
            [FromBody] CreatePropertyRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new CreatePropertyCommand
            {
                Title = request.Title,
                Description = request.Description,
                RealEstateAgentId = request.RealEstateAgentId,
                Price = request.Price,
                Size = request.Size,
                Bedrooms = request.Bedrooms,
                Bathrooms = request.Bathrooms,
                Type = request.Type,
                Status = request.Status,
                TransactionType = request.TransactionType,
                Street = request.Street,
                Number = request.Number,
                Parish = request.Parish,
                Municipality = request.Municipality,
                District = request.District,
                PostalCode = request.PostalCode,
                Complement = request.Complement,
                GrossArea = request.GrossArea,
                LandArea = request.LandArea,
                WcCount = request.WcCount,
                Floor = request.Floor,
                ParkingSpaces = request.ParkingSpaces,
                Condominium = request.Condominium,
                Amenities = request.Amenities,
                YearBuilt = request.YearBuilt,
                EnergyRating = request.EnergyRating,
                Orientation = request.Orientation,
                HasElevator = request.HasElevator,
                HasGarage = request.HasGarage,
                HasPool = request.HasPool,
                IsFurnished = request.IsFurnished
            };

            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.CreatedAtRoute(result.Value!, "GetPropertyById", new { id = result.Value!.Id })
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<UpdatePropertyResponse>, BadRequest<Error>>> UpdateProperty(
            [FromServices] ISender sender,
            Guid id,
            [FromBody] UpdatePropertyRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdatePropertyCommand(
                id,
                request.Title,
                request.Description,
                request.Price,
                request.Status,
                request.Amenities
            );

            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<DeletePropertyResponse>, NotFound<Error>>> DeleteProperty(
            [FromServices] ISender sender,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var command = new DeletePropertyCommand(id);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.NotFound(result.Error!);
        }
    }

    private static class Queries
    {
        public static async Task<Results<Ok<GetPropertiesResponse>, BadRequest<Error>>> GetProperties(
            [FromServices] ISender sender,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? type = null,
            [FromQuery] int? status = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? municipality = null,
            [FromQuery] int? minBedrooms = null,
            [FromQuery] bool? featuredOnly = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetPropertiesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Type = type,
                Status = status,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Municipality = municipality,
                MinBedrooms = minBedrooms,
                FeaturedOnly = featuredOnly
            };

            var result = await sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error);
        }

        public static async Task<Results<Ok<PropertyDetailResponse>, NotFound<Error>>> GetPropertyById(
            [FromServices] ISender sender,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var query = new GetPropertyByIdQuery { Id = id };
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.NotFound(result.Error);
        }
    }
}

