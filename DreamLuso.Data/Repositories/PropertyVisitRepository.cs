using DreamLuso.Data.Context;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamLuso.Data.Repositories;

public class PropertyVisitRepository : Repository<PropertyVisit>, IPropertyVisitRepository
{
    public PropertyVisitRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<PropertyVisit> SaveAsync(PropertyVisit visit)
    {
        if (visit == null)
            throw new ArgumentNullException(nameof(visit));

        await _dbSet.AddAsync(visit);
        return visit;
    }

    public async Task<IEnumerable<PropertyVisit>> GetByPropertyIdAsync(Guid propertyId)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID cannot be empty", nameof(propertyId));

        return await _dbSet
            .Include(pv => pv.Client).ThenInclude(c => c.User)
            .Include(pv => pv.RealEstateAgent).ThenInclude(a => a.User)
            .Where(pv => pv.PropertyId == propertyId)
            .OrderByDescending(pv => pv.VisitDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PropertyVisit>> GetByClientIdAsync(Guid clientId)
    {
        if (clientId == Guid.Empty)
            throw new ArgumentException("Client ID cannot be empty", nameof(clientId));

        return await _dbSet
            .Include(pv => pv.Property).ThenInclude(p => p.Address)
            .Include(pv => pv.RealEstateAgent).ThenInclude(a => a.User)
            .Where(pv => pv.ClientId == clientId)
            .OrderByDescending(pv => pv.VisitDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PropertyVisit>> GetByAgentIdAsync(Guid agentId)
    {
        if (agentId == Guid.Empty)
            throw new ArgumentException("Agent ID cannot be empty", nameof(agentId));

        return await _dbSet
            .Include(pv => pv.Property).ThenInclude(p => p.Address)
            .Include(pv => pv.Client).ThenInclude(c => c.User)
            .Where(pv => pv.RealEstateAgentId == agentId)
            .OrderByDescending(pv => pv.VisitDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PropertyVisit>> GetByStatusAsync(VisitStatus status)
    {
        return await _dbSet
            .Include(pv => pv.Property).ThenInclude(p => p.Address)
            .Include(pv => pv.Client).ThenInclude(c => c.User)
            .Include(pv => pv.RealEstateAgent).ThenInclude(a => a.User)
            .Where(pv => pv.Status == status)
            .OrderByDescending(pv => pv.VisitDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PropertyVisit>> GetUpcomingVisitsAsync(DateOnly fromDate, DateOnly toDate)
    {
        return await _dbSet
            .Include(pv => pv.Property).ThenInclude(p => p.Address)
            .Include(pv => pv.Client).ThenInclude(c => c.User)
            .Include(pv => pv.RealEstateAgent).ThenInclude(a => a.User)
            .Where(pv => pv.VisitDate >= fromDate && pv.VisitDate <= toDate &&
                        (pv.Status == VisitStatus.Pending || pv.Status == VisitStatus.Confirmed))
            .OrderBy(pv => pv.VisitDate).ThenBy(pv => pv.TimeSlot)
            .ToListAsync();
    }

    public async Task<PropertyVisit?> GetByConfirmationTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        return await _dbSet
            .Include(pv => pv.Property).ThenInclude(p => p.Address)
            .Include(pv => pv.Client).ThenInclude(c => c.User)
            .Include(pv => pv.RealEstateAgent).ThenInclude(a => a.User)
            .FirstOrDefaultAsync(pv => pv.ConfirmationToken == token);
    }

    public async Task<bool> IsTimeSlotAvailableAsync(Guid propertyId, DateOnly date, TimeSlot timeSlot)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID cannot be empty", nameof(propertyId));

        var existingVisit = await _dbSet
            .AnyAsync(pv => pv.PropertyId == propertyId &&
                           pv.VisitDate == date &&
                           pv.TimeSlot == timeSlot &&
                           pv.Status != VisitStatus.Cancelled);

        return !existingVisit;
    }

    public override async Task<PropertyVisit> UpdateAsync(PropertyVisit visit)
    {
        if (visit == null)
            throw new ArgumentNullException(nameof(visit));

        _dbSet.Update(visit);
        return visit;
    }
}
