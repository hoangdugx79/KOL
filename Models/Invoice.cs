using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("BookingId", Name = "UQ__Invoices__73951AEC45E5E1E3", IsUnique = true)]
[Index("InvoiceNo", Name = "UQ__Invoices__D796B22765A66D74", IsUnique = true)]
public partial class Invoice
{
    [Key]
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    [StringLength(50)]
    public string InvoiceNo { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Fee { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Tax { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Total { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public DateTime IssuedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Invoice")]
    public virtual Booking Booking { get; set; } = null!;

    [InverseProperty("Invoice")]
    public virtual ICollection<PaymentIntent> PaymentIntents { get; set; } = new List<PaymentIntent>();
}
