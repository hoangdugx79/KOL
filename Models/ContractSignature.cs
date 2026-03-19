using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("ContractId", "UserId", Name = "UQ_Contract_Signer", IsUnique = true)]
public partial class ContractSignature
{
    [Key]
    public Guid Id { get; set; }

    public Guid ContractId { get; set; }

    public Guid UserId { get; set; }

    public DateTime SignedAt { get; set; }

    public string? SignatureData { get; set; }

    [StringLength(50)]
    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    [ForeignKey("ContractId")]
    [InverseProperty("ContractSignatures")]
    public virtual Contract Contract { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ContractSignatures")]
    public virtual User User { get; set; } = null!;
}
