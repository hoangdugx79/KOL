using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Services;

public class DiscoveryService : IDiscoveryService
{
    private readonly KolMarketplaceContext _context;

    public DiscoveryService(KolMarketplaceContext context)
    {
        _context = context;
    }

    public async Task<DiscoveryViewModel> GetDiscoveryResultsAsync(DiscoveryFilterViewModel filters, int page = 1, int pageSize = 12)
    {
        var query = _context.ViewKolSearches.AsQueryable();

        // 1. Filtering
        if (!string.IsNullOrEmpty(filters.SearchQuery))
        {
            query = query.Where(k => k.FullName.Contains(filters.SearchQuery) || 
                                     (k.CategoriesText != null && k.CategoriesText.Contains(filters.SearchQuery)) ||
                                     (k.TagsText != null && k.TagsText.Contains(filters.SearchQuery)));
        }

        if (!string.IsNullOrEmpty(filters.Category))
        {
            query = query.Where(k => k.InfluencerType == filters.Category);
        }

        if (!string.IsNullOrEmpty(filters.Platform))
        {
            query = query.Where(k => k.PlatformsText != null && k.PlatformsText.Contains(filters.Platform));
        }

        if (!string.IsNullOrEmpty(filters.Location))
        {
            query = query.Where(k => k.LocationCity == filters.Location);
        }

        if (filters.MinBudget.HasValue)
        {
            query = query.Where(k => k.MinBudget >= filters.MinBudget.Value);
        }

        if (filters.MaxBudget.HasValue)
        {
            query = query.Where(k => k.MinBudget <= filters.MaxBudget.Value);
        }

        if (filters.MinFollowers.HasValue)
        {
            query = query.Where(k => (k.MaxFollowers ?? 0) >= filters.MinFollowers.Value);
        }

        // Note: View_KolSearch already only contains verified KOLs (IsVerified = 1)
        // The IsVerified filter is preserved for UI display consistency

        // 2. Sorting
        query = filters.SortBy switch
        {
            "newest" => query.OrderByDescending(k => k.UserId), 
            "price_low" => query.OrderBy(k => k.MinBudget),
            "followers_high" => query.OrderByDescending(k => k.MaxFollowers),
            "rating" => query.OrderByDescending(k => k.RatingAvg),
            _ => query.OrderByDescending(k => k.RatingAvg).ThenByDescending(k => k.MaxFollowers) 
        };

        // 3. Pagination & Execution
        var totalCount = await query.CountAsync();
        var pagedResults = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var kols = pagedResults.Select(k => MapToViewModel(k)).ToList();

        return new DiscoveryViewModel
        {
            Filters = filters,
            Kols = kols,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<List<PublicKolProfileViewModel>> GetTrendingKolsAsync(int count = 10)
    {
        var trending = await _context.ViewKolSearches
            .OrderByDescending(k => k.RatingAvg)
            .ThenByDescending(k => k.RatingCount)
            .Take(count)
            .ToListAsync();

        return trending.Select(MapToViewModel).ToList();
    }

    public async Task<List<PublicKolProfileViewModel>> GetNewRisingKolsAsync(int count = 10)
    {
        // For rising, we might look at recent ratings or simply newest profiles
        var rising = await _context.ViewKolSearches
            .OrderByDescending(k => k.UserId) // Guid placeholder for newest
            .Take(count)
            .ToListAsync();

        return rising.Select(MapToViewModel).ToList();
    }

    private PublicKolProfileViewModel MapToViewModel(ViewKolSearch k)
    {
        return new PublicKolProfileViewModel
        {
            UserId = k.UserId,
            FullName = k.FullName,
            AvatarUrl = k.AvatarUrl,
            InfluencerType = k.InfluencerType,
            LocationCity = k.LocationCity,
            MinBudget = k.MinBudget,
            RatingAvg = k.RatingAvg,
            RatingCount = k.RatingCount,
            TotalFollowers = k.MaxFollowers ?? 0,
            Category = k.CategoriesText?.Split(',').FirstOrDefault() ?? "General",
            Platforms = k.PlatformsText?.Split(',').ToList() ?? new List<string>()
        };
    }
}
