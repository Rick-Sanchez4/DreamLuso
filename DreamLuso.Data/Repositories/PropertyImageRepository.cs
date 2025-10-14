using DreamLuso.Data.Context;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamLuso.Data.Repositories;

public class PropertyImageRepository : Repository<PropertyImage>, IPropertyImageRepository
{
    public PropertyImageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<PropertyImage> SaveAsync(PropertyImage image)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        await _dbSet.AddAsync(image);
        return image;
    }

    public async Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(Guid propertyId)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID cannot be empty", nameof(propertyId));

        return await _dbSet
            .Where(pi => pi.PropertyId == propertyId)
            .OrderBy(pi => pi.DisplayOrder)
            .ToListAsync();
    }

    public async Task<PropertyImage?> GetPrimaryImageAsync(Guid propertyId)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID cannot be empty", nameof(propertyId));

        return await _dbSet
            .FirstOrDefaultAsync(pi => pi.PropertyId == propertyId && pi.IsPrimary);
    }

    public async Task DeleteByPropertyIdAsync(Guid propertyId)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID cannot be empty", nameof(propertyId));

        var images = await _dbSet
            .Where(pi => pi.PropertyId == propertyId)
            .ToListAsync();

        _dbSet.RemoveRange(images);
    }

    public override async Task<PropertyImage> UpdateAsync(PropertyImage image)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        _dbSet.Update(image);
        return image;
    }
}
