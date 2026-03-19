using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class KolPortfolio
{
    [Key]
    public Guid Id { get; set; }

    public Guid KolUserId { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? MediaUrl { get; set; }

    [StringLength(50)]
    public string? ContentType { get; set; } // Image, Article, Announcement

    public DateTime CreatedAt { get; set; }

    [ForeignKey("KolUserId")]
    [InverseProperty("KolPortfolios")]
    public virtual KolProfile KolUser { get; set; } = null!;
}
