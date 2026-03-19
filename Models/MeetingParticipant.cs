using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[PrimaryKey("MeetingId", "UserId")]
public partial class MeetingParticipant
{
    [Key]
    public Guid MeetingId { get; set; }

    [Key]
    public Guid UserId { get; set; }

    [StringLength(20)]
    public string Role { get; set; } = null!;

    [StringLength(20)]
    public string AttendanceStatus { get; set; } = null!;

    [ForeignKey("MeetingId")]
    [InverseProperty("MeetingParticipants")]
    public virtual Meeting Meeting { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("MeetingParticipants")]
    public virtual User User { get; set; } = null!;
}
