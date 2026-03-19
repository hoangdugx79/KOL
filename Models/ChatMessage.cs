using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class ChatMessage
{
    [Key]
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid SenderUserId { get; set; }

    [StringLength(20)]
    public string MessageType { get; set; } = null!;

    public string? Content { get; set; }

    public Guid? AttachmentId { get; set; }

    public DateTime SentAt { get; set; }

    [ForeignKey("AttachmentId")]
    [InverseProperty("ChatMessages")]
    public virtual File? Attachment { get; set; }

    [ForeignKey("ConversationId")]
    [InverseProperty("ChatMessages")]
    public virtual ChatConversation Conversation { get; set; } = null!;

    [InverseProperty("Message")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [ForeignKey("SenderUserId")]
    [InverseProperty("ChatMessages")]
    public virtual User SenderUser { get; set; } = null!;
}
