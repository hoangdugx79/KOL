using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("ProviderChargeId", Name = "UQ__Payments__7E985B0A4608B23B", IsUnique = true)]
public partial class Payment
{
    [Key]
    public Guid Id { get; set; }

    public Guid PaymentIntentId { get; set; }

    [StringLength(255)]
    public string? ProviderChargeId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PaidAmount { get; set; }

    public DateTime? PaidAt { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [StringLength(100)]
    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("PaymentIntentId")]
    [InverseProperty("Payments")]
    public virtual PaymentIntent PaymentIntent { get; set; } = null!;

    [InverseProperty("Payment")]
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
