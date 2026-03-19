using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[PrimaryKey("ConversationId", "UserId")]
public partial class ChatMember
{
    [Key]
    public Guid ConversationId { get; set; }

    [Key]
    public Guid UserId { get; set; }

    [StringLength(20)]
    public string? RoleInChat { get; set; }

    public DateTime? LastReadAt { get; set; }

    [ForeignKey("ConversationId")]
    [InverseProperty("ChatMembers")]
    public virtual ChatConversation Conversation { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ChatMembers")]
    public virtual User User { get; set; } = null!;
}
