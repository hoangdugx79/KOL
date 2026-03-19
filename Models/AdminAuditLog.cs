using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class AdminAuditLog
{
    [Key]
    public Guid Id { get; set; }

    public Guid AdminUserId { get; set; }

    [StringLength(100)]
    public string Action { get; set; } = null!;

    [StringLength(80)]
    public string? TargetTable { get; set; }

    public Guid? TargetId { get; set; }

    public string? MetadataJson { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("AdminUserId")]
    [InverseProperty("AdminAuditLogs")]
    public virtual User AdminUser { get; set; } = null!;
}
