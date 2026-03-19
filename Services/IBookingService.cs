using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;

namespace KOL_KOC_TAAA.Services;

public interface IBookingService
{
    // Booking Request Flow
    Task<BookingRequest?> GetBookingRequestAsync(Guid id);
    Task<List<BookingRequest>> GetCustomerBookingRequestsAsync(Guid customerId);
    Task<List<BookingRequest>> GetKolBookingRequestsAsync(Guid kolId);
    
    Task<BookingRequest> CreateBookingRequestAsync(Guid customerId, Guid kolId, CreateBookingRequestViewModel model);
    
    Task<bool> UpdateBookingRequestStatusAsync(Guid requestId, string status); // e.g., 'accepted', 'rejected', 'cancelled'
    
    // Booking Flow (Finalized)
    Task<Booking?> GetBookingAsync(Guid id);
    Task<Booking> FinalizeBookingFromRequestAsync(Guid requestId);
}
