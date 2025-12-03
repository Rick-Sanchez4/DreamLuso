using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Properties.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Clients.Queries.GetFavorites;

public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, Result<GetFavoritesResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetFavoritesQueryHandler> _logger;

    public GetFavoritesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetFavoritesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetFavoritesResponse, Success, Error>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
    {
        // Verify client exists and load with favorite properties
        var clientObj = await _unitOfWork.ClientRepository.GetByIdWithFavoritesAsync(request.ClientId);
        if (clientObj == null)
        {
            _logger.LogWarning("Cliente não encontrado: {ClientId}", request.ClientId);
            return Error.ClientNotFound;
        }

        var client = (Client)clientObj;

        // Get favorite properties (already loaded with relations)
        var favoriteProperties = client.FavoriteProperties.ToList();

        var response = new GetFavoritesResponse
        {
            Properties = favoriteProperties.Select(p => new PropertyResponse
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
                EnergyRating = p.EnergyRating,
                YearBuilt = p.YearBuilt,
                IsFeatured = p.IsFeatured,
                ViewCount = p.ViewCount,
                Street = p.Address.Street,
                Municipality = p.Address.Municipality,
                District = p.Address.District,
                PostalCode = p.Address.PostalCode,
                AgentId = p.RealEstateAgentId,
                AgentName = p.RealEstateAgent.User.Name.FullName,
                ImageUrls = p.Images.OrderBy(img => img.DisplayOrder).Select(img => img.ImageUrl).ToList(),
                CreatedAt = p.CreatedAt
            }),
            TotalCount = favoriteProperties.Count
        };

        _logger.LogInformation("Favoritos obtidos para cliente: {ClientId}, Total: {Count}", 
            request.ClientId, response.TotalCount);

        return response;
    }
}

