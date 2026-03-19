using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOL_KOC_TAAA.Data;

namespace KOL_KOC_TAAA.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly KolMarketplaceContext _context;

    public DashboardController(KolMarketplaceContext context)
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

        ViewBag.TotalUsers = await _context.Users.CountAsync();
        ViewBag.TotalKols = await _context.KolProfiles.CountAsync();
        ViewBag.TotalBookings = await _context.Bookings.CountAsync();
        ViewBag.PlatformRevenue = await _context.Bookings.SumAsync(b => b.PlatformFee);
        
        var recentBookings = await _context.Bookings
            .Include(b => b.CustomerUser)
            .Include(b => b.KolUser.User)
            .Include(b => b.BookingRequest)
            .OrderByDescending(b => b.CreatedAt)
            .Take(10)
            .ToListAsync();

        return View(recentBookings);
    }
}
