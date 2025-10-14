using DreamLuso.Domain.Model;

namespace DreamLuso.Domain.Core.Interfaces;

public interface IPropertyRepository : IRepository<Property>
{
    Task<Property> SaveAsync(Property property);
    Task<IEnumerable<Property>> GetByAgentIdAsync(Guid agentId);
    Task<IEnumerable<Property>> GetByStatusAsync(PropertyStatus status);
    Task<IEnumerable<Property>> GetByTypeAsync(PropertyType type);
    Task<IEnumerable<Property>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Property>> GetByLocationAsync(string municipality, string? district = null);
    Task<IEnumerable<Property>> GetFeaturedPropertiesAsync(int count = 10);
    Task<IEnumerable<Property>> GetRecentPropertiesAsync(int count = 20);
    Task<IEnumerable<Property>> SearchPropertiesAsync(string searchTerm);
    Task<Property?> GetByIdWithImagesAsync(Guid id);
    Task<Property?> GetByIdWithAllDetailsAsync(Guid id);
    Task<Property> UpdateAsync(Property property);
}
