using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;

namespace KOL_KOC_TAAA.Services;

public interface IMeetingService
{
    Task<Meeting?> GetMeetingAsync(Guid id);
    Task<List<Meeting>> GetBookingMeetingsAsync(Guid bookingId);
    Task<Meeting> CreateMeetingAsync(Guid createdByUserId, CreateMeetingViewModel model);
    Task<bool> UpdateMeetingStatusAsync(Guid meetingId, string status);
}
