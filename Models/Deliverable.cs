using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class Deliverable
{
    [Key]
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid? ItemId { get; set; }

    [StringLength(50)]
    public string DeliverableType { get; set; } = null!;

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? DueAt { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Deliverables")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("Deliverables")]
    public virtual BookingItem? Item { get; set; }

    [ForeignKey("DeliverableId")]
    [InverseProperty("Deliverables")]
    public virtual ICollection<File> Files { get; set; } = new List<File>();
}
