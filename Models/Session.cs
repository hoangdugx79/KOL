using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("UserId", "ExpiresAt", Name = "IDX_Sessions_User_Expires")]
public partial class Session
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public string? DeviceInfo { get; set; }

    [StringLength(50)]
    public string? IpAddress { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Sessions")]
    public virtual User User { get; set; } = null!;
}
