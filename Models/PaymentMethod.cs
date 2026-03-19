using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("Provider", "ProviderPaymentMethodId", Name = "UQ_PaymentMethod_ProviderId", IsUnique = true)]
public partial class PaymentMethod
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [StringLength(30)]
    public string Provider { get; set; } = null!;

    [StringLength(255)]
    public string ProviderPaymentMethodId { get; set; } = null!;

    [StringLength(30)]
    public string Type { get; set; } = null!;

    [StringLength(30)]
    public string? CardBrand { get; set; }

    [StringLength(4)]
    public string? CardLast4 { get; set; }

    public int? CardExpMonth { get; set; }

    public int? CardExpYear { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("PaymentMethods")]
    public virtual User User { get; set; } = null!;
}
