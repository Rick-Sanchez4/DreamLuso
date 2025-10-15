using DreamLuso.Domain.Model;

namespace DreamLuso.Domain.Core.Interfaces;

public interface INotificationRepository : IRepository<Notification>
{
    Task<Notification> SaveAsync(Notification notification);
    Task<IEnumerable<Notification>> GetByRecipientAsync(Guid recipientId);
    Task<IEnumerable<Notification>> GetUnreadByRecipientAsync(Guid recipientId);
    Task<int> GetUnreadCountAsync(Guid recipientId);
    Task<IEnumerable<Notification>> GetByTypeAsync(Guid recipientId, NotificationType type);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(Guid recipientId);
    Task DeleteExpiredAsync();
}

