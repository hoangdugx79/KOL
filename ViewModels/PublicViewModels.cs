using KOL_KOC_TAAA.Models;

namespace KOL_KOC_TAAA.ViewModels;

public class PublicKolProfileViewModel
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string InfluencerType { get; set; } = null!;
    public string? Bio { get; set; }
    public string? LocationCity { get; set; }
    public string? LocationCountry { get; set; }
    public decimal RatingAvg { get; set; }
    public int RatingCount { get; set; }
    public bool IsVerified { get; set; }
    public decimal? MinBudget { get; set; }

    // Trust Signals
    public string? AvgResponseTime { get; set; } // e.g., "Trong 2 giờ"
    public int CompletedCampaigns { get; set; }
    public decimal CompletionRate { get; set; } // e.g., 98.5

    // Social stats for card display
    public string? TopPlatform { get; set; }
    public long? TotalFollowers { get; set; }
    public List<string> Platforms { get; set; } = new();

    // Category / lĩnh vực (BeautyFashion, FoodBeverage, Tech, SportsFitness, Travel, Gaming)
    public string? Category { get; set; }

    // Thông tin liên lạc
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public List<KolSocialAccount> SocialAccounts { get; set; } = new List<KolSocialAccount>();
    public List<KolPortfolio> Portfolios { get; set; } = new List<KolPortfolio>();
    public List<RateCard> RateCards { get; set; } = new List<RateCard>();
}
