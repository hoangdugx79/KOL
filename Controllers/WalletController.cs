using KOL_KOC_TAAA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KOL_KOC_TAAA.Controllers;

[Authorize]
public class WalletController : Controller
{
    private readonly IFinanceService _financeService;

    public WalletController(IFinanceService financeService)
    {
        _financeService = financeService;
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
        var wallet = await _financeService.GetOrCreateWalletAsync(userId);
        var ledger = await _financeService.GetLedgerHistoryAsync(userId);
        var (pendingEscrowTotal, escrowBookings) = await _financeService.GetPendingEscrowAsync(userId);

        ViewBag.Ledger = ledger;
        ViewBag.PendingEscrow = pendingEscrowTotal;
        ViewBag.EscrowBookings = escrowBookings;
        return View(wallet);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestPayout(decimal amount, string bankInfo)
    {
        if (amount <= 0)
        {
            TempData["ErrorMessage"] = "Số tiền rút phải lớn hơn 0.";
            return RedirectToAction(nameof(Index));
        }

        var userId = GetCurrentUserId();
        try
        {
            await _financeService.RequestPayoutAsync(userId, amount, bankInfo);
            TempData["SuccessMessage"] = "Yêu cầu rút tiền của bạn đã được gửi. Chúng tôi sẽ xử lý trong vòng 24-48h.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
