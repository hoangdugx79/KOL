using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("BookingId", "Version", Name = "UQ_Contract_Booking_Version", IsUnique = true)]
public partial class Contract
{
    [Key]
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public int Version { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string TermsText { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Contracts")]
    public virtual Booking Booking { get; set; } = null!;

    [InverseProperty("Contract")]
    public virtual ICollection<ContractSignature> ContractSignatures { get; set; } = new List<ContractSignature>();

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("Contracts")]
    public virtual User CreatedByUser { get; set; } = null!;
}
