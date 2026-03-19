using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class ChatConversation
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(20)]
    public string Type { get; set; } = null!;

    public Guid? BookingRequestId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("BookingRequestId")]
    [InverseProperty("ChatConversations")]
    public virtual BookingRequest? BookingRequest { get; set; }

    [InverseProperty("Conversation")]
    public virtual ICollection<ChatMember> ChatMembers { get; set; } = new List<ChatMember>();

    [InverseProperty("Conversation")]
    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}
