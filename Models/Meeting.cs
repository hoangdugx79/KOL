using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class Meeting
{
    [Key]
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid CreatedByUserId { get; set; }

    [StringLength(10)]
    public string MeetingType { get; set; } = null!;

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Agenda { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? LocationText { get; set; }

    public string? MeetingUrl { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Meetings")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("Meetings")]
    public virtual User CreatedByUser { get; set; } = null!;

    [InverseProperty("Meeting")]
    public virtual ICollection<MeetingParticipant> MeetingParticipants { get; set; } = new List<MeetingParticipant>();
}
