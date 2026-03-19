using KOL_KOC_TAAA.Models;

namespace KOL_KOC_TAAA.Services;

public interface INotificationService
{
    Task<List<Notification>> GetUserNotificationsAsync(Guid userId, int limit = 20);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<bool> MarkAsReadAsync(Guid notificationId);
    Task<bool> MarkAllAsReadAsync(Guid userId);
    
    Task<Notification> SendNotificationAsync(Guid userId, string type, string title, string body, string? actionUrl = null);
}
