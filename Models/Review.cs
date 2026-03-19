using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOL_KOC_TAAA.Models;

public partial class Review
{
    [Key]
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid CustomerUserId { get; set; }

    public Guid KolUserId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("BookingId")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("CustomerUserId")]
    public virtual User CustomerUser { get; set; } = null!;

    [ForeignKey("KolUserId")]
    public virtual KolProfile KolProfile { get; set; } = null!;
}
