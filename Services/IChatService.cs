using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;

namespace KOL_KOC_TAAA.Services;

public interface IChatService
{
    Task<List<ChatConversation>> GetUserConversationsAsync(Guid userId);
    Task<ChatConversation?> GetConversationAsync(Guid conversationId, Guid userId);
    Task<List<ChatMessage>> GetMessageHistoryAsync(Guid conversationId, int limit = 50);
    
    Task<ChatMessage> SendMessageAsync(Guid conversationId, Guid senderId, string content, string type = "text");
    
    Task<ChatConversation> GetOrCreateDirectConversationAsync(Guid userA, Guid userB);
    Task<ChatConversation> GetOrCreateBookingConversationAsync(Guid bookingRequestId, Guid kolUserId, Guid customerUserId, string title);
    Task<ChatConversation?> GetConversationByBookingRequestAsync(Guid requestId);
}
