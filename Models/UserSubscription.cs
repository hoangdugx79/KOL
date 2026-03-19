using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class UserSubscription
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PlanId { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public DateTime CurrentPeriodStart { get; set; }

    public DateTime CurrentPeriodEnd { get; set; }

    public bool CancelAtPeriodEnd { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("PlanId")]
    [InverseProperty("UserSubscriptions")]
    public virtual SubscriptionPlan Plan { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserSubscriptions")]
    public virtual User User { get; set; } = null!;
}
