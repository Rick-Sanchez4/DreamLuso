using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Properties.Commands.CreateProperty;
using DreamLuso.Application.CQ.Properties.Commands.UpdateProperty;
using DreamLuso.Application.CQ.Properties.Commands.DeleteProperty;
using DreamLuso.Application.CQ.Properties.Queries.GetProperties;
using DreamLuso.Application.CQ.Properties.Queries.GetPropertyById;
using DreamLuso.Application.CQ.Properties.Common;
using DreamLuso.Domain.Core.Uow;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Linq;

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

        // GET /api/properties/agent/{agentId} - Obter imóveis do agente
        properties.MapGet("/agent/{agentId:guid}", Queries.GetPropertiesByAgent)
            .WithName("GetPropertiesByAgent")
            .Produces<GetPropertiesResponse>(200)
            .Produces<Error>(404);

        // POST /api/properties - Criar novo imóvel
        properties.MapPost("/", Commands.CreateProperty)
            .WithName("CreateProperty")
            .DisableAntiforgery() // Required for multipart/form-data
            .Accepts<HttpRequest>("multipart/form-data")
            .Produces<CreatePropertyResponse>(201)
            .Produces<Error>(400);

        // PUT /api/properties/{id} - Atualizar imóvel
        properties.MapPut("/{id:guid}", Commands.UpdateProperty)
            .WithName("UpdateProperty")
            .DisableAntiforgery() // Required for multipart/form-data
            .Accepts<HttpRequest>("multipart/form-data")
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
            HttpRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!request.HasFormContentType)
            {
                return TypedResults.BadRequest(new Error("InvalidContentType", "Request must be multipart/form-data"));
            }

            var form = await request.ReadFormAsync(cancellationToken);
            
            // Parse form fields
            var title = form["title"].ToString();
            var description = form["description"].ToString();
            var realEstateAgentIdStr = form["realEstateAgentId"].ToString();
            
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(realEstateAgentIdStr))
            {
                return TypedResults.BadRequest(new Error("ValidationFailed", "Title, Description, and RealEstateAgentId are required"));
            }

            if (!Guid.TryParse(realEstateAgentIdStr, out var realEstateAgentId))
            {
                return TypedResults.BadRequest(new Error("InvalidGuid", "Invalid RealEstateAgentId format"));
            }

            // Parse numeric fields
            decimal.TryParse(form["price"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var price);
            double.TryParse(form["size"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var size);
            int.TryParse(form["bedrooms"].ToString(), out var bedrooms);
            int.TryParse(form["bathrooms"].ToString(), out var bathrooms);
            int.TryParse(form["type"].ToString(), out var type);
            int.TryParse(form["status"].ToString(), out var status);
            int.TryParse(form["transactionType"].ToString(), out var transactionType);

            // Parse optional numeric fields
            double? grossArea = null;
            if (form.ContainsKey("grossArea") && !string.IsNullOrWhiteSpace(form["grossArea"].ToString()))
            {
                if (double.TryParse(form["grossArea"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var ga))
                    grossArea = ga;
            }

            double? landArea = null;
            if (form.ContainsKey("landArea") && !string.IsNullOrWhiteSpace(form["landArea"].ToString()))
            {
                if (double.TryParse(form["landArea"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var la))
                    landArea = la;
            }

            int? wcCount = null;
            if (form.ContainsKey("wcCount") && !string.IsNullOrWhiteSpace(form["wcCount"].ToString()))
            {
                if (int.TryParse(form["wcCount"].ToString(), out var wc))
                    wcCount = wc;
            }

            int? floor = null;
            if (form.ContainsKey("floor") && !string.IsNullOrWhiteSpace(form["floor"].ToString()))
            {
                if (int.TryParse(form["floor"].ToString(), out var f))
                    floor = f;
            }

            int? parkingSpaces = null;
            if (form.ContainsKey("parkingSpaces") && !string.IsNullOrWhiteSpace(form["parkingSpaces"].ToString()))
            {
                if (int.TryParse(form["parkingSpaces"].ToString(), out var ps))
                    parkingSpaces = ps;
            }

            decimal? condominium = null;
            if (form.ContainsKey("condominium") && !string.IsNullOrWhiteSpace(form["condominium"].ToString()))
            {
                if (decimal.TryParse(form["condominium"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var c))
                    condominium = c;
            }

            int? yearBuilt = null;
            if (form.ContainsKey("yearBuilt") && !string.IsNullOrWhiteSpace(form["yearBuilt"].ToString()))
            {
                if (int.TryParse(form["yearBuilt"].ToString(), out var yb))
                    yearBuilt = yb;
            }

            // Parse boolean fields
            bool.TryParse(form["hasElevator"].ToString(), out var hasElevator);
            bool.TryParse(form["hasGarage"].ToString(), out var hasGarage);
            bool.TryParse(form["hasPool"].ToString(), out var hasPool);
            bool.TryParse(form["isFurnished"].ToString(), out var isFurnished);

            // Get images from form
            var images = form.Files.Where(f => f.Name == "images").ToList();

            var command = new CreatePropertyCommand
            {
                Title = title,
                Description = description,
                RealEstateAgentId = realEstateAgentId,
                Price = price,
                Size = size,
                Bedrooms = bedrooms,
                Bathrooms = bathrooms,
                Type = type,
                Status = status,
                TransactionType = transactionType,
                Street = form["street"].ToString() ?? string.Empty,
                Number = form["number"].ToString() ?? "0",
                Parish = form["parish"].ToString() ?? string.Empty,
                Municipality = form["municipality"].ToString() ?? string.Empty,
                District = form["district"].ToString() ?? string.Empty,
                PostalCode = form["postalCode"].ToString() ?? string.Empty,
                Complement = form["complement"].ToString(),
                GrossArea = grossArea,
                LandArea = landArea,
                WcCount = wcCount,
                Floor = floor,
                ParkingSpaces = parkingSpaces,
                Condominium = condominium,
                Amenities = form["amenities"].ToString(),
                YearBuilt = yearBuilt,
                EnergyRating = form["energyRating"].ToString(),
                Orientation = form["orientation"].ToString(),
                HasElevator = hasElevator,
                HasGarage = hasGarage,
                HasPool = hasPool,
                IsFurnished = isFurnished,
                Images = images.Count > 0 ? images : null
            };

            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.CreatedAtRoute(result.Value!, "GetPropertyById", new { id = result.Value!.Id })
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<UpdatePropertyResponse>, BadRequest<Error>>> UpdateProperty(
            [FromServices] ISender sender,
            Guid id,
            HttpRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!request.HasFormContentType)
            {
                return TypedResults.BadRequest(new Error("InvalidContentType", "Request must be multipart/form-data"));
            }

            var form = await request.ReadFormAsync(cancellationToken);
            
            // Parse required fields
            var title = form["title"].ToString();
            var description = form["description"].ToString();
            
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                return TypedResults.BadRequest(new Error("ValidationFailed", "Title and Description are required"));
            }

            // Parse numeric fields
            decimal.TryParse(form["price"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var price);
            int.TryParse(form["status"].ToString(), out var status);

            // Parse optional fields
            double? size = null;
            if (form.ContainsKey("size") && !string.IsNullOrWhiteSpace(form["size"].ToString()))
            {
                if (double.TryParse(form["size"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var s))
                    size = s;
            }

            int? bedrooms = null;
            if (form.ContainsKey("bedrooms") && !string.IsNullOrWhiteSpace(form["bedrooms"].ToString()))
            {
                if (int.TryParse(form["bedrooms"].ToString(), out var b))
                    bedrooms = b;
            }

            int? bathrooms = null;
            if (form.ContainsKey("bathrooms") && !string.IsNullOrWhiteSpace(form["bathrooms"].ToString()))
            {
                if (int.TryParse(form["bathrooms"].ToString(), out var ba))
                    bathrooms = ba;
            }

            int? type = null;
            if (form.ContainsKey("type") && !string.IsNullOrWhiteSpace(form["type"].ToString()))
            {
                if (int.TryParse(form["type"].ToString(), out var t))
                    type = t;
            }

            int? transactionType = null;
            if (form.ContainsKey("transactionType") && !string.IsNullOrWhiteSpace(form["transactionType"].ToString()))
            {
                if (int.TryParse(form["transactionType"].ToString(), out var tt))
                    transactionType = tt;
            }

            // Parse other optional fields
            double? grossArea = null;
            if (form.ContainsKey("grossArea") && !string.IsNullOrWhiteSpace(form["grossArea"].ToString()))
            {
                if (double.TryParse(form["grossArea"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var ga))
                    grossArea = ga;
            }

            double? landArea = null;
            if (form.ContainsKey("landArea") && !string.IsNullOrWhiteSpace(form["landArea"].ToString()))
            {
                if (double.TryParse(form["landArea"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var la))
                    landArea = la;
            }

            int? wcCount = null;
            if (form.ContainsKey("wcCount") && !string.IsNullOrWhiteSpace(form["wcCount"].ToString()))
            {
                if (int.TryParse(form["wcCount"].ToString(), out var wc))
                    wcCount = wc;
            }

            int? floor = null;
            if (form.ContainsKey("floor") && !string.IsNullOrWhiteSpace(form["floor"].ToString()))
            {
                if (int.TryParse(form["floor"].ToString(), out var f))
                    floor = f;
            }

            int? parkingSpaces = null;
            if (form.ContainsKey("parkingSpaces") && !string.IsNullOrWhiteSpace(form["parkingSpaces"].ToString()))
            {
                if (int.TryParse(form["parkingSpaces"].ToString(), out var ps))
                    parkingSpaces = ps;
            }

            decimal? condominium = null;
            if (form.ContainsKey("condominium") && !string.IsNullOrWhiteSpace(form["condominium"].ToString()))
            {
                if (decimal.TryParse(form["condominium"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var c))
                    condominium = c;
            }

            int? yearBuilt = null;
            if (form.ContainsKey("yearBuilt") && !string.IsNullOrWhiteSpace(form["yearBuilt"].ToString()))
            {
                if (int.TryParse(form["yearBuilt"].ToString(), out var yb))
                    yearBuilt = yb;
            }

            // Parse boolean fields
            bool? hasElevator = null;
            if (form.ContainsKey("hasElevator") && !string.IsNullOrWhiteSpace(form["hasElevator"].ToString()))
            {
                if (bool.TryParse(form["hasElevator"].ToString(), out var he))
                    hasElevator = he;
            }

            bool? hasGarage = null;
            if (form.ContainsKey("hasGarage") && !string.IsNullOrWhiteSpace(form["hasGarage"].ToString()))
            {
                if (bool.TryParse(form["hasGarage"].ToString(), out var hg))
                    hasGarage = hg;
            }

            bool? hasPool = null;
            if (form.ContainsKey("hasPool") && !string.IsNullOrWhiteSpace(form["hasPool"].ToString()))
            {
                if (bool.TryParse(form["hasPool"].ToString(), out var hp))
                    hasPool = hp;
            }

            bool? isFurnished = null;
            if (form.ContainsKey("isFurnished") && !string.IsNullOrWhiteSpace(form["isFurnished"].ToString()))
            {
                if (bool.TryParse(form["isFurnished"].ToString(), out var ifu))
                    isFurnished = ifu;
            }

            // Get images from form
            var images = form.Files.Where(f => f.Name == "images").ToList();

            var command = new UpdatePropertyCommand(
                id,
                title,
                description,
                price,
                status,
                form["amenities"].ToString(),
                condominium,
                size,
                bedrooms,
                bathrooms,
                type,
                transactionType,
                form["street"].ToString(),
                form["number"].ToString(),
                form["parish"].ToString(),
                form["municipality"].ToString(),
                form["district"].ToString(),
                form["postalCode"].ToString(),
                form["complement"].ToString(),
                grossArea,
                landArea,
                wcCount,
                floor,
                parkingSpaces,
                yearBuilt,
                form["energyRating"].ToString(),
                form["orientation"].ToString(),
                hasElevator ?? false,
                hasGarage ?? false,
                hasPool ?? false,
                isFurnished ?? false,
                images.Count > 0 ? images : null
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
            [FromQuery] int? minBathrooms = null,
            [FromQuery] bool? featuredOnly = null,
            [FromQuery] int? transactionType = null,
            [FromQuery] Guid? agentId = null,
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
                MinBathrooms = minBathrooms,
                FeaturedOnly = featuredOnly,
                TransactionType = transactionType,
                AgentId = agentId
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

        public static async Task<Results<Ok<IEnumerable<PropertyResponse>>, NotFound<Error>>> GetPropertiesByAgent(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid agentId,
            CancellationToken cancellationToken = default)
        {
            var properties = await unitOfWork.PropertyRepository.GetByAgentIdAsync(agentId);

            var response = properties.Select(p => new PropertyResponse
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
                Size = p.Size,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Type = p.Type.ToString(),
                Status = p.Status.ToString(),
                TransactionType = p.TransactionType.ToString(),
                Street = p.Address.Street,
                Municipality = p.Address.Municipality,
                District = p.Address.District,
                PostalCode = p.Address.PostalCode,
                CreatedAt = p.CreatedAt
            });

            return TypedResults.Ok(response);
        }
    }
}

