using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;
using System.Security.Claims;

namespace KOL_KOC_TAAA.Controllers;

[Authorize(Roles = "Customer")]
public class ReviewController : Controller
{
    private readonly KolMarketplaceContext _context;

    public ReviewController(KolMarketplaceContext context)
    {
        _context = context;
    }

    private Guid GetCurrentUserId()
    {
        var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idString, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> Submit(Guid bookingId)
    {
        var booking = await _context.Bookings
            .Include(b => b.KolUser)
                .ThenInclude(k => k.User)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.CustomerUserId == GetCurrentUserId());

        if (booking == null) return NotFound();
        if (booking.Status != "completed") return BadRequest("Chỉ có thể đánh giá sau khi hoàn thành đơn hàng.");

        // Check if already reviewed
        var existing = await _context.Reviews.AnyAsync(r => r.BookingId == bookingId);
        if (existing) return BadRequest("Bạn đã đánh giá đơn hàng này rồi.");

        var model = new SubmitReviewViewModel
        {
            BookingId = bookingId,
            KolName = booking.KolUser.User.FullName ?? "KOL",
            Rating = 5
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(SubmitReviewViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = GetCurrentUserId();
        var booking = await _context.Bookings
            .Include(b => b.KolUser)
            .FirstOrDefaultAsync(b => b.Id == model.BookingId && b.CustomerUserId == userId);

        if (booking == null) return NotFound();

        var review = new Review
        {
            Id = Guid.NewGuid(),
            BookingId = model.BookingId,
            CustomerUserId = userId,
            KolUserId = booking.KolUserId,
            Rating = model.Rating,
            Comment = model.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);

        // Aggregate Ratings
        var kolProfile = await _context.KolProfiles.FindAsync(booking.KolUserId);
        if (kolProfile != null)
        {
            var allReviews = await _context.Reviews.Where(r => r.KolUserId == booking.KolUserId).ToListAsync();
            int newCount = allReviews.Count + 1;
            decimal newAvg = (allReviews.Sum(r => r.Rating) + model.Rating) / (decimal)newCount;

            kolProfile.RatingCount = newCount;
            kolProfile.RatingAvg = newAvg;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi đánh giá!";
        return RedirectToAction("Index", "Home");
    }
}
