using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Services;

public class ChatService : IChatService
{
    private readonly KolMarketplaceContext _context;

    public ChatService(KolMarketplaceContext context)
    {
        _context = context;
    }

    public async Task<List<ChatConversation>> GetUserConversationsAsync(Guid userId)
    {
        return await _context.ChatMembers
            .Where(m => m.UserId == userId)
            .Include(m => m.Conversation)
                .ThenInclude(c => c.ChatMembers)
                    .ThenInclude(cm => cm.User)
            .Include(m => m.Conversation)
                .ThenInclude(c => c.ChatMessages.OrderByDescending(msg => msg.SentAt).Take(1))
            .Select(m => m.Conversation)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();
    }

    public async Task<ChatConversation?> GetConversationAsync(Guid conversationId, Guid userId)
    {
        // Verify membership
        var member = await _context.ChatMembers
            .FirstOrDefaultAsync(m => m.ConversationId == conversationId && m.UserId == userId);
            
        if (member == null) return null;

        return await _context.ChatConversations
            .Include(c => c.ChatMembers)
                .ThenInclude(cm => cm.User)
            .Include(c => c.BookingRequest)
                .ThenInclude(r => r.BookingRequestItems)
            .Include(c => c.BookingRequest)
                .ThenInclude(r => r.CustomerUser)
            .Include(c => c.BookingRequest)
                .ThenInclude(r => r.KolUser)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
    }

    public async Task<List<ChatMessage>> GetMessageHistoryAsync(Guid conversationId, int limit = 50)
    {
        return await _context.ChatMessages
            .Where(m => m.ConversationId == conversationId)
            .Include(m => m.SenderUser)
            .OrderBy(m => m.SentAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<ChatMessage> SendMessageAsync(Guid conversationId, Guid senderId, string content, string type = "text")
    {
        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderUserId = senderId,
            Content = content,
            MessageType = type,
            SentAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);
        
        // Update conversation timestamp
        var conversation = await _context.ChatConversations.FindAsync(conversationId);
        if (conversation != null)
        {
            conversation.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<ChatConversation> GetOrCreateDirectConversationAsync(Guid userA, Guid userB)
    {
        // Check if exists
        var existing = await _context.ChatConversations
            .Where(c => c.Type == "direct")
            .Where(c => c.ChatMembers.Any(m => m.UserId == userA) && c.ChatMembers.Any(m => m.UserId == userB))
            .FirstOrDefaultAsync();

        if (existing != null) return existing;

        var conversation = new ChatConversation
        {
            Id = Guid.NewGuid(),
            Type = "direct",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ChatConversations.Add(conversation);
        _context.ChatMembers.Add(new ChatMember { ConversationId = conversation.Id, UserId = userA });
        _context.ChatMembers.Add(new ChatMember { ConversationId = conversation.Id, UserId = userB });

        await _context.SaveChangesAsync();
        return conversation;
    }

    public async Task<ChatConversation?> GetConversationByBookingRequestAsync(Guid requestId)
    {
        return await _context.ChatConversations
            .Include(c => c.BookingRequest)
            .FirstOrDefaultAsync(c => c.BookingRequestId == requestId);
    }

    public async Task<ChatConversation> GetOrCreateBookingConversationAsync(
        Guid bookingRequestId, Guid kolUserId, Guid customerUserId, string title)
    {
        // Return existing if already created for this booking
        var existing = await _context.ChatConversations
            .Include(c => c.ChatMembers)
            .FirstOrDefaultAsync(c => c.BookingRequestId == bookingRequestId);
        if (existing != null) return existing;

        var conversation = new ChatConversation
        {
            Id = Guid.NewGuid(),
            Type = "booking",
            BookingRequestId = bookingRequestId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ChatConversations.Add(conversation);
        _context.ChatMembers.Add(new ChatMember
        {
            ConversationId = conversation.Id,
            UserId = kolUserId,
            RoleInChat = "kol"
        });
        _context.ChatMembers.Add(new ChatMember
        {
            ConversationId = conversation.Id,
            UserId = customerUserId,
            RoleInChat = "customer"
        });

        // System welcome message
        _context.ChatMessages.Add(new ChatMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderUserId = kolUserId,
            Content = $"🎉 Hợp tác \"{title}\" đã được chấp nhận! Đây là kênh liên lạc riêng giữa KOL và khách hàng. Lịch sử tin nhắn sẽ được lưu trữ suốt thời gian hợp tác.",
            MessageType = "system",
            SentAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return conversation;
    }
}
