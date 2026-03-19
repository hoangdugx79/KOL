using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Services;

public class MeetingService : IMeetingService
{
    private readonly KolMarketplaceContext _context;

    public MeetingService(KolMarketplaceContext context)
    {
        _context = context;
    }

    public async Task<Meeting?> GetMeetingAsync(Guid id)
    {
        return await _context.Meetings
            .Include(m => m.MeetingParticipants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Meeting>> GetBookingMeetingsAsync(Guid bookingId)
    {
        return await _context.Meetings
            .Where(m => m.BookingId == bookingId)
            .OrderByDescending(m => m.StartTime)
            .ToListAsync();
    }

    public async Task<Meeting> CreateMeetingAsync(Guid createdByUserId, CreateMeetingViewModel model)
    {
        var booking = await _context.Bookings.FindAsync(model.BookingId);
        if (booking == null) throw new Exception("Booking not found");

        var meeting = new Meeting
        {
            Id = Guid.NewGuid(),
            BookingId = model.BookingId,
            CreatedByUserId = createdByUserId,
            Title = model.Title,
            Agenda = model.Agenda,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            MeetingType = model.MeetingType ?? "online",
            MeetingUrl = model.MeetingUrl,
            Status = "scheduled",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Meetings.Add(meeting);

        // Add both parties as participants
        _context.MeetingParticipants.Add(new MeetingParticipant
        {
            MeetingId = meeting.Id,
            UserId = booking.CustomerUserId,
            Role = "participant",
            AttendanceStatus = "pending"
        });

        _context.MeetingParticipants.Add(new MeetingParticipant
        {
            MeetingId = meeting.Id,
            UserId = booking.KolUserId,
            Role = "participant",
            AttendanceStatus = "pending"
        });

        await _context.SaveChangesAsync();
        return meeting;
    }

    public async Task<bool> UpdateMeetingStatusAsync(Guid meetingId, string status)
    {
        var meeting = await _context.Meetings.FindAsync(meetingId);
        if (meeting == null) return false;

        meeting.Status = status;
        meeting.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
