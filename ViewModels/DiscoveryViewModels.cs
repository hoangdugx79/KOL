using System.Collections.Generic;
using KOL_KOC_TAAA.Models;

namespace KOL_KOC_TAAA.ViewModels;

public class DiscoveryViewModel
{
    // Search & Filter state
    public DiscoveryFilterViewModel Filters { get; set; } = new();
    
    // Results
    public List<PublicKolProfileViewModel> Kols { get; set; } = new();
    
    // Layout sections
    public List<PublicKolProfileViewModel> TrendingKols { get; set; } = new();
    public List<PublicKolProfileViewModel> NewRisingKols { get; set; } = new();
    
    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int TotalCount { get; set; }
}

public class DiscoveryFilterViewModel
{
    public string? SearchQuery { get; set; }
    public string? Category { get; set; }
    public string? Platform { get; set; }
    public string? Location { get; set; }
    public decimal? MinBudget { get; set; }
    public decimal? MaxBudget { get; set; }
    public long? MinFollowers { get; set; }
    public bool? IsVerified { get; set; }
    public decimal? MinRating { get; set; }
    public string? SortBy { get; set; } // featured, relevant, newest, price_low, followers_high
}
