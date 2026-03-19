using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class PayoutRequest
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    public string? BankInfoJson { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public DateTime? ProcessedAt { get; set; }

    public string? AdminNote { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("PayoutRequests")]
    public virtual User User { get; set; } = null!;
}
