using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class Report
{
    [Key]
    public Guid Id { get; set; }

    public Guid ReporterUserId { get; set; }

    public Guid? TargetUserId { get; set; }

    public Guid? MessageId { get; set; }

    public string Reason { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public string? AdminNote { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("MessageId")]
    [InverseProperty("Reports")]
    public virtual ChatMessage? Message { get; set; }

    [ForeignKey("ReporterUserId")]
    [InverseProperty("ReportReporterUsers")]
    public virtual User ReporterUser { get; set; } = null!;

    [ForeignKey("TargetUserId")]
    [InverseProperty("ReportTargetUsers")]
    public virtual User? TargetUser { get; set; }
}
