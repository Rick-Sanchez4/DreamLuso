using DreamLuso.Domain.Model;

namespace DreamLuso.Domain.Core.Interfaces;

public interface IPropertyImageRepository : IRepository<PropertyImage>
{
    Task<PropertyImage> SaveAsync(PropertyImage image);
    Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(Guid propertyId);
    Task<PropertyImage?> GetPrimaryImageAsync(Guid propertyId);
    Task DeleteByPropertyIdAsync(Guid propertyId);
    Task<PropertyImage> UpdateAsync(PropertyImage image);
}
