using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Properties.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Properties.Queries.GetPropertyById;

public class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, Result<PropertyDetailResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPropertyByIdQueryHandler> _logger;

    public GetPropertyByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPropertyByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PropertyDetailResponse, Success, Error>> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.PropertyRepository.GetByIdWithAllDetailsAsync(request.Id);
        
        if (property == null)
        {
            _logger.LogWarning("Imóvel não encontrado: {PropertyId}", request.Id);
            return Error.PropertyNotFound;
        }

        var prop = (Property)property;

        // Increment view count (fire and forget)
        prop.IncrementViewCount();
        await _unitOfWork.PropertyRepository.UpdateAsync(prop);
        await _unitOfWork.CommitAsync(cancellationToken);

        var response = new PropertyDetailResponse
        {
            Id = prop.Id,
            Title = prop.Title,
            Description = prop.Description,
            Price = prop.Price,
            Size = prop.Size,
            GrossArea = prop.GrossArea,
            LandArea = prop.LandArea,
            Bedrooms = prop.Bedrooms,
            Bathrooms = prop.Bathrooms,
            WcCount = prop.WcCount,
            Floor = prop.Floor,
            TotalFloors = prop.TotalFloors,
            ParkingSpaces = prop.ParkingSpaces,
            Type = prop.Type.ToString(),
            Status = prop.Status.ToString(),
            TransactionType = prop.TransactionType.ToString(),
            PricePerSqm = prop.PricePerSqm,
            Condominium = prop.Condominium,
            Amenities = prop.Amenities,
            EnergyRating = prop.EnergyRating,
            YearBuilt = prop.YearBuilt,
            YearRenovated = prop.YearRenovated,
            Orientation = prop.Orientation,
            ViewType = prop.ViewType,
            IsFeatured = prop.IsFeatured,
            ViewCount = prop.ViewCount,
            HasElevator = prop.HasElevator,
            HasGarage = prop.HasGarage,
            HasGarden = prop.HasGarden,
            HasPool = prop.HasPool,
            HasBalcony = prop.HasBalcony,
            HasTerrace = prop.HasTerrace,
            HasAirConditioning = prop.HasAirConditioning,
            HasCentralHeating = prop.HasCentralHeating,
            IsFurnished = prop.IsFurnished,
            PetsAllowed = prop.PetsAllowed,
            VideoUrl = prop.VideoUrl,
            VirtualTourUrl = prop.VirtualTourUrl,
            Street = prop.Address?.Street ?? "",
            Municipality = prop.Address?.Municipality ?? "",
            District = prop.Address?.District ?? "",
            PostalCode = prop.Address?.PostalCode ?? "",
            Parish = prop.Address?.Parish ?? "",
            Complement = prop.Address?.Complement ?? "",
            AgentId = prop.RealEstateAgentId,
            AgentName = prop.RealEstateAgent?.User?.Name?.FullName ?? "Agente não encontrado",
            ImageUrls = prop.Images.Select(img => img.ImageUrl).ToList(),
            CreatedAt = prop.CreatedAt,
            UpdatedAt = prop.UpdatedAt
        };

        _logger.LogInformation("Imóvel encontrado: {PropertyId}, {PropertyTitle}", prop.Id, prop.Title);

        return response;
    }
}

