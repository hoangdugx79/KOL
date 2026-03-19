using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class RateCardItem
{
    [Key]
    public Guid Id { get; set; }

    public Guid RateCardId { get; set; }

    [StringLength(50)]
    public string ServiceType { get; set; } = null!;

    [StringLength(50)]
    public string? Platform { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [StringLength(30)]
    public string Unit { get; set; } = null!;

    public int? DurationMinutes { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("RateCardId")]
    [InverseProperty("RateCardItems")]
    public virtual RateCard RateCard { get; set; } = null!;
}
