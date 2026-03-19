using System.ComponentModel.DataAnnotations;

namespace KOL_KOC_TAAA.ViewModels;

public class ConversationViewModel
{
    public Guid Id { get; set; }
    public string OtherUserName { get; set; } = null!;
    public string LastMessage { get; set; } = null!;
    public DateTime LastMessageTime { get; set; }
    public string ConversationType { get; set; } = null!; // E.g., "booking"
}

public class ChatDetailsViewModel
{
    public Guid ConversationId { get; set; }
    public string OtherUserName { get; set; } = null!;
    public Guid? BookingRequestId { get; set; }
    public List<MessageViewModel> Messages { get; set; } = new();
    
    [Required]
    public string NewMessage { get; set; } = null!;
}

public class MessageViewModel
{
    public Guid Id { get; set; }
    public string SenderName { get; set; } = null!;
    public bool IsMe { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public string MessageType { get; set; } = "text";
}
