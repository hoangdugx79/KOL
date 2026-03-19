using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Keyless]
public partial class ViewKolSearch
{
    public Guid UserId { get; set; }

    [StringLength(255)]
    public string FullName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    [StringLength(10)]
    public string InfluencerType { get; set; } = null!;

    [StringLength(100)]
    public string? LocationCity { get; set; }

    [StringLength(100)]
    public string? LocationCountry { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? MinBudget { get; set; }

    [Column(TypeName = "decimal(3, 2)")]
    public decimal RatingAvg { get; set; }

    public int RatingCount { get; set; }

    [StringLength(4000)]
    public string? CategoriesText { get; set; }

    [StringLength(4000)]
    public string? TagsText { get; set; }

    [StringLength(4000)]
    public string? PlatformsText { get; set; }

    public long? MaxFollowers { get; set; }
}
