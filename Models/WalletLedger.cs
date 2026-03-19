using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Table("WalletLedger")]
public partial class WalletLedger
{
    [Key]
    public Guid Id { get; set; }

    public Guid WalletUserId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string TransactionType { get; set; } = null!;

    public Guid? ReferenceId { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("WalletUserId")]
    [InverseProperty("WalletLedgers")]
    public virtual UserWallet WalletUser { get; set; } = null!;
}
