using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class BookingRequestItem
{
    [Key]
    public Guid Id { get; set; }

    public Guid BookingRequestId { get; set; }

    [StringLength(50)]
    public string ServiceType { get; set; } = null!;

    [StringLength(50)]
    public string? Platform { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ExpectedUnitPrice { get; set; }

    public string? Notes { get; set; }

    [ForeignKey("BookingRequestId")]
    [InverseProperty("BookingRequestItems")]
    public virtual BookingRequest BookingRequest { get; set; } = null!;
}
