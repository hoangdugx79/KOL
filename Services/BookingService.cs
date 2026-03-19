using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Services;

public class BookingService : IBookingService
{
    private readonly KolMarketplaceContext _context;
    private readonly INotificationService _notificationService;

    public BookingService(KolMarketplaceContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<BookingRequest?> GetBookingRequestAsync(Guid id)
    {
        return await _context.BookingRequests
            .Include(r => r.BookingRequestItems)
            .Include(r => r.CustomerUser)
            .Include(r => r.KolUser.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<BookingRequest>> GetCustomerBookingRequestsAsync(Guid customerId)
    {
        return await _context.BookingRequests
            .Include(r => r.KolUser.User)
            .Include(r => r.Booking)
            .Include(r => r.ChatConversations)
            .Where(r => r.CustomerUserId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<BookingRequest>> GetKolBookingRequestsAsync(Guid kolId)
    {
        return await _context.BookingRequests
            .Include(r => r.CustomerUser)
            .Include(r => r.Booking)
            .Include(r => r.ChatConversations)
            .Where(r => r.KolUserId == kolId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<BookingRequest> CreateBookingRequestAsync(Guid customerId, Guid kolId, CreateBookingRequestViewModel model)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var requestId = Guid.NewGuid();
            var request = new BookingRequest
            {
                Id = requestId,
                CustomerUserId = customerId,
                KolUserId = kolId,
                Title = model.Title,
                Brief = model.Brief,
                BudgetMin = model.BudgetMin,
                BudgetMax = model.BudgetMax,
                Currency = model.Currency,
                ProposedStartDate = model.ProposedStartDate,
                ProposedEndDate = model.ProposedEndDate,
                Status = "sent",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (model.Items != null && model.Items.Count > 0)
            {
                foreach (var item in model.Items)
                {
                    request.BookingRequestItems.Add(new BookingRequestItem
                    {
                        Id = Guid.NewGuid(),
                        BookingRequestId = requestId,
                        ServiceType = item.ServiceType,
                        Platform = item.Platform,
                        Quantity = item.Quantity,
                        ExpectedUnitPrice = item.ExpectedUnitPrice,
                        Notes = item.Notes
                    });
                }
            }

            _context.BookingRequests.Add(request);

            // Auto-create Chat Conversation for this request
            var chat = new ChatConversation
            {
                Id = Guid.NewGuid(),
                Type = "booking_negotiation",
                BookingRequestId = requestId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _context.ChatConversations.Add(chat);

            // Add members to chat
            _context.ChatMembers.Add(new ChatMember { ConversationId = chat.Id, UserId = customerId });
            _context.ChatMembers.Add(new ChatMember { ConversationId = chat.Id, UserId = kolId });

            // Initial System Message
            _context.ChatMessages.Add(new ChatMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = chat.Id,
                SenderUserId = customerId, // System could be a special user but using customer for now
                Content = $"Yêu cầu hợp tác mới cho chiến dịch: **{model.Title}** đã được khởi tạo.",
                MessageType = "system",
                SentAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            
            // Notify KOL about new request
            await _notificationService.SendNotificationAsync(kolId, 
                "booking_request",
                "Yêu cầu hợp tác mới", 
                $"Bạn nhận được một yêu cầu mới từ nhãn hàng cho chiến dịch: {model.Title}");

            await transaction.CommitAsync();

            return request;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateBookingRequestStatusAsync(Guid requestId, string status)
    {
        var request = await _context.BookingRequests.FindAsync(requestId);
        if (request == null) return false;

        request.Status = status;
        request.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<Booking?> GetBookingAsync(Guid id)
    {
        return _context.Bookings
            .Include(b => b.BookingRequest)
                .ThenInclude(r => r.BookingRequestItems)
            .Include(b => b.BookingRequest)
                .ThenInclude(r => r.ChatConversations)
            .Include(b => b.BookingItems)
            .Include(b => b.Contracts)
                .ThenInclude(c => c.ContractSignatures)
            .Include(b => b.Deliverables)
            .Include(b => b.CustomerUser)
            .Include(b => b.KolUser)
                .ThenInclude(k => k.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Booking> FinalizeBookingFromRequestAsync(Guid requestId)
    {
        var request = await _context.BookingRequests
            .Include(r => r.BookingRequestItems)
            .FirstOrDefaultAsync(r => r.Id == requestId);
            
        if (request == null) throw new Exception("Request not found");

        var itemsTotal = request.BookingRequestItems.Sum(i => (i.ExpectedUnitPrice ?? 0) * i.Quantity);
        var totalAmount = itemsTotal > 0 ? itemsTotal : (request.BudgetMin ?? 0);

        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            BookingRequestId = requestId,
            CustomerUserId = request.CustomerUserId,
            KolUserId = request.KolUserId,
            TotalAmount = totalAmount,
            Currency = request.Currency,
            Status = "in_progress",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var item in request.BookingRequestItems)
        {
            var unitPrice = item.ExpectedUnitPrice ?? 0;
            // Nếu chỉ có 1 item và giá = 0, dùng BudgetMin
            if (unitPrice == 0 && request.BookingRequestItems.Count == 1)
                unitPrice = request.BudgetMin ?? 0;

            booking.BookingItems.Add(new BookingItem
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                ServiceType = item.ServiceType,
                UnitPrice = unitPrice,
                Quantity = item.Quantity,
                LineTotal = unitPrice * item.Quantity
            });
        }

        _context.Bookings.Add(booking);
        
        // Update request status to accepted
        request.Status = "accepted";

        await _context.SaveChangesAsync();

        // Notify Customer that KOL accepted
        await _notificationService.SendNotificationAsync(request.CustomerUserId,
            "booking_accepted",
            "Yêu cầu đã được chấp nhận",
            $"KOL đã chấp nhận yêu cầu của bạn. Đơn hàng #{bookingId.ToString()[..8].ToUpper()} đã được khởi tạo.");

        return booking;
    }
}
