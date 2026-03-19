using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Table("RateCardHistory")]
public partial class RateCardHistory
{
    [Key]
    public Guid Id { get; set; }

    public Guid RateCardId { get; set; }

    public string? SnapshotData { get; set; }

    public DateTime ChangedAt { get; set; }

    [ForeignKey("RateCardId")]
    [InverseProperty("RateCardHistories")]
    public virtual RateCard RateCard { get; set; } = null!;
}
