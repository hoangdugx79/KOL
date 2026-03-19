using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Services;

public class NotificationService : INotificationService
{
    private readonly KolMarketplaceContext _context;

    public NotificationService(KolMarketplaceContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId, int limit = 20)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && n.ReadAt == null);
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null) return false;

        notification.ReadAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(Guid userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && n.ReadAt == null)
            .ToListAsync();

        foreach (var n in unread) n.ReadAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Notification> SendNotificationAsync(Guid userId, string type, string title, string body, string? actionUrl = null)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Title = title,
            Body = body,
            CreatedAt = DateTime.UtcNow
        };

        // Note: Notification model doesn't have ActionUrl in schema, 
        // we can store it in Body or just ignore if not critical now.
        // Assuming we keep it simple with existing schema.

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }
}
