using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("Platform", "Username", Name = "UQ_Social_Platform_Username", IsUnique = true)]
public partial class KolSocialAccount
{
    [Key]
    public Guid Id { get; set; }

    public Guid KolUserId { get; set; }

    [StringLength(50)]
    public string Platform { get; set; } = null!;

    [StringLength(100)]
    public string? Username { get; set; }

    public string? ProfileUrl { get; set; }

    public long? Followers { get; set; }

    public long? AvgViews { get; set; }

    [Column(TypeName = "decimal(6, 3)")]
    public decimal? EngagementRate { get; set; }

    public bool IsVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("KolUserId")]
    [InverseProperty("KolSocialAccounts")]
    public virtual KolProfile KolUser { get; set; } = null!;
}
