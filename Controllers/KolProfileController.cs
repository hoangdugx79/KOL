using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;
using System.Security.Claims;
using KOL_KOC_TAAA.Services;

namespace KOL_KOC_TAAA.Controllers;

[Authorize(Roles = "KOL")]
public class KolProfileController : Controller
{
    private readonly IKolProfileService _profileService;

    public KolProfileController(IKolProfileService profileService)
    {
        _profileService = profileService;
    }

    private Guid GetCurrentUserId()
    {
        var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idString, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var profile = await _profileService.GetProfileByUserIdAsync(userId);

        if (profile == null) return NotFound("Profile not found");

        return View(profile);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userId = GetCurrentUserId();
        var profile = await _profileService.GetProfileByUserIdAsync(userId);
        if (profile == null) return NotFound();

        var model = new KolProfileEditViewModel
        {
            InfluencerType = profile.InfluencerType,
            Bio = profile.Bio,
            Gender = profile.Gender,
            Dob = profile.Dob,
            LocationCity = profile.LocationCity,
            LocationCountry = profile.LocationCountry,
            MinBudget = profile.MinBudget
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(KolProfileEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = GetCurrentUserId();
        var success = await _profileService.UpdateProfileAsync(userId, model);

        if (success)
        {
            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Socials()
    {
        var userId = GetCurrentUserId();
        var accounts = await _profileService.GetSocialAccountsAsync(userId);
        
        var model = accounts.Select(s => new KolSocialAccountViewModel
        {
            Id = s.Id,
            Platform = s.Platform,
            Url = s.ProfileUrl ?? "",
            FollowersCount = (int?)s.Followers
        }).ToList();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddSocial(KolSocialAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Thông tin không hợp lệ.";
            return RedirectToAction(nameof(Socials));
        }

        var userId = GetCurrentUserId();
        await _profileService.AddSocialAccountAsync(userId, model);
        
        TempData["SuccessMessage"] = "Đã thêm mạng xã hội.";
        return RedirectToAction(nameof(Socials));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveSocial(Guid id)
    {
        var userId = GetCurrentUserId();
        await _profileService.RemoveSocialAccountAsync(userId, id);
        
        TempData["SuccessMessage"] = "Đã xóa mạng xã hội.";
        return RedirectToAction(nameof(Socials));
    }

    [HttpGet]
    public async Task<IActionResult> RateCards()
    {
        var userId = GetCurrentUserId();
        var rateCards = await _profileService.GetRateCardsAsync(userId);
        return View(rateCards);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRateCard(RateCardViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = GetCurrentUserId();
            await _profileService.CreateRateCardAsync(userId, model);
            TempData["SuccessMessage"] = "Tạo Rate Card thành công.";
        }
        return RedirectToAction(nameof(RateCards));
    }
}
