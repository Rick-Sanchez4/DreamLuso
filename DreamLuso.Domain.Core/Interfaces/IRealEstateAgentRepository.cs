using DreamLuso.Domain.Model;

namespace DreamLuso.Domain.Core.Interfaces;

public interface IRealEstateAgentRepository : IRepository<RealEstateAgent>
{
    Task<RealEstateAgent> SaveAsync(RealEstateAgent agent);
    Task<RealEstateAgent?> GetByUserIdAsync(Guid userId);
    Task<RealEstateAgent?> GetByLicenseNumberAsync(string licenseNumber);
    Task<IEnumerable<RealEstateAgent>> GetActiveAgentsAsync();
    Task<IEnumerable<RealEstateAgent>> GetTopAgentsByRevenueAsync(int top = 10);
    Task<IEnumerable<RealEstateAgent>> GetAgentsBySpecializationAsync(string specialization);
    Task<RealEstateAgent> UpdateAsync(RealEstateAgent agent);
}
