using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("Email", Name = "UQ__Users__A9D105340CB4E8BC", IsUnique = true)]
public partial class User
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(256)]
    public string Email { get; set; } = null!;

    [StringLength(30)]
    public string? Phone { get; set; }

    public string PasswordHash { get; set; } = null!;

    [StringLength(255)]
    public string FullName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [InverseProperty("AdminUser")]
    public virtual ICollection<AdminAuditLog> AdminAuditLogs { get; set; } = new List<AdminAuditLog>();

    [InverseProperty("CustomerUser")]
    public virtual ICollection<BookingRequest> BookingRequests { get; set; } = new List<BookingRequest>();

    [InverseProperty("CustomerUser")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("User")]
    public virtual ICollection<ChatMember> ChatMembers { get; set; } = new List<ChatMember>();

    [InverseProperty("SenderUser")]
    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    [InverseProperty("User")]
    public virtual ICollection<ContractSignature> ContractSignatures { get; set; } = new List<ContractSignature>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    [InverseProperty("User")]
    public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();

    [InverseProperty("RaisedByUser")]
    public virtual ICollection<Dispute> DisputeRaisedByUsers { get; set; } = new List<Dispute>();

    [InverseProperty("ResolvedByAdmin")]
    public virtual ICollection<Dispute> DisputeResolvedByAdmins { get; set; } = new List<Dispute>();

    [InverseProperty("UploaderUser")]
    public virtual ICollection<File> Files { get; set; } = new List<File>();

    [InverseProperty("User")]
    public virtual KolProfile? KolProfile { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<MeetingParticipant> MeetingParticipants { get; set; } = new List<MeetingParticipant>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();

    [InverseProperty("User")]
    public virtual ICollection<PayoutRequest> PayoutRequests { get; set; } = new List<PayoutRequest>();

    [InverseProperty("ReporterUser")]
    public virtual ICollection<Report> ReportReporterUsers { get; set; } = new List<Report>();

    [InverseProperty("TargetUser")]
    public virtual ICollection<Report> ReportTargetUsers { get; set; } = new List<Report>();

    [InverseProperty("User")]
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    [InverseProperty("User")]
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();

    [InverseProperty("User")]
    public virtual UserWallet? UserWallet { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
