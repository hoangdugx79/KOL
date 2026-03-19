using System.Security.Claims;
using KOL_KOC_TAAA.Services;
using KOL_KOC_TAAA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KOL_KOC_TAAA.Controllers;

[Authorize(Roles = "KOL")]
public class CreatorStudioController : Controller
{
    private readonly IKolProfileService _profileService;
    private readonly IWebHostEnvironment _env;

    public CreatorStudioController(IKolProfileService profileService, IWebHostEnvironment env)
    {
        _profileService = profileService;
        _env = env;
    }

    private async Task<string?> SaveUploadedFileAsync(IFormFile file, string subFolder)
    {
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext)) return null;
        if (file.Length > 10L * 1024 * 1024) return null; // max 10MB

        var uploadPath = Path.Combine(_env.WebRootPath, "uploads", subFolder);
        Directory.CreateDirectory(uploadPath);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadPath, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return $"/uploads/{subFolder}/{fileName}";
    }

    public async Task<IActionResult> Index()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            var model = await _profileService.GetStudioDashboardAsync(userId);
            return View(model);
        }
        return Challenge();
    }

    public async Task<IActionResult> EditProfile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            var profile = await _profileService.GetProfileByUserIdAsync(userId);
            if (profile == null) return NotFound();

            var model = new KolProfileEditViewModel
            {
                FullName = profile.User?.FullName,
                AvatarUrl = profile.User?.AvatarUrl,
                ContactEmail = profile.User?.Email,
                InfluencerType = profile.InfluencerType,
                Bio = profile.Bio,
                Gender = profile.Gender,
                Dob = profile.Dob,
                LocationCity = profile.LocationCity,
                LocationCountry = profile.LocationCountry,
                MinBudget = profile.MinBudget
            };

            ViewBag.SocialAccounts = profile.KolSocialAccounts?.ToList() ?? new List<KOL_KOC_TAAA.Models.KolSocialAccount>();
            ViewBag.Portfolios = profile.KolPortfolios?.ToList() ?? new List<KOL_KOC_TAAA.Models.KolPortfolio>();
            ViewBag.RateCards = profile.RateCards?.ToList() ?? new List<KOL_KOC_TAAA.Models.RateCard>();

            return View(model);
        }
        return Challenge();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(KolProfileEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            // Handle avatar file upload — overrides manual URL
            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                var url = await SaveUploadedFileAsync(model.AvatarFile, "avatars");
                if (url != null) model.AvatarUrl = url;
            }

            var success = await _profileService.UpdateProfileAsync(userId, model);
            if (success)
            {
                TempData["SuccessMessage"] = "Hồ sơ đã được cập nhật thành công!";
                return RedirectToAction(nameof(EditProfile));
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Portfolio()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            var portfolios = await _profileService.GetPortfoliosAsync(userId);
            return View(portfolios);
        }
        return Challenge();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPortfolio(PortfolioItemViewModel model)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Challenge();

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            var url = await SaveUploadedFileAsync(model.ImageFile, "portfolio");
            if (url != null) model.MediaUrl = url;
        }

        if (!string.IsNullOrWhiteSpace(model.Title))
        {
            await _profileService.AddPortfolioAsync(userId, model);
            TempData["SuccessMessage"] = "Đã thêm tác phẩm vào Portfolio!";
        }
        return RedirectToAction(nameof(EditProfile));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePortfolio(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            await _profileService.DeletePortfolioAsync(userId, id);
            TempData["SuccessMessage"] = "Đã xóa tác phẩm khỏi Portfolio.";
        }
        return RedirectToAction(nameof(EditProfile));
    }

    public async Task<IActionResult> SocialAccounts()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            var accounts = await _profileService.GetSocialAccountsAsync(userId);
            return View(accounts);
        }
        return Challenge();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSocialAccount(KolSocialAccountViewModel model)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(SocialAccounts));

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            await _profileService.AddSocialAccountAsync(userId, model);
            TempData["SuccessMessage"] = "Đã liên kết tài khoản mạng xã hội thành công!";
        }
        return RedirectToAction(nameof(SocialAccounts));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveSocialAccount(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            await _profileService.RemoveSocialAccountAsync(userId, id);
            TempData["SuccessMessage"] = "Đã hủy liên kết tài khoản.";
        }
        return RedirectToAction(nameof(EditProfile));
    }

    public async Task<IActionResult> RateCard()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            var rateCards = await _profileService.GetRateCardsAsync(userId);
            return View(rateCards);
        }
        return Challenge();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRateCard(RateCardViewModel model)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(RateCard));

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            await _profileService.CreateRateCardAsync(userId, model);
            TempData["SuccessMessage"] = "Đã tạo gói dịch vụ mới!";
        }
        return RedirectToAction(nameof(RateCard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRateCardItem(Guid rateCardId, AddRateCardItemViewModel model)    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            await _profileService.AddRateCardItemAsync(userId, rateCardId, model);
            TempData["SuccessMessage"] = "Đã thêm hạng mục dịch vụ!";
        }
        return RedirectToAction(nameof(RateCard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRateCard(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            await _profileService.DeleteRateCardAsync(userId, id);
            TempData["SuccessMessage"] = "Đã xóa gói dịch vụ.";
        }
        return RedirectToAction(nameof(RateCard));
    }

    public IActionResult CreateContent()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRateCardNotes(Guid id, string? notes)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            await _profileService.UpdateRateCardNotesAsync(userId, id, notes);
            TempData["SuccessMessage"] = "Đã lưu ghi chú!";
        }
        return RedirectToAction(nameof(RateCard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateContent(PortfolioItemViewModel model)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Challenge();

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            var url = await SaveUploadedFileAsync(model.ImageFile, "portfolio");
            if (url != null) model.MediaUrl = url;
        }

        if (!string.IsNullOrWhiteSpace(model.Title))
        {
            await _profileService.AddPortfolioAsync(userId, model);
            TempData["SuccessMessage"] = "Đã đăng nội dung thành công!";
            return RedirectToAction(nameof(CreateContent));
        }

        return View(model);
    }
}
