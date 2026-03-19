using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class UserWallet
{
    [Key]
    public Guid UserId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal LockedBalance { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserWallet")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("WalletUser")]
    public virtual ICollection<WalletLedger> WalletLedgers { get; set; } = new List<WalletLedger>();
}
