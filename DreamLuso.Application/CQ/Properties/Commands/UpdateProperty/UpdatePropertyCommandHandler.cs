using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Properties.Commands.UpdateProperty;

public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, Result<UpdatePropertyResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePropertyCommandHandler> _logger;

    public UpdatePropertyCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdatePropertyCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<UpdatePropertyResponse, Success, Error>> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var propertyObj = await _unitOfWork.PropertyRepository.GetByIdAsync(request.Id);
        if (propertyObj == null)
        {
            _logger.LogWarning("Imóvel não encontrado: {PropertyId}", request.Id);
            return Error.PropertyNotFound;
        }

        var property = (Property)propertyObj;

        // Update basic fields
        property.Title = request.Title;
        property.Description = request.Description;
        property.UpdatePrice(request.Price);
        property.UpdateStatus((PropertyStatus)request.Status);
        property.Amenities = request.Amenities;
        
        // Update optional fields if provided
        if (request.Size.HasValue) property.Size = request.Size.Value;
        if (request.Bedrooms.HasValue) property.Bedrooms = request.Bedrooms.Value;
        if (request.Bathrooms.HasValue) property.Bathrooms = request.Bathrooms.Value;
        if (request.Type.HasValue) property.Type = (PropertyType)request.Type.Value;
        if (request.TransactionType.HasValue) property.TransactionType = (TransactionType)request.TransactionType.Value;
        
        // Update address - ValueObject owned by Property
        if (!string.IsNullOrEmpty(request.Street)) property.Address.Street = request.Street;
        if (!string.IsNullOrEmpty(request.Number)) property.Address.Number = request.Number;
        if (!string.IsNullOrEmpty(request.Parish)) property.Address.Parish = request.Parish;
        if (!string.IsNullOrEmpty(request.Municipality)) property.Address.Municipality = request.Municipality;
        if (!string.IsNullOrEmpty(request.District)) property.Address.District = request.District;
        if (!string.IsNullOrEmpty(request.PostalCode)) property.Address.PostalCode = request.PostalCode;
        if (!string.IsNullOrEmpty(request.Complement)) property.Address.Complement = request.Complement;
        
        // Update additional fields
        if (request.GrossArea.HasValue) property.GrossArea = request.GrossArea.Value;
        if (request.LandArea.HasValue) property.LandArea = request.LandArea;
        if (request.WcCount.HasValue) property.WcCount = request.WcCount;
        if (request.Floor.HasValue) property.Floor = request.Floor;
        if (request.ParkingSpaces.HasValue) property.ParkingSpaces = request.ParkingSpaces.Value;
        if (request.Condominium.HasValue) property.Condominium = request.Condominium;
        if (request.YearBuilt.HasValue) property.YearBuilt = request.YearBuilt;
        if (!string.IsNullOrEmpty(request.EnergyRating)) property.EnergyRating = request.EnergyRating;
        if (!string.IsNullOrEmpty(request.Orientation)) property.Orientation = request.Orientation;
        if (request.HasElevator.HasValue) property.HasElevator = request.HasElevator.Value;
        if (request.HasGarage.HasValue) property.HasGarage = request.HasGarage.Value;
        if (request.HasPool.HasValue) property.HasPool = request.HasPool.Value;
        if (request.IsFurnished.HasValue) property.IsFurnished = request.IsFurnished.Value;
        
        // Update images if provided
        if (request.Images != null && request.Images.Count > 0)
        {
            int currentMaxOrder = property.Images.Any() ? property.Images.Max(i => i.DisplayOrder) : -1;
            
            for (int i = 0; i < request.Images.Count; i++)
            {
                var imageFile = request.Images[i];
                
                // Save file and get filename
                var fileName = await _unitOfWork.FileStorageService.SaveFileAsync(imageFile, cancellationToken);
                
                // Create NEW PropertyImage entity (not attached to property yet)
                var propertyImage = new PropertyImage
                {
                    Id = Guid.NewGuid(), // Explicitly set new ID
                    PropertyId = property.Id,
                    ImageUrl = fileName,
                    IsPrimary = !property.Images.Any() && i == 0,
                    DisplayOrder = currentMaxOrder + i + 1,
                    Type = ImageType.Exterior,
                    CreatedAt = DateTime.UtcNow
                };
                
                // Add to repository directly to ensure it's tracked as Added, not Modified
                await _unitOfWork.PropertyImageRepository.SaveAsync(propertyImage);
            }
        }

        // Update timestamp
        property.UpdatedAt = DateTime.UtcNow;
        
        _logger.LogInformation("About to save property {PropertyId} with {ImageCount} images. Title: {Title}", 
            property.Id, property.Images.Count, property.Title);
        
        // Since property is already being tracked from GetByIdAsync,
        // EF will automatically detect changes and update
        try
        {
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error updating property {PropertyId}. Error: {Message}", property.Id, ex.Message);
            throw;
        }

        _logger.LogInformation("Imóvel atualizado: {PropertyId}", property.Id);

        var response = new UpdatePropertyResponse(
            property.Id,
            property.Title,
            property.Price,
            property.Status.ToString(),
            property.UpdatedAt ?? DateTime.UtcNow
        );

        return response;
    }
}

