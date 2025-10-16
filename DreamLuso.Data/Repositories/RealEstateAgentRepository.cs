using DreamLuso.Data.Context;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamLuso.Data.Repositories;

public class RealEstateAgentRepository : Repository<RealEstateAgent>, IRealEstateAgentRepository
{
    public RealEstateAgentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<RealEstateAgent>> GetAllAsync()
    {
        return await _dbSet
            .Include(a => a.User)
            .ToListAsync();
    }

    public override async Task<RealEstateAgent> SaveAsync(RealEstateAgent agent)
    {
        if (agent == null)
            throw new ArgumentNullException(nameof(agent));

        await _dbSet.AddAsync(agent);
        return agent;
    }

    public async Task<RealEstateAgent?> GetByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        return await _dbSet
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.UserId == userId);
    }

    public async Task<RealEstateAgent?> GetByLicenseNumberAsync(string licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new ArgumentException("License number cannot be null or empty", nameof(licenseNumber));

        return await _dbSet
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.LicenseNumber == licenseNumber);
    }

    public async Task<IEnumerable<RealEstateAgent>> GetActiveAgentsAsync()
    {
        return await _dbSet
            .Include(a => a.User)
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.Rating)
            .ToListAsync();
    }

    public async Task<IEnumerable<RealEstateAgent>> GetTopAgentsByRevenueAsync(int top = 10)
    {
        return await _dbSet
            .Include(a => a.User)
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.TotalRevenue)
            .Take(top)
            .ToListAsync();
    }

    public async Task<IEnumerable<RealEstateAgent>> GetAgentsBySpecializationAsync(string specialization)
    {
        if (string.IsNullOrWhiteSpace(specialization))
            throw new ArgumentException("Specialization cannot be null or empty", nameof(specialization));

        return await _dbSet
            .Include(a => a.User)
            .Where(a => a.IsActive && a.Specialization != null && 
                       a.Specialization.Contains(specialization))
            .ToListAsync();
    }

    public override async Task<RealEstateAgent> UpdateAsync(RealEstateAgent agent)
    {
        if (agent == null)
            throw new ArgumentNullException(nameof(agent));

        _dbSet.Update(agent);
        return agent;
    }
}
