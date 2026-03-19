using System;
using System.Collections.Generic;
using KOL_KOC_TAAA.Models;

namespace KOL_KOC_TAAA.ViewModels;

public class BookingListViewModel
{
    public List<BookingRequest> SentRequests { get; set; } = new();
    public List<BookingRequest> ReceivedRequests { get; set; } = new();
    public List<Booking> ActiveBookings { get; set; } = new();
}

public class BookingDetailViewModel
{
    public Booking Booking { get; set; } = null!;
    public string Role { get; set; } = null!; // 'Customer' or 'KOL'
    
    // Status tracking flags
    public bool IsContractSigned { get; set; }
    public bool HasDeliverablesSubmitted { get; set; }
    public bool IsApproved { get; set; }
    public bool IsPaymentReceived { get; set; }
    public bool IsCompleted { get; set; }
    
    // Activities / Timeline
    public List<BookingActivityViewModel> Timeline { get; set; } = new();
}

public class BookingActivityViewModel
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "completed"; // completed, current, pending
}
