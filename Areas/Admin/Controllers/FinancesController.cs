using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;

namespace KOL_KOC_TAAA.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Admin")]
public class FinancesController : Controller
{
    private readonly KolMarketplaceContext _context;

    public FinancesController(KolMarketplaceContext context)
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

    public async Task<IActionResult> Payouts()
    {

        var payouts = await _context.PayoutRequests
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return View(payouts);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessPayout(Guid payoutId, bool approve, string? adminNote)
    {

        var payout = await _context.PayoutRequests.FindAsync(payoutId);
        if (payout == null || payout.Status != "Pending") return NotFound();

        var wallet = await _context.UserWallets.FindAsync(payout.UserId);
        if (wallet == null) return NotFound();

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (approve)
            {
                payout.Status = "Approved";
                payout.ProcessedAt = DateTime.UtcNow;
                wallet.LockedBalance -= payout.Amount; // Remove from locked
            }
            else
            {
                payout.Status = "Rejected";
                payout.ProcessedAt = DateTime.UtcNow;
                wallet.LockedBalance -= payout.Amount; 
                wallet.Balance += payout.Amount; // Return to available balance
            }

            payout.AdminNote = adminNote;
            payout.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["SuccessMessage"] = $"Yêu cầu rút tiền đã được {(approve ? "duyệt" : "từ chối")}.";
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi xử lý thanh toán.";
        }

        return RedirectToAction(nameof(Payouts));
    }
}
