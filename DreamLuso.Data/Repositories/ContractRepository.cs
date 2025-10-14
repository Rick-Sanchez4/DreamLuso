using DreamLuso.Data.Context;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamLuso.Data.Repositories;

public class ContractRepository : Repository<Contract>, IContractRepository
{
    public ContractRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Contract> SaveAsync(Contract contract)
    {
        if (contract == null)
            throw new ArgumentNullException(nameof(contract));

        await _dbSet.AddAsync(contract);
        return contract;
    }

    public async Task<IEnumerable<Contract>> GetByPropertyIdAsync(Guid propertyId)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID cannot be empty", nameof(propertyId));

        return await _dbSet
            .Include(c => c.Property).ThenInclude(p => p.Address)
            .Include(c => c.Client).ThenInclude(cl => cl.User)
            .Include(c => c.RealEstateAgent).ThenInclude(a => a.User)
            .Where(c => c.PropertyId == propertyId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contract>> GetByClientIdAsync(Guid clientId)
    {
        if (clientId == Guid.Empty)
            throw new ArgumentException("Client ID cannot be empty", nameof(clientId));

        return await _dbSet
            .Include(c => c.Property).ThenInclude(p => p.Address)
            .Include(c => c.RealEstateAgent).ThenInclude(a => a.User)
            .Where(c => c.ClientId == clientId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contract>> GetByAgentIdAsync(Guid agentId)
    {
        if (agentId == Guid.Empty)
            throw new ArgumentException("Agent ID cannot be empty", nameof(agentId));

        return await _dbSet
            .Include(c => c.Property).ThenInclude(p => p.Address)
            .Include(c => c.Client).ThenInclude(cl => cl.User)
            .Where(c => c.RealEstateAgentId == agentId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contract>> GetByStatusAsync(ContractStatus status)
    {
        return await _dbSet
            .Include(c => c.Property).ThenInclude(p => p.Address)
            .Include(c => c.Client).ThenInclude(cl => cl.User)
            .Include(c => c.RealEstateAgent).ThenInclude(a => a.User)
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contract>> GetActiveContractsAsync()
    {
        return await _dbSet
            .Include(c => c.Property).ThenInclude(p => p.Address)
            .Include(c => c.Client).ThenInclude(cl => cl.User)
            .Include(c => c.RealEstateAgent).ThenInclude(a => a.User)
            .Where(c => c.Status == ContractStatus.Active)
            .OrderByDescending(c => c.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contract>> GetExpiringContractsAsync(int daysUntilExpiry)
    {
        var expiryDate = DateTime.UtcNow.AddDays(daysUntilExpiry);
        
        return await _dbSet
            .Include(c => c.Property).ThenInclude(p => p.Address)
            .Include(c => c.Client).ThenInclude(cl => cl.User)
            .Include(c => c.RealEstateAgent).ThenInclude(a => a.User)
            .Where(c => c.Status == ContractStatus.Active &&
                       c.EndDate.HasValue &&
                       c.EndDate.Value <= expiryDate &&
                       c.EndDate.Value > DateTime.UtcNow)
            .OrderBy(c => c.EndDate)
            .ToListAsync();
    }

    public async Task<Contract?> GetByIdWithDetailsAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty", nameof(id));

        return await _dbSet
            .Include(c => c.Property)
                .ThenInclude(p => p.Address)
            .Include(c => c.Property)
                .ThenInclude(p => p.Images)
            .Include(c => c.Client)
                .ThenInclude(cl => cl.User)
            .Include(c => c.RealEstateAgent)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task<Contract> UpdateAsync(Contract contract)
    {
        if (contract == null)
            throw new ArgumentNullException(nameof(contract));

        _dbSet.Update(contract);
        return contract;
    }
}
