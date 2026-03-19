using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.Services;
using KOL_KOC_TAAA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KOL_KOC_TAAA.Controllers;

public class PublicProfileController : Controller
{
    private readonly IKolProfileService _profileService;
    private readonly IBookingService _bookingService;
    private readonly IChatService _chatService;

    public PublicProfileController(IKolProfileService profileService, IBookingService bookingService, IChatService chatService)
    {
        _profileService = profileService;
        _bookingService = bookingService;
        _chatService = chatService;
    }

    [HttpGet]
    [Route("kol/{id}")]
    public async Task<IActionResult> Detail(Guid id)
    {
        var p = await _profileService.GetProfileByUserIdAsync(id);
        if (p == null || p.User.Status != "active") return NotFound();

        var mockIdols = MockIdolService.GetMockIdols();
        var mockMatch = mockIdols.FirstOrDefault(m => m.UserId == id);

        var model = new PublicKolProfileViewModel
        {
            UserId = p.UserId,
            FullName = p.User.FullName ?? p.User.Email,
            AvatarUrl = p.User.AvatarUrl ?? mockMatch?.AvatarUrl,
            InfluencerType = p.InfluencerType,
            Bio = p.Bio,
            LocationCity = p.LocationCity,
            LocationCountry = p.LocationCountry,
            RatingAvg = p.RatingAvg,
            RatingCount = p.RatingCount,
            IsVerified = p.IsVerified,
            MinBudget = p.MinBudget,
            ContactEmail = p.User?.Email,

            // Stats & Trust Signals
            TotalFollowers = p.KolSocialAccounts.Sum(s => s.Followers ?? 0),
            Platforms = p.KolSocialAccounts.Select(s => s.Platform).Distinct().ToList(),
            AvgResponseTime = mockMatch?.AvgResponseTime ?? "Trong vài giờ",
            CompletedCampaigns = mockMatch?.CompletedCampaigns ?? 0,
            CompletionRate = mockMatch?.CompletionRate ?? 100,

            SocialAccounts = p.KolSocialAccounts.ToList(),
            Portfolios = p.KolPortfolios.ToList(),
            RateCards = p.RateCards.ToList()
        };

        ViewBag.ContactEmail = p.User?.Email;
        ViewBag.KolUserId = p.UserId;

        return View(model);
    }

    [Authorize(Roles = "Customer")]
    [HttpGet]
    [Route("kol/{id}/book")]
    public async Task<IActionResult> Book(Guid id)
    {
        var p = await _profileService.GetProfileByUserIdAsync(id);
        if (p == null) return NotFound();

        var model = new CreateBookingRequestViewModel
        {
            KolUserId = id,
            Items = new List<BookingRequestItemViewModel> { new BookingRequestItemViewModel() }
        };

        ViewBag.KolName = p.User.FullName ?? p.User.Email;
        ViewBag.RateCards = p.RateCards.ToList();

        return View(model);
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    [Route("kol/{id}/book")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(Guid id, CreateBookingRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
             var p = await _profileService.GetProfileByUserIdAsync(id);
             ViewBag.KolName = p?.User.FullName ?? "KOL";
             ViewBag.RateCards = p?.RateCards.ToList();
             return View(model);
        }

        var customerIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(customerIdStr, out var customerId))
        {
            await _bookingService.CreateBookingRequestAsync(customerId, id, model);
            TempData["SuccessMessage"] = "Yêu cầu hợp tác của bạn đã được gửi thành công! Hãy chờ phản hồi từ KOL.";
            return RedirectToAction("Detail", new { id = id });
        }

        return Challenge();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> StartChat(Guid kolId)
    {
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(currentUserIdStr, out var currentUserId))
            return Challenge();

        var conv = await _chatService.GetOrCreateDirectConversationAsync(currentUserId, kolId);
        return RedirectToAction("Conversation", "Chat", new { id = conv.Id });
    }
}
