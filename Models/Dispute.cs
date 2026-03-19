using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class Dispute
{
    [Key]
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid RaisedByUserId { get; set; }

    [StringLength(30)]
    public string DisputeType { get; set; } = null!;

    public string Reason { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AmountClaimed { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public string? ResolutionNote { get; set; }

    public Guid? ResolvedByAdminId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Disputes")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("RaisedByUserId")]
    [InverseProperty("DisputeRaisedByUsers")]
    public virtual User RaisedByUser { get; set; } = null!;

    [ForeignKey("ResolvedByAdminId")]
    [InverseProperty("DisputeResolvedByAdmins")]
    public virtual User? ResolvedByAdmin { get; set; }
}
