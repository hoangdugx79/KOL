using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class BookingRequest
{
    [Key]
    public Guid Id { get; set; }

    public Guid CustomerUserId { get; set; }

    public Guid KolUserId { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Brief { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? BudgetMin { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? BudgetMax { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    public DateOnly? ProposedStartDate { get; set; }

    public DateOnly? ProposedEndDate { get; set; }

    [StringLength(30)]
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [InverseProperty("BookingRequest")]
    public virtual Booking? Booking { get; set; }

    [InverseProperty("BookingRequest")]
    public virtual ICollection<BookingRequestItem> BookingRequestItems { get; set; } = new List<BookingRequestItem>();

    [InverseProperty("BookingRequest")]
    public virtual ICollection<ChatConversation> ChatConversations { get; set; } = new List<ChatConversation>();

    [ForeignKey("CustomerUserId")]
    [InverseProperty("BookingRequests")]
    public virtual User CustomerUser { get; set; } = null!;

    [ForeignKey("KolUserId")]
    [InverseProperty("BookingRequests")]
    public virtual KolProfile KolUser { get; set; } = null!;
}
