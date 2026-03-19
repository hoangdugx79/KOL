using System.Security.Claims;
using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly KolMarketplaceContext _db;

    public ChatHub(KolMarketplaceContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Client gọi khi mở trang chat để join vào group của conversation
    /// </summary>
    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    /// <summary>
    /// Client gọi để gửi tin nhắn — lưu DB rồi broadcast tới tất cả member trong group
    /// </summary>
    public async Task SendMessage(string conversationId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return;

        var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var senderId)) return;
        if (!Guid.TryParse(conversationId, out var convId)) return;

        // Verify sender is a member
        var isMember = await _db.ChatMembers
            .AnyAsync(m => m.ConversationId == convId && m.UserId == senderId);
        if (!isMember) return;

        // Persist message
        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = convId,
            SenderUserId = senderId,
            Content = content.Trim(),
            MessageType = "text",
            SentAt = DateTime.UtcNow
        };
        _db.ChatMessages.Add(message);

        var conversation = await _db.ChatConversations.FindAsync(convId);
        if (conversation != null) conversation.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        // Get sender display name
        var sender = await _db.Users.FindAsync(senderId);
        var senderName = sender?.FullName ?? "Unknown";

        // Broadcast to all members in this conversation group (including sender)
        await Clients.Group(conversationId).SendAsync("ReceiveMessage", new
        {
            id = message.Id,
            senderId = senderId,
            senderName,
            content = message.Content,
            sentAt = message.SentAt.ToString("HH:mm")
        });
    }
}
