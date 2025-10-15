using DreamLuso.Data.Context;
using DreamLuso.Domain.Core.Interfaces;
using DreamLuso.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamLuso.Data.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Notification> SaveAsync(Notification notification)
    {
        if (notification == null)
            throw new ArgumentNullException(nameof(notification));

        await _dbSet.AddAsync(notification);
        return notification;
    }

    public async Task<IEnumerable<Notification>> GetByRecipientAsync(Guid recipientId)
    {
        return await _dbSet
            .Include(n => n.Sender)
            .Include(n => n.Recipient)
            .Where(n => n.RecipientId == recipientId && n.Status != NotificationStatus.Deleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetUnreadByRecipientAsync(Guid recipientId)
    {
        return await _dbSet
            .Include(n => n.Sender)
            .Where(n => n.RecipientId == recipientId && n.Status == NotificationStatus.Unread)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid recipientId)
    {
        return await _dbSet
            .CountAsync(n => n.RecipientId == recipientId && n.Status == NotificationStatus.Unread);
    }

    public async Task<IEnumerable<Notification>> GetByTypeAsync(Guid recipientId, NotificationType type)
    {
        return await _dbSet
            .Include(n => n.Sender)
            .Where(n => n.RecipientId == recipientId && n.Type == type && n.Status != NotificationStatus.Deleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _dbSet.FindAsync(notificationId);
        if (notification != null)
        {
            notification.MarkAsRead();
        }
    }

    public async Task MarkAllAsReadAsync(Guid recipientId)
    {
        var unreadNotifications = await _dbSet
            .Where(n => n.RecipientId == recipientId && n.Status == NotificationStatus.Unread)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }
    }

    public async Task DeleteExpiredAsync()
    {
        var expiredNotifications = await _dbSet
            .Where(n => n.IsTransient && n.ExpirationDate < DateTime.UtcNow)
            .ToListAsync();

        _dbSet.RemoveRange(expiredNotifications);
    }
}

