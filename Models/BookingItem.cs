using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class BookingItem
{
    [Key]
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    [StringLength(50)]
    public string ServiceType { get; set; } = null!;

    [StringLength(50)]
    public string? Platform { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal LineTotal { get; set; }

    public DateOnly? DeliverDueDate { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("BookingItems")]
    public virtual Booking Booking { get; set; } = null!;

    [InverseProperty("Item")]
    public virtual ICollection<Deliverable> Deliverables { get; set; } = new List<Deliverable>();
}
