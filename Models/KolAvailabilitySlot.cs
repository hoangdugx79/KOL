using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class KolAvailabilitySlot
{
    [Key]
    public Guid Id { get; set; }

    public Guid KolUserId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("KolUserId")]
    [InverseProperty("KolAvailabilitySlots")]
    public virtual KolProfile KolUser { get; set; } = null!;
}
