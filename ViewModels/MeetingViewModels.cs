using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KOL_KOC_TAAA.ViewModels;

public class KolAvailabilityViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public List<BookingRangeDto> BookingRanges { get; set; } = new();
}

public class BookingRangeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string Status { get; set; } = "";
}

public class CreateMeetingViewModel
{
    [Required]
    public Guid BookingId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tiêu đề cuộc họp")]
    [Display(Name = "Tiêu đề cuộc họp")]
    public string Title { get; set; } = null!;

    [Display(Name = "Nội dung cuộc họp")]
    public string? Agenda { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
    [Display(Name = "Thời gian bắt đầu")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
    [Display(Name = "Thời gian kết thúc")]
    public DateTime EndTime { get; set; }

    [Display(Name = "Hình thức")]
    public string? MeetingType { get; set; } = "online";

    [Display(Name = "Link phòng họp (Zoom/Meet)")]
    public string? MeetingUrl { get; set; }
}
