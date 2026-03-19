using System.ComponentModel.DataAnnotations;

namespace KOL_KOC_TAAA.ViewModels;

public class CreateBookingRequestViewModel
{
    public Guid KolUserId { get; set; }
    public string? KolName { get; set; } // Helper for view
    public List<KOL_KOC_TAAA.Models.RateCard> AvailableServices { get; set; } = new(); // Helper for view

    [Required(ErrorMessage = "Vui lòng nhập tiêu đề chiến dịch")]
    [Display(Name = "Tiêu đề chiến dịch")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mô tả ngắn gọn (Brief)")]
    [Display(Name = "Mô tả công việc (Brief)")]
    public string? Brief { get; set; }

    [Display(Name = "Ngân sách tối thiểu")]
    public decimal? BudgetMin { get; set; }

    [Display(Name = "Ngân sách tối đa")]
    public decimal? BudgetMax { get; set; }

    public string Currency { get; set; } = "VND";

    [Display(Name = "Ngày bắt đầu dự kiến")]
    public DateOnly? ProposedStartDate { get; set; }

    [Display(Name = "Ngày kết thúc dự kiến")]
    public DateOnly? ProposedEndDate { get; set; }

    public List<BookingRequestItemViewModel> Items { get; set; } = new();
}

public class BookingRequestItemViewModel
{
    [Required]
    public string ServiceType { get; set; } = null!;
    
    public string? Platform { get; set; }
    
    [Range(1, 100)]
    public int Quantity { get; set; } = 1;

    public decimal? ExpectedUnitPrice { get; set; }
    
    public string? Notes { get; set; }
}
