using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("ProviderRefundId", Name = "UQ__Refunds__620B0A2F1BEA6DB2", IsUnique = true)]
public partial class Refund
{
    [Key]
    public Guid Id { get; set; }

    public Guid PaymentId { get; set; }

    [StringLength(255)]
    public string ProviderRefundId { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    public string? Reason { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("PaymentId")]
    [InverseProperty("Refunds")]
    public virtual Payment Payment { get; set; } = null!;
}
