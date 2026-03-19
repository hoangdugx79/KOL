using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly KolMarketplaceContext _context;

    public AdminController(KolMarketplaceContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Platform Overview
        ViewBag.UserCount = await _context.Users.CountAsync();
        ViewBag.KolCount = await _context.KolProfiles.CountAsync();
        ViewBag.TotalRevenue = await _context.Bookings.Where(b => b.Status == "completed").SumAsync(b => b.PlatformFee);
        ViewBag.PendingPayouts = await _context.PayoutRequests.Where(p => p.Status == "pending").CountAsync();

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> KolManagement()
    {
        var kols = await _context.KolProfiles
            .Include(p => p.User)
            .OrderByDescending(p => p.User.CreatedAt)
            .ToListAsync();
        return View(kols);
    }

    [HttpGet]
    public async Task<IActionResult> PayoutRequests()
    {
        var requests = await _context.PayoutRequests
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return View(requests);
    }

    [HttpPost]
    public async Task<IActionResult> ApprovePayout(Guid id)
    {
        var request = await _context.PayoutRequests.FindAsync(id);
        if (request == null) return NotFound();

        request.Status = "completed";
        request.UpdatedAt = DateTime.UtcNow;

        // In a real app, move LockedBalance to actual Outgoing
        var wallet = await _context.UserWallets.FindAsync(request.UserId);
        if (wallet != null)
        {
            wallet.LockedBalance -= request.Amount;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã phê duyệt và hoàn tất rút tiền.";
        return RedirectToAction(nameof(PayoutRequests));
    }

    [HttpGet]
    public async Task<IActionResult> Disputes()
    {
        var disputes = await _context.Disputes
            .Include(d => d.Booking)
            .Include(d => d.RaisedByUser)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
        return View(disputes);
    }
}
