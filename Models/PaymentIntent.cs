using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("ProviderIntentId", Name = "UQ__PaymentI__84849D6F2BE3F25A", IsUnique = true)]
public partial class PaymentIntent
{
    [Key]
    public Guid Id { get; set; }

    public Guid InvoiceId { get; set; }

    [StringLength(30)]
    public string Provider { get; set; } = null!;

    [StringLength(255)]
    public string ProviderIntentId { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    [StringLength(30)]
    public string MethodType { get; set; } = null!;

    [StringLength(30)]
    public string Status { get; set; } = null!;

    public string? ReturnUrl { get; set; }

    public string? QrPayload { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("PaymentIntents")]
    public virtual Invoice Invoice { get; set; } = null!;

    [InverseProperty("PaymentIntent")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
