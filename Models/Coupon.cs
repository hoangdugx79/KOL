using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("Code", Name = "UQ__Coupons__A25C5AA72DE4FBCD", IsUnique = true)]
public partial class Coupon
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? DiscountPercent { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DiscountAmount { get; set; }

    public int? MaxUses { get; set; }

    public int UsesCount { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Coupon")]
    public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
}
