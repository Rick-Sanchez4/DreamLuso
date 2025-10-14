using DreamLuso.Domain.Model;

namespace DreamLuso.Domain.Core.Interfaces;

public interface IPropertyVisitRepository : IRepository<PropertyVisit>
{
    Task<PropertyVisit> SaveAsync(PropertyVisit visit);
    Task<IEnumerable<PropertyVisit>> GetByPropertyIdAsync(Guid propertyId);
    Task<IEnumerable<PropertyVisit>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<PropertyVisit>> GetByAgentIdAsync(Guid agentId);
    Task<IEnumerable<PropertyVisit>> GetByStatusAsync(VisitStatus status);
    Task<IEnumerable<PropertyVisit>> GetUpcomingVisitsAsync(DateOnly fromDate, DateOnly toDate);
    Task<PropertyVisit?> GetByConfirmationTokenAsync(string token);
    Task<bool> IsTimeSlotAvailableAsync(Guid propertyId, DateOnly date, TimeSlot timeSlot);
    Task<PropertyVisit> UpdateAsync(PropertyVisit visit);
}
