using DreamLuso.Data.Context;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamLuso.Data.Repositories;

public class ClientRepository : Repository<Client>, IClientRepository
{
    public ClientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Client>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.User)
            .ToListAsync();
    }

    public override async Task<Client> SaveAsync(Client client)
    {
        if (client == null)
            throw new ArgumentNullException(nameof(client));

        await _dbSet.AddAsync(client);
        return client;
    }

    public async Task<Client?> GetByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        return await _dbSet
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Client?> GetByNifAsync(string nif)
    {
        if (string.IsNullOrWhiteSpace(nif))
            throw new ArgumentException("NIF cannot be null or empty", nameof(nif));

        return await _dbSet
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Nif == nif);
    }

    public async Task<IEnumerable<Client>> GetActiveClientsAsync()
    {
        return await _dbSet
            .Include(c => c.User)
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Client>> GetClientsByBudgetRangeAsync(decimal minBudget, decimal maxBudget)
    {
        return await _dbSet
            .Include(c => c.User)
            .Where(c => c.MinBudget.HasValue && c.MaxBudget.HasValue &&
                       c.MinBudget.Value >= minBudget &&
                       c.MaxBudget.Value <= maxBudget)
            .ToListAsync();
    }

    public override async Task<Client> UpdateAsync(Client client)
    {
        if (client == null)
            throw new ArgumentNullException(nameof(client));

        _dbSet.Update(client);
        return client;
    }

    public async Task<Client?> GetByIdWithFavoritesAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty", nameof(id));

        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.FavoriteProperties)
                .ThenInclude(p => p.Address)
            .Include(c => c.FavoriteProperties)
                .ThenInclude(p => p.Images)
            .Include(c => c.FavoriteProperties)
                .ThenInclude(p => p.RealEstateAgent)
                    .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
