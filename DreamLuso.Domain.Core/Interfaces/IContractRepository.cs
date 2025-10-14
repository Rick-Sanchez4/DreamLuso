using DreamLuso.Domain.Model;

namespace DreamLuso.Domain.Core.Interfaces;

public interface IContractRepository : IRepository<Contract>
{
    Task<Contract> SaveAsync(Contract contract);
    Task<IEnumerable<Contract>> GetByPropertyIdAsync(Guid propertyId);
    Task<IEnumerable<Contract>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<Contract>> GetByAgentIdAsync(Guid agentId);
    Task<IEnumerable<Contract>> GetByStatusAsync(ContractStatus status);
    Task<IEnumerable<Contract>> GetActiveContractsAsync();
    Task<IEnumerable<Contract>> GetExpiringContractsAsync(int daysUntilExpiry);
    Task<Contract?> GetByIdWithDetailsAsync(Guid id);
    Task<Contract> UpdateAsync(Contract contract);
}
