using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.Areas.Admin.ViewModels;

namespace KOL_KOC_TAAA.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Admin")]
public class KolManagementController : Controller
{
    private readonly KolMarketplaceContext _context;

    public KolManagementController(KolMarketplaceContext context)
    {
        _context = context;
    }

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        bool isAdmin = User.IsInRole("Admin") || 
            User.Claims.Any(c => c.Type == System.Security.Claims.ClaimTypes.Email && c.Value == "admin@kol.com");
            
        if (!isAdmin)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Auth", new { area = "" });
        }
        base.OnActionExecuting(context);
    }

    public async Task<IActionResult> Index()
    {

        var kols = await _context.KolProfiles
            .Include(k => k.User)
            .Where(k => k.User.Status != "deleted") // Soft delete filter
            .OrderByDescending(k => k.CreatedAt)
            .ToListAsync();
        return View(kols);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleVerify(Guid userId)
    {
        var kol = await _context.KolProfiles.FindAsync(userId);
        if (kol != null)
        {
            kol.IsVerified = !kol.IsVerified;
            kol.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã {(kol.IsVerified ? "xác thực" : "hủy xác thực")} KOL thành công.";
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/KolManagement/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/KolManagement/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KolCreateViewModel model)
    {

        if (!ModelState.IsValid)
            return View(model);

        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Email này đã được sử dụng.");
            return View(model);
        }

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Code.ToUpper() == "KOL");
        if (role == null)
        {
            role = new Role { Code = "KOL", Name = "KOL" };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        var newUserId = Guid.NewGuid();
        var newUser = new User
        {
            Id = newUserId,
            Email = model.Email,
            FullName = model.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Status = "active",
            Phone = model.Phone,
            CreatedAt = DateTime.UtcNow,
            Roles = new List<Role> { role }
        };

        var newProfile = new KolProfile
        {
            UserId = newUserId,
            InfluencerType = model.InfluencerType,
            Bio = model.Bio,
            LocationCity = model.LocationCity,
            LocationCountry = model.LocationCountry,
            MinBudget = model.MinBudget,
            RatingAvg = 0,
            RatingCount = 0,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(newUser);
        _context.KolProfiles.Add(newProfile);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Thêm KOL mới thành công.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/KolManagement/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        if (id == Guid.Empty) return NotFound();

        var kol = await _context.KolProfiles
            .Include(k => k.User)
            .FirstOrDefaultAsync(k => k.UserId == id);

        if (kol == null || kol.User.Status == "deleted") return NotFound();

        var model = new KolEditViewModel
        {
            UserId = kol.UserId,
            FullName = kol.User.FullName,
            Phone = kol.User.Phone,
            InfluencerType = kol.InfluencerType,
            Bio = kol.Bio,
            LocationCity = kol.LocationCity,
            LocationCountry = kol.LocationCountry,
            MinBudget = kol.MinBudget
        };

        return View(model);
    }

    // POST: Admin/KolManagement/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, KolEditViewModel model)
    {
        if (id != model.UserId) return NotFound();

        if (!ModelState.IsValid) return View(model);

        var kol = await _context.KolProfiles
            .Include(k => k.User)
            .FirstOrDefaultAsync(k => k.UserId == id);

        if (kol == null || kol.User.Status == "deleted") return NotFound();

        kol.User.FullName = model.FullName;
        kol.User.Phone = model.Phone;
        kol.User.UpdatedAt = DateTime.UtcNow;

        kol.InfluencerType = model.InfluencerType;
        kol.Bio = model.Bio;
        kol.LocationCity = model.LocationCity;
        kol.LocationCountry = model.LocationCountry;
        kol.MinBudget = model.MinBudget;
        kol.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật KOL thành công.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/KolManagement/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.Status = "deleted"; // Soft delete
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa KOL thành công.";
        }
        return RedirectToAction(nameof(Index));
    }
}
