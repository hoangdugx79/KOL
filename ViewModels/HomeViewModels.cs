using System.Collections.Generic;

namespace KOL_KOC_TAAA.ViewModels;

public class HomeIndexViewModel
{
    public List<PublicKolProfileViewModel> FeaturedKols { get; set; } = new();
    public List<PublicKolProfileViewModel> TrendingKols { get; set; } = new();
    public List<PublicKolProfileViewModel> RecommendedKols { get; set; } = new();
    
    public PlatformStatsViewModel Stats { get; set; } = new();
    public List<BrandLogoViewModel> PartnerBrands { get; set; } = new();
    public List<CaseStudyViewModel> CaseStudies { get; set; } = new();
    public List<TestimonialViewModel> Testimonials { get; set; } = new();
}

public class PlatformStatsViewModel
{
    public int ActiveKols { get; set; }
    public int CompletedCampaigns { get; set; }
    public decimal TotalRevenue { get; set; }
    public string TotalReach { get; set; } = "0";
}

public class BrandLogoViewModel
{
    public string Name { get; set; } = null!;
    public string LogoUrl { get; set; } = null!;
}

public class CaseStudyViewModel
{
    public string BrandName { get; set; } = null!;
    public string CampaignGoal { get; set; } = null!;
    public string ResultHighlight { get; set; } = null!; // e.g., "+230% Reach"
    public string Description { get; set; } = null!;
    public string? ImageUrl { get; set; }
}

public class TestimonialViewModel
{
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!; // e.g., "Marketing Manager @ BrandA"
    public string Content { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public int Rating { get; set; } = 5;
}
