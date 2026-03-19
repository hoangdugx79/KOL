using System.ComponentModel.DataAnnotations;

namespace KOL_KOC_TAAA.ViewModels;

public class SubmitReviewViewModel
{
    public Guid BookingId { get; set; }
    public string KolName { get; set; } = null!;

    [Required]
    [Range(1, 5, ErrorMessage = "Vui lòng chọn đánh giá từ 1 đến 5 sao")]
    public int Rating { get; set; }

    [Display(Name = "Nhận xét của bạn")]
    public string? Comment { get; set; }
}
