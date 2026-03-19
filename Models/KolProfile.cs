using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class KolProfile
{
    [Key]
    public Guid UserId { get; set; }

    [StringLength(10)]
    public string InfluencerType { get; set; } = null!;

    public string? Bio { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    public DateOnly? Dob { get; set; }

    [StringLength(100)]
    public string? LocationCity { get; set; }

    [StringLength(100)]
    public string? LocationCountry { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? MinBudget { get; set; }

    [Column(TypeName = "decimal(3, 2)")]
    public decimal RatingAvg { get; set; }

    public int RatingCount { get; set; }

    public bool IsVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [InverseProperty("KolUser")]
    public virtual ICollection<BookingRequest> BookingRequests { get; set; } = new List<BookingRequest>();

    [InverseProperty("KolUser")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("KolUser")]
    public virtual ICollection<KolAvailabilitySlot> KolAvailabilitySlots { get; set; } = new List<KolAvailabilitySlot>();

    [InverseProperty("KolUser")]
    public virtual ICollection<KolPortfolio> KolPortfolios { get; set; } = new List<KolPortfolio>();

    [InverseProperty("KolUser")]
    public virtual ICollection<KolSocialAccount> KolSocialAccounts { get; set; } = new List<KolSocialAccount>();

    [InverseProperty("KolUser")]
    public virtual ICollection<RateCard> RateCards { get; set; } = new List<RateCard>();

    [ForeignKey("UserId")]
    [InverseProperty("KolProfile")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("KolUserId")]
    [InverseProperty("KolUsers")]
    public virtual ICollection<KolCategory> Categories { get; set; } = new List<KolCategory>();

    [ForeignKey("KolUserId")]
    [InverseProperty("KolUsers")]
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
