using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Properties.Commands.CreateProperty;

public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, Result<CreatePropertyResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePropertyCommandHandler> _logger;

    public CreatePropertyCommandHandler(IUnitOfWork unitOfWork, ILogger<CreatePropertyCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreatePropertyResponse, Success, Error>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        // Verify agent exists
        var agent = await _unitOfWork.RealEstateAgentRepository.GetByIdAsync(request.RealEstateAgentId);
        if (agent == null)
        {
            _logger.LogWarning("Agent não encontrado: {AgentId}", request.RealEstateAgentId);
            return Error.AgentNotFound;
        }

        // Create address
        var address = new Address(
            request.Street,
            request.Number,
            request.Parish,
            request.Municipality,
            request.District,
            request.PostalCode,
            request.Complement
        );

        // Create property
        var property = new Property
        {
            Title = request.Title,
            Description = request.Description,
            RealEstateAgentId = request.RealEstateAgentId,
            RealEstateAgent = (RealEstateAgent)agent,
            Address = address,
            Type = (PropertyType)request.Type,
            Status = (PropertyStatus)request.Status,
            TransactionType = (TransactionType)request.TransactionType,
            Size = request.Size,
            GrossArea = request.GrossArea,
            LandArea = request.LandArea,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            WcCount = request.WcCount,
            Floor = request.Floor,
            ParkingSpaces = request.ParkingSpaces,
            Price = request.Price,
            PricePerSqm = request.Size > 0 ? request.Price / (decimal)request.Size : null,
            Condominium = request.Condominium,
            Amenities = request.Amenities,
            YearBuilt = request.YearBuilt,
            EnergyRating = request.EnergyRating,
            Orientation = request.Orientation,
            HasElevator = request.HasElevator,
            HasGarage = request.HasGarage,
            HasPool = request.HasPool,
            IsFurnished = request.IsFurnished,
            IsActive = true,
            DateListed = DateTime.UtcNow
        };

        var savedProperty = await _unitOfWork.PropertyRepository.SaveAsync(property);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Imóvel criado com sucesso: {PropertyId}, {PropertyTitle}", savedProperty.Id, property.Title);

        var response = new CreatePropertyResponse
        {
            Id = property.Id,
            Title = property.Title,
            Price = property.Price,
            CreatedAt = DateTime.UtcNow
        };

        return response;
    }
}

