using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;
using System.Security.Claims;
using System.Text.Json;
using KOL_KOC_TAAA.Services;

namespace KOL_KOC_TAAA.Controllers;

[Authorize]
public class BookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IFinanceService _financeService;
    private readonly KolMarketplaceContext _db;
    private readonly IChatService _chatService;

    public BookingController(IBookingService bookingService, IFinanceService financeService, KolMarketplaceContext db, IChatService chatService)
    {
        _bookingService = bookingService;
        _financeService = financeService;
        _db = db;
        _chatService = chatService;
    }

    private Guid GetCurrentUserId()
    {
        var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idString, out var id) ? id : Guid.Empty;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var model = new BookingListViewModel();
        
        if (User.IsInRole("Customer"))
        {
            model.SentRequests = await _bookingService.GetCustomerBookingRequestsAsync(userId);
            model.ActiveBookings = await _db.Bookings
                .Include(b => b.BookingRequest)
                .Include(b => b.KolUser).ThenInclude(k => k.User)
                .Where(b => b.CustomerUserId == userId && b.Status != "completed")
                .OrderByDescending(b => b.UpdatedAt)
                .ToListAsync();
        }
        
        if (User.IsInRole("KOL"))
        {
            model.ReceivedRequests = await _bookingService.GetKolBookingRequestsAsync(userId);
            model.ActiveBookings = await _db.Bookings
                .Include(b => b.BookingRequest)
                .Include(b => b.CustomerUser)
                .Where(b => b.KolUserId == userId && b.Status != "completed")
                .OrderByDescending(b => b.UpdatedAt)
                .ToListAsync();
        }

        return View(model);
    }

    [Authorize(Roles = "KOL")]
    public async Task<IActionResult> ManageBookings()
    {
        var userId = GetCurrentUserId();
        var requests = await _db.BookingRequests
            .Include(r => r.CustomerUser)
            .Include(r => r.ChatConversations)
            .Include(r => r.Booking)
                .ThenInclude(b => b.Contracts)
            .Where(r => r.KolUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(requests);
    }

    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> MyRequests()
    {
        var userId = GetCurrentUserId();
        var requests = await _db.BookingRequests
            .Include(r => r.KolUser).ThenInclude(k => k.User)
            .Include(r => r.BookingRequestItems)
            .Include(r => r.ChatConversations)
            .Include(r => r.Booking)
                .ThenInclude(b => b.Contracts)
            .Where(r => r.CustomerUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(requests);
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        var booking = await _bookingService.GetBookingAsync(id);
        if (booking == null) return NotFound();

        var userId = GetCurrentUserId();
        if (booking.CustomerUserId != userId && booking.KolUserId != userId) return Forbid();

        var completedStatuses = new[] { "paid", "completed" };
        var model = new BookingDetailViewModel
        {
            Booking = booking,
            Role = booking.KolUserId == userId ? "KOL" : "Customer",
            IsContractSigned = booking.Contracts.Any(c => c.Status == "signed"),
            HasDeliverablesSubmitted = booking.Deliverables.Any(d => d.Status == "submitted"),
            IsApproved = booking.Status == "approved" || completedStatuses.Contains(booking.Status),
            IsPaymentReceived = completedStatuses.Contains(booking.Status),
            IsCompleted = booking.Status == "completed"
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "KOL")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Respond(Guid requestId, string status)
    {
        var userId = GetCurrentUserId();
        var request = await _bookingService.GetBookingRequestAsync(requestId);
        
        if (request == null || request.KolUserId != userId) return NotFound();

        if (status == "accepted")
        {
            await _bookingService.FinalizeBookingFromRequestAsync(requestId);
            // Auto-create chat conversation between KOL and Customer
            await _chatService.GetOrCreateBookingConversationAsync(
                requestId, request.KolUserId, request.CustomerUserId, request.Title);
            TempData["SuccessMessage"] = "Bạn đã chấp nhận yêu cầu. Hội thoại chat với khách hàng đã được tạo tự động!";
            return RedirectToAction(nameof(RequestDetail), new { id = requestId });
        }
        else if (status == "rejected")
        {
            await _bookingService.UpdateBookingRequestStatusAsync(requestId, "rejected");
            TempData["SuccessMessage"] = "Đã từ chối yêu cầu hợp tác.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Pay(Guid id)
    {
        var booking = await _bookingService.GetBookingAsync(id);
        if (booking == null || booking.CustomerUserId != GetCurrentUserId()) return NotFound();
        if (booking.Status != "approved")
        {
            TempData["ErrorMessage"] = "Chỉ có thể thanh toán sau khi nghiệm thu sản phẩm.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        return View(booking);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPayment(Guid bookingId)
    {
        var userId = GetCurrentUserId();
        var booking = await _bookingService.GetBookingAsync(bookingId);
        
        if (booking == null || booking.CustomerUserId != userId) return NotFound();
        if (booking.Status != "approved")
        {
            TempData["ErrorMessage"] = "Trạng thái đơn hàng không hợp lệ để thanh toán.";
            return RedirectToAction(nameof(Detail), new { id = bookingId });
        }

        var success = await _financeService.ProcessPaymentAsync(bookingId, userId, booking.TotalAmount);

        if (success)
        {
            // Notify KOL about payment & release
            var notificationService = HttpContext.RequestServices.GetRequiredService<INotificationService>();
            await notificationService.SendNotificationAsync(booking.KolUserId,
                "payment_released",
                "Thanh toán hoàn tất",
                $"Nhãn hàng đã thanh toán đơn hàng #{bookingId.ToString()[..8].ToUpper()}. Tiền đã được chuyển vào ví của bạn.");

            TempData["SuccessMessage"] = "Thanh toán thành công! Tiền đã được chuyển vào ví KOL. Đơn hàng hoàn tất.";
            return RedirectToAction(nameof(Detail), new { id = bookingId });
        }

        TempData["ErrorMessage"] = "Thanh toán thất bại. Vui lòng thử lại.";
        return RedirectToAction(nameof(Pay), new { id = bookingId });
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    [Route("api/booking/create")]
    public async Task<IActionResult> ApiCreate([FromBody] ApiBookingRequestDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Brief))
            return BadRequest(new { success = false, message = "Vui lòng nhập mô tả chiến dịch." });

        var customerId = GetCurrentUserId();
        if (customerId == Guid.Empty) return Unauthorized();

        var model = new CreateBookingRequestViewModel
        {
            KolUserId = dto.KolUserId,
            Title = dto.Title ?? dto.Brief,
            Brief = dto.Brief,
            BudgetMin = dto.BudgetMin,
            BudgetMax = dto.BudgetMin,
            Currency = "VND",
            ProposedStartDate = dto.StartDate.HasValue ? DateOnly.FromDateTime(dto.StartDate.Value) : null,
            ProposedEndDate = dto.EndDate.HasValue ? DateOnly.FromDateTime(dto.EndDate.Value) : null,
            Items = new List<BookingRequestItemViewModel>
            {
                new BookingRequestItemViewModel
                {
                    ServiceType = dto.ServiceType ?? "Content",
                    Platform = dto.ServiceType ?? "All",
                    Quantity = 1,
                    Notes = JsonSerializer.Serialize(new { hour = dto.WorkingHour ?? "", note = dto.Note ?? "" })
                }
            }
        };

        var request = await _bookingService.CreateBookingRequestAsync(customerId, dto.KolUserId, model);
        return Ok(new { success = true, requestId = request.Id });
    }

    [Authorize(Roles = "KOL,Customer,Admin")]
    public async Task<IActionResult> RequestDetail(Guid id)
    {
        var userId = GetCurrentUserId();
        var request = await _db.BookingRequests
            .Include(r => r.CustomerUser)
            .Include(r => r.KolUser).ThenInclude(k => k.User)
            .Include(r => r.BookingRequestItems)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null) return NotFound();
        bool isAdmin = User.IsInRole("Admin");
        if (!isAdmin && request.KolUserId != userId && request.CustomerUserId != userId) return Forbid();

        ViewBag.IsKol = request.KolUserId == userId;
        ViewBag.IsAdmin = isAdmin;
        return View(request);
    }
}

public class ApiBookingRequestDto
{
    public Guid KolUserId { get; set; }
    public string? Title { get; set; }
    public string? Brief { get; set; }
    public decimal? BudgetMin { get; set; }
    public string? ServiceType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? WorkingHour { get; set; }
    public string? Note { get; set; }
}
