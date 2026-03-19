using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("BookingRequestId", Name = "UQ__Bookings__68EDFCC62A47CD02", IsUnique = true)]
public partial class Booking
{
    [Key]
    public Guid Id { get; set; }

    public Guid BookingRequestId { get; set; }

    public Guid CustomerUserId { get; set; }

    public Guid KolUserId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal AgreedSubtotal { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PlatformFee { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    [StringLength(30)]
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    [ForeignKey("BookingRequestId")]
    [InverseProperty("Booking")]
    public virtual BookingRequest BookingRequest { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    [InverseProperty("Booking")]
    public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();

    [ForeignKey("CustomerUserId")]
    [InverseProperty("Bookings")]
    public virtual User CustomerUser { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<Deliverable> Deliverables { get; set; } = new List<Deliverable>();

    [InverseProperty("Booking")]
    public virtual ICollection<Dispute> Disputes { get; set; } = new List<Dispute>();

    [InverseProperty("Booking")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("KolUserId")]
    [InverseProperty("Bookings")]
    public virtual KolProfile KolUser { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
}
