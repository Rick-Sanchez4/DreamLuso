using DreamLuso.Data.Context;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamLuso.Data.Repositories;

public class PropertyRepository : Repository<Property>, IPropertyRepository
{
    public PropertyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Property> SaveAsync(Property property)
    {
        if (property == null)
            throw new ArgumentNullException(nameof(property));

        await _dbSet.AddAsync(property);
        return property;
    }

    public async Task<IEnumerable<Property>> GetByAgentIdAsync(Guid agentId)
    {
        if (agentId == Guid.Empty)
            throw new ArgumentException("Agent ID cannot be empty", nameof(agentId));

        return await _dbSet
            .Include(p => p.Address)
            .Include(p => p.Images)
            .Include(p => p.RealEstateAgent)
                .ThenInclude(a => a.User)
            .Where(p => p.RealEstateAgentId == agentId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetByStatusAsync(PropertyStatus status)
    {
        return await _dbSet
            .Include(p => p.Address)
            .Include(p => p.Images)
            .Include(p => p.RealEstateAgent)
                .ThenInclude(a => a.User)
            .Where(p => p.Status == status && p.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetByTypeAsync(PropertyType type)
    {
        return await _dbSet
            .Include(p => p.Address)
            .Include(p => p.Images)
            .Include(p => p.RealEstateAgent)
                .ThenInclude(a => a.User)
            .Where(p => p.Type == type && p.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _dbSet
            .Include(p => p.Address)
            .Include(p => p.Images)
            .Include(p => p.RealEstateAgent)
                .ThenInclude(a => a.User)
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice && p.IsActive)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetByLocationAsync(string municipality, string? district = null)
    {
        if (string.IsNullOrWhiteSpace(municipality))
            throw new ArgumentException("Municipality cannot be null or empty", nameof(municipality));

        var query = _dbSet
            .Include(p => p.Address)
            .Include(p => p.Images)
            .Include(p => p.RealEstateAgent)
                .ThenInclude(a => a.User)
            .Where(p => p.Address.Municipality.Contains(municipality) && p.IsActive);

        if (!string.IsNullOrWhiteSpace(district))
        {
            query = query.Where(p => p.Address.District.Contains(district));
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetFeaturedPropertiesAsync(int count = 10)
    {
        return await _dbSet
            .Include(p => p.Address)
            .Include(p => p.Images)
            .Include(p => p.RealEstateAgent)
                .ThenInclude(a => a.User)
            .Where(p => p.IsFeatured && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetRecentPropertiesAsync(int count = 20)
    {
        return await _dbSet
            .Include(p => p.Address)
            .Include(p => p.Images)
            .Include(p => p.RealEstateAgent)
                .ThenInclude(a => a.User)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> SearchPropertiesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term cannot be null or empty", nameof(searchTerm));

        var searchLower = searchTerm.ToLower();
        return await _dbSet
            .Include(p => p.Address)
            .Include(p => p.Images)
            .Include(p => p.RealEstateAgent)
                .ThenInclude(a => a.User)
            .Where(p => p.IsActive && (
                p.Title.ToLower().Contains(searchLower) ||
                p.Description.ToLower().Contains(searchLower) ||
                p.Address.Municipality.ToLower().Contains(searchLower) ||
                p.Address.District.ToLower().Contains(searchLower)))
            .ToListAsync();
    }

    public async Task<Property?> GetByIdWithImagesAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty", nameof(id));

        return await _dbSet
            .Include(p => p.Images)
            .Include(p => p.Address)
            .Include(p => p.RealEstateAgent)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Property?> GetByIdWithAllDetailsAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty", nameof(id));

        return await _dbSet
            .Include(p => p.Address)
            .Include(p => p.Images)
            .Include(p => p.RealEstateAgent)
                .ThenInclude(a => a.User)
            .Include(p => p.PropertyVisits)
                .ThenInclude(pv => pv.Client)
                    .ThenInclude(c => c.User)
            .Include(p => p.Contracts)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<Property> UpdateAsync(Property property)
    {
        if (property == null)
            throw new ArgumentNullException(nameof(property));

        _dbSet.Update(property);
        return property;
    }
}
