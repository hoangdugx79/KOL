using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KOL_KOC_TAAA.Controllers;

[Authorize]
public class DeliverablesController : Controller
{
    private readonly KolMarketplaceContext _context;
    private readonly INotificationService _notificationService;
    private readonly IFinanceService _financeService;

    public DeliverablesController(KolMarketplaceContext context, 
                                INotificationService notificationService,
                                IFinanceService financeService)
    {
        _context = context;
        _notificationService = notificationService;
        _financeService = financeService;
    }

    private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Manage(Guid bookingId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Deliverables)
            .Include(b => b.KolUser.User)
            .Include(b => b.CustomerUser)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null) return NotFound();
        
        var userId = GetCurrentUserId();
        if (booking.CustomerUserId != userId && booking.KolUserId != userId) return Forbid();

        return View(booking);
    }

    [HttpPost]
    [Authorize(Roles = "KOL")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(Guid bookingId, string contentUrl, string notes)
    {
        var userId = GetCurrentUserId();
        var booking = await _context.Bookings.FindAsync(bookingId);
        
        if (booking == null || booking.KolUserId != userId) return NotFound();

        var deliverable = new Deliverable
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            DeliverableType = "content",
            Title = contentUrl, // Using Title to store URL
            Description = notes, // Using Description for notes
            Status = "submitted",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Deliverables.Add(deliverable);
        booking.Status = "work_submitted";
        
        await _context.SaveChangesAsync();

        // Notify Brand
        await _notificationService.SendNotificationAsync(booking.CustomerUserId,
            "work_submitted",
            "Sản phẩm mới đã được nộp",
            $"KOL đã nộp sản phẩm cho đơn hàng #{bookingId.ToString()[..8].ToUpper()}. Vui lòng kiểm tra và nghiệm thu.");

        return RedirectToAction(nameof(Manage), new { bookingId });
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid bookingId)
    {
        var userId = GetCurrentUserId();
        var booking = await _context.Bookings.FindAsync(bookingId);
        
        if (booking == null || booking.CustomerUserId != userId) return NotFound();
        if (booking.Status != "work_submitted") return BadRequest("Trạng thái không hợp lệ.");

        booking.Status = "approved";
        booking.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Notify KOL about approval
        await _notificationService.SendNotificationAsync(booking.KolUserId,
            "deliverable_approved",
            "Sản phẩm đã được nghiệm thu",
            $"Nhãn hàng đã nghiệm thu sản phẩm đơn hàng #{bookingId.ToString()[..8].ToUpper()}. Đang chờ thanh toán.");

        TempData["SuccessMessage"] = "Đã nghiệm thu sản phẩm thành công. Vui lòng thanh toán để hoàn tất.";
        return RedirectToAction("Detail", "Booking", new { id = bookingId });
    }
}
