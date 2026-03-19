using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class RateCard
{
    [Key]
    public Guid Id { get; set; }

    public Guid KolUserId { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Notes { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateOnly? EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("KolUserId")]
    [InverseProperty("RateCards")]
    public virtual KolProfile KolUser { get; set; } = null!;

    [InverseProperty("RateCard")]
    public virtual ICollection<RateCardHistory> RateCardHistories { get; set; } = new List<RateCardHistory>();

    [InverseProperty("RateCard")]
    public virtual ICollection<RateCardItem> RateCardItems { get; set; } = new List<RateCardItem>();
}
