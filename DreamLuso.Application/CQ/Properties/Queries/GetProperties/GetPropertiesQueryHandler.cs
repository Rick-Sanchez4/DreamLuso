using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Properties.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Properties.Queries.GetProperties;

public class GetPropertiesQueryHandler : IRequestHandler<GetPropertiesQuery, Result<GetPropertiesResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPropertiesQueryHandler> _logger;

    public GetPropertiesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPropertiesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetPropertiesResponse, Success, Error>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        var allProperties = await _unitOfWork.PropertyRepository.GetAllAsync();
        var properties = allProperties.Cast<Property>().AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            properties = properties.Where(p =>
                p.Title.ToLower().Contains(searchLower) ||
                p.Description.ToLower().Contains(searchLower) ||
                p.Address.Municipality.ToLower().Contains(searchLower) ||
                p.Address.District.ToLower().Contains(searchLower));
        }

        if (request.Type.HasValue)
        {
            properties = properties.Where(p => (int)p.Type == request.Type.Value);
        }

        if (request.Status.HasValue)
        {
            properties = properties.Where(p => (int)p.Status == request.Status.Value);
        }

        if (request.MinPrice.HasValue)
        {
            properties = properties.Where(p => p.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            properties = properties.Where(p => p.Price <= request.MaxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Municipality))
        {
            properties = properties.Where(p => p.Address.Municipality.Equals(request.Municipality, StringComparison.OrdinalIgnoreCase));
        }

        if (request.MinBedrooms.HasValue)
        {
            properties = properties.Where(p => p.Bedrooms >= request.MinBedrooms.Value);
        }

        if (request.FeaturedOnly == true)
        {
            properties = properties.Where(p => p.IsFeatured);
        }

        // Get total count
        var totalCount = properties.Count();

        // Apply pagination
        var paginatedProperties = properties
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var response = new GetPropertiesResponse
        {
            Properties = paginatedProperties.Select(p => new PropertyResponse
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
                ImageUrls = p.Images.Select(img => img.ImageUrl).ToList(),
                CreatedAt = p.CreatedAt
            }),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };

        _logger.LogInformation("Listados {Count} imóveis de um total de {Total}", paginatedProperties.Count, totalCount);

        return response;
    }
}

