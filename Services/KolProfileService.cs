using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Services;

public class KolProfileService : IKolProfileService
{
    private readonly KolMarketplaceContext _context;

    public KolProfileService(KolMarketplaceContext context)
    {
        _context = context;
    }

    public async Task<KolProfile?> GetProfileByUserIdAsync(Guid userId)
    {
        return await _context.KolProfiles
            .Include(p => p.User)
            .Include(p => p.KolSocialAccounts)
            .Include(p => p.KolPortfolios)
            .Include(p => p.RateCards)
                .ThenInclude(rc => rc.RateCardItems)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<bool> UpdateProfileAsync(Guid userId, KolProfileEditViewModel model)
    {
        var profile = await _context.KolProfiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null) return false;

        // Update KolProfile fields
        profile.InfluencerType = model.InfluencerType;
        profile.Bio = model.Bio;
        profile.Gender = model.Gender;
        profile.Dob = model.Dob;
        profile.LocationCity = model.LocationCity;
        profile.LocationCountry = model.LocationCountry;
        profile.MinBudget = model.MinBudget;
        profile.UpdatedAt = DateTime.UtcNow;

        // Update User fields
        if (profile.User != null)
        {
            if (!string.IsNullOrWhiteSpace(model.FullName))
                profile.User.FullName = model.FullName;
            if (!string.IsNullOrWhiteSpace(model.AvatarUrl))
                profile.User.AvatarUrl = model.AvatarUrl;
            if (!string.IsNullOrWhiteSpace(model.ContactEmail))
                profile.User.Email = model.ContactEmail;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<KolSocialAccount>> GetSocialAccountsAsync(Guid userId)
    {
        return await _context.KolSocialAccounts
            .Where(s => s.KolUserId == userId)
            .ToListAsync();
    }

    public async Task<bool> AddSocialAccountAsync(Guid userId, KolSocialAccountViewModel model)
    {
        var social = new KolSocialAccount
        {
            Id = Guid.NewGuid(),
            KolUserId = userId,
            Platform = model.Platform,
            ProfileUrl = model.Url,
            Followers = model.FollowersCount,
            CreatedAt = DateTime.UtcNow
        };

        _context.KolSocialAccounts.Add(social);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveSocialAccountAsync(Guid userId, Guid socialId)
    {
        var social = await _context.KolSocialAccounts.FirstOrDefaultAsync(s => s.Id == socialId && s.KolUserId == userId);
        if (social == null) return false;

        _context.KolSocialAccounts.Remove(social);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<RateCard>> GetRateCardsAsync(Guid userId)
    {
        return await _context.RateCards
            .Include(r => r.RateCardItems)
            .Where(r => r.KolUserId == userId)
            .ToListAsync();
    }

    public async Task<bool> CreateRateCardAsync(Guid userId, RateCardViewModel model)
    {
        var rc = new RateCard
        {
            Id = Guid.NewGuid(),
            KolUserId = userId,
            Title = model.Title,
            Notes = model.Notes,
            Currency = model.Currency ?? "VND",
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.RateCards.Add(rc);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddRateCardItemAsync(Guid userId, Guid rateCardId, AddRateCardItemViewModel model)
    {
        var rc = await _context.RateCards.FirstOrDefaultAsync(r => r.Id == rateCardId && r.KolUserId == userId);
        if (rc == null) return false;

        var item = new RateCardItem
        {
            Id = Guid.NewGuid(),
            RateCardId = rateCardId,
            ServiceType = model.ServiceType,
            Platform = model.Platform,
            UnitPrice = model.UnitPrice,
            Unit = string.IsNullOrWhiteSpace(model.Unit) ? "Bài" : model.Unit,
            DurationMinutes = model.DurationMinutes,
            Description = model.Description
        };
        _context.RateCardItems.Add(item);
        rc.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRateCardAsync(Guid userId, Guid rateCardId)
    {
        var rc = await _context.RateCards
            .Include(r => r.RateCardItems)
            .FirstOrDefaultAsync(r => r.Id == rateCardId && r.KolUserId == userId);
        if (rc == null) return false;

        _context.RateCardItems.RemoveRange(rc.RateCardItems);
        _context.RateCards.Remove(rc);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateRateCardNotesAsync(Guid userId, Guid rateCardId, string? notes)
    {
        var rc = await _context.RateCards.FirstOrDefaultAsync(r => r.Id == rateCardId && r.KolUserId == userId);
        if (rc == null) return false;
        rc.Notes = notes;
        rc.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<StudioDashboardViewModel> GetStudioDashboardAsync(Guid userId)
    {
        var profile = await _context.KolProfiles
            .Include(p => p.User)
            .Include(p => p.KolSocialAccounts)
            .Include(p => p.KolPortfolios)
            .Include(p => p.RateCards)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null) throw new Exception("Profile not found");

        // --- Recent Booking Requests (real data) ---
        var recentRequests = await _context.BookingRequests
            .Where(r => r.KolUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Select(r => new BookingRequestSummaryViewModel
            {
                Id = r.Id,
                Title = r.Title,
                CustomerName = r.CustomerUser.FullName ?? r.CustomerUser.Email,
                BudgetMin = r.BudgetMin,
                BudgetMax = r.BudgetMax,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        // --- Upcoming Meetings (real data) ---
        var now = DateTime.UtcNow;
        var upcomingMeetings = await _context.MeetingParticipants
            .Where(mp => mp.UserId == userId && mp.Meeting.Status == "scheduled" && mp.Meeting.StartTime >= now)
            .OrderBy(mp => mp.Meeting.StartTime)
            .Take(3)
            .Select(mp => new UpcomingMeetingViewModel
            {
                Id = mp.Meeting.Id,
                Title = mp.Meeting.Title,
                StartTime = mp.Meeting.StartTime,
                EndTime = mp.Meeting.EndTime,
                MeetingUrl = mp.Meeting.MeetingUrl,
                MeetingType = mp.Meeting.MeetingType
            })
            .ToListAsync();

        // --- Real Activities from Notifications ---
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .Select(n => new StudioActivityViewModel
            {
                Title = n.Title,
                Description = n.Body ?? "",
                CreatedAt = n.CreatedAt,
                Type = n.Type == "booking_request" ? "success" : n.Type == "warning" ? "warning" : "info",
                Link = n.Link
            })
            .ToListAsync();

        var model = new StudioDashboardViewModel
        {
            FullName = profile.User.FullName ?? profile.User.Email,
            AvatarUrl = profile.User.AvatarUrl,
            InfluencerType = profile.InfluencerType,
            RatingAvg = profile.RatingAvg,
            TotalFollowers = profile.KolSocialAccounts.Sum(s => s.Followers ?? 0),
            PendingRequestsCount = recentRequests.Count(r => r.Status == "sent"),
            UpcomingMeetingsCount = upcomingMeetings.Count,
            WalletBalance = (await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == userId))?.Balance ?? 0,
            RecentBookingRequests = recentRequests,
            UpcomingMeetings = upcomingMeetings,
            RecentActivities = notifications
        };

        // Calculate Completeness
        int score = 0;
        if (!string.IsNullOrEmpty(profile.User.AvatarUrl)) score += 20;
        if (!string.IsNullOrEmpty(profile.Bio) && profile.Bio.Length > 20) score += 20;
        if (profile.KolSocialAccounts.Any()) score += 20;
        if (profile.KolPortfolios.Any()) score += 20;
        if (profile.RateCards.Any()) score += 20;
        model.ProfileCompleteness = score;

        return model;
    }

    public async Task<List<KolPortfolio>> GetPortfoliosAsync(Guid userId)
    {
        return await _context.KolPortfolios
            .Where(p => p.KolUserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> AddPortfolioAsync(Guid userId, PortfolioItemViewModel model)
    {
        var portfolio = new KolPortfolio
        {
            Id = Guid.NewGuid(),
            KolUserId = userId,
            Title = model.Title,
            Description = model.Description,
            MediaUrl = model.MediaUrl,
            ContentType = model.ContentType ?? "Image",
            CreatedAt = DateTime.UtcNow
        };

        _context.KolPortfolios.Add(portfolio);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePortfolioAsync(Guid userId, Guid portfolioId)
    {
        var portfolio = await _context.KolPortfolios.FirstOrDefaultAsync(p => p.Id == portfolioId && p.KolUserId == userId);
        if (portfolio == null) return false;

        _context.KolPortfolios.Remove(portfolio);
        await _context.SaveChangesAsync();
        return true;
    }
}
