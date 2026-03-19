using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;

namespace KOL_KOC_TAAA.Controllers;

[ApiController]
[Route("api/kolkoc")]
public class KolKocController : ControllerBase
{
    private readonly KolMarketplaceContext _context;

    public KolKocController(KolMarketplaceContext context)
    {
        _context = context;
    }

    // GET /api/kolkoc
    // Supports filter: platform, type, keyword
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? platform,
        [FromQuery] string? type,
        [FromQuery] string? keyword)
    {
        var query = _context.KolProfiles
            .Include(p => p.User)
            .Include(p => p.KolSocialAccounts)
            .Include(p => p.Categories)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(p => p.InfluencerType == type);

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(p =>
                p.User.FullName.Contains(keyword) ||
                (p.Bio != null && p.Bio.Contains(keyword)));

        if (!string.IsNullOrWhiteSpace(platform))
            query = query.Where(p =>
                p.KolSocialAccounts.Any(s => s.Platform == platform));

        var profiles = await query.ToListAsync();

        var result = profiles.Select(p => MapProfile(p));
        return Ok(result);
    }

    // GET /api/kolkoc/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var profile = await _context.KolProfiles
            .Include(p => p.User)
            .Include(p => p.KolSocialAccounts)
            .Include(p => p.Categories)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.UserId == id);

        if (profile == null)
            return NotFound(new { message = $"KOL with id {id} not found." });

        return Ok(MapProfile(profile));
    }

    // GET /api/kolkoc/platforms
    [HttpGet("platforms")]
    public async Task<IActionResult> GetPlatforms()
    {
        var platforms = await _context.KolSocialAccounts
            .Select(s => s.Platform)
            .Distinct()
            .OrderBy(p => p)
            .ToListAsync();

        return Ok(platforms);
    }

    private static object MapProfile(KolProfile p)
    {
        return new
        {
            id = p.UserId,
            fullName = p.User?.FullName,
            avatarUrl = p.User?.AvatarUrl,
            influencerType = p.InfluencerType,
            bio = p.Bio,
            gender = p.Gender,
            locationCity = p.LocationCity,
            locationCountry = p.LocationCountry,
            minBudget = p.MinBudget,
            ratingAvg = p.RatingAvg,
            ratingCount = p.RatingCount,
            isVerified = p.IsVerified,
            categories = p.Categories.Select(c => c.Name).ToList(),
            platformAccounts = p.KolSocialAccounts.Select(s => new
            {
                platform = s.Platform,
                username = s.Username,
                profileUrl = s.ProfileUrl,
                followers = s.Followers,
                avgViews = s.AvgViews,
                engagementRate = s.EngagementRate,
                isVerified = s.IsVerified
            }).ToList()
        };
    }
}
