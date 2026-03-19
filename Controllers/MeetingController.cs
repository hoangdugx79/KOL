using System.Security.Claims;
using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Services;
using KOL_KOC_TAAA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Controllers;

[Authorize]
public class MeetingController : Controller
{
    private readonly IMeetingService _meetingService;
    private readonly IBookingService _bookingService;
    private readonly KolMarketplaceContext _db;

    public MeetingController(IMeetingService meetingService, IBookingService bookingService, KolMarketplaceContext db)
    {
        _meetingService = meetingService;
        _bookingService = bookingService;
        _db = db;
    }

    [Authorize(Roles = "KOL")]
    public async Task<IActionResult> Availability(int? year, int? month)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Challenge();

        var now = DateTime.Today;
        var today = DateOnly.FromDateTime(now);
        var y = year ?? now.Year;
        var m = month ?? now.Month;
        // keep within valid range
        if (m < 1) { m = 12; y--; }
        if (m > 12) { m = 1; y++; }

        var ranges = await _db.BookingRequests
            .Include(r => r.CustomerUser)
            .Where(r => r.KolUserId == userId && r.ProposedStartDate != null && r.ProposedEndDate != null)
            .OrderBy(r => r.ProposedStartDate)
            .ToListAsync();

        // Auto-complete expired bookings
        var expiredStatuses = new[] { "accepted", "active_contract", "sent" };
        var expired = ranges.Where(r => r.ProposedEndDate < today && expiredStatuses.Contains(r.Status)).ToList();
        foreach (var r in expired)
        {
            r.Status = "completed";
            r.UpdatedAt = DateTime.UtcNow;
        }
        if (expired.Any())
            await _db.SaveChangesAsync();

        var rangeDtos = ranges.Select(r => new BookingRangeDto
            {
                Id = r.Id,
                Title = r.Title ?? "(Không có tiêu đề)",
                CustomerName = r.CustomerUser != null ? r.CustomerUser.FullName : "Khách hàng",
                StartDate = r.ProposedStartDate,
                EndDate = r.ProposedEndDate,
                Status = r.Status ?? "sent"
            })
            .ToList();

        var vm = new KolAvailabilityViewModel
        {
            Year = y,
            Month = m,
            BookingRanges = rangeDtos
        };

        return View(vm);
    }

    public async Task<IActionResult> Schedule(Guid bookingId)
    {
        var booking = await _bookingService.GetBookingAsync(bookingId);
        if (booking == null) return NotFound();

        var model = new CreateMeetingViewModel
        {
            BookingId = bookingId,
            Title = $"Họp thảo luận: {booking.BookingRequest.Title}",
            StartTime = DateTime.Now.AddDays(1).Date.AddHours(9),
            EndTime = DateTime.Now.AddDays(1).Date.AddHours(10)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Schedule(CreateMeetingViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Challenge();

        try
        {
            var meeting = await _meetingService.CreateMeetingAsync(userId, model);
            TempData["SuccessMessage"] = "Đã lên lịch họp thành công!";
            return RedirectToAction("Detail", "Booking", new { id = model.BookingId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Lỗi: " + ex.Message);
            return View(model);
        }
    }
}
