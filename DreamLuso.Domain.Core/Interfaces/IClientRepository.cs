using DreamLuso.Domain.Model;

namespace DreamLuso.Domain.Core.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    Task<Client> SaveAsync(Client client);
    Task<Client?> GetByUserIdAsync(Guid userId);
    Task<Client?> GetByNifAsync(string nif);
    Task<IEnumerable<Client>> GetActiveClientsAsync();
    Task<IEnumerable<Client>> GetClientsByBudgetRangeAsync(decimal minBudget, decimal maxBudget);
    Task<Client> UpdateAsync(Client client);
    Task<Client?> GetByIdWithFavoritesAsync(Guid id);
}
