using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class CouponUsage
{
    [Key]
    public Guid Id { get; set; }

    public Guid CouponId { get; set; }

    public Guid UserId { get; set; }

    public Guid? BookingId { get; set; }

    public DateTime UsedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("CouponUsages")]
    public virtual Booking? Booking { get; set; }

    [ForeignKey("CouponId")]
    [InverseProperty("CouponUsages")]
    public virtual Coupon Coupon { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("CouponUsages")]
    public virtual User User { get; set; } = null!;
}
