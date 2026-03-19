using System.ComponentModel.DataAnnotations;
using KOL_KOC_TAAA.Models;

namespace KOL_KOC_TAAA.ViewModels;

public class KolProfileEditViewModel
{
    [Display(Name = "Tên hiển thị")]
    [StringLength(255)]
    public string? FullName { get; set; }

    [Display(Name = "Ảnh đại diện (URL)")]
    public string? AvatarUrl { get; set; }

    [Display(Name = "Upload ảnh đại diện")]
    public IFormFile? AvatarFile { get; set; }

    [Display(Name = "Email liên hệ công khai")]
    [EmailAddress]
    public string? ContactEmail { get; set; }

    [Required]
    [Display(Name = "Loại KOL (Nano/Micro/Macro...)")]
    [StringLength(10)]
    public string InfluencerType { get; set; } = "Nano";

    [Display(Name = "Giới thiệu bản thân")]
    public string? Bio { get; set; }

    [Display(Name = "Giới tính")]
    [StringLength(20)]
    public string? Gender { get; set; }

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateOnly? Dob { get; set; }

    [Display(Name = "Thành phố")]
    [StringLength(100)]
    public string? LocationCity { get; set; }

    [Display(Name = "Quốc gia")]
    [StringLength(100)]
    public string? LocationCountry { get; set; }

    [Display(Name = "Ngân sách tối thiểu")]
    public decimal? MinBudget { get; set; }
}

public class KolSocialAccountViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "Nền tảng (Platform)")]
    [StringLength(50)]
    public string Platform { get; set; } = null!;

    [Required]
    [Display(Name = "Đường dẫn (URL)")]
    [Url]
    public string Url { get; set; } = null!;

    [Display(Name = "Số lượng người theo dõi (Followers)")]
    public int? FollowersCount { get; set; }
}

public class RateCardViewModel
{
    public Guid? Id { get; set; }
    
    [Required]
    [Display(Name = "Tên Rate Card (Gói dịch vụ)")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Display(Name = "Đơn vị tiền tệ")]
    public string Currency { get; set; } = "VND";

    [Display(Name = "Ghi chú / Mô tả")]
    public string? Notes { get; set; }

    [Display(Name = "Kích hoạt?")]
    public bool IsActive { get; set; } = true;
    
    public List<RateCardItemViewModel> Items { get; set; } = new List<RateCardItemViewModel>();
}

public class AddRateCardItemViewModel
{
    [Required]
    [Display(Name = "Tên hạng mục")]
    [StringLength(50)]
    public string ServiceType { get; set; } = null!;

    [Display(Name = "Nền tảng")]
    [StringLength(50)]
    public string? Platform { get; set; }

    [Required]
    [Display(Name = "Đơn giá")]
    public decimal UnitPrice { get; set; }

    [Display(Name = "Đơn vị")]
    [StringLength(30)]
    public string Unit { get; set; } = "Bài";

    [Display(Name = "Thời gian bàn giao (phút)")]
    public int? DurationMinutes { get; set; }

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }
}

public class RateCardItemViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "Tên hạng mục")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Giá tiền")]
    public decimal Price { get; set; }

    [Display(Name = "Số ngày bàn giao")]
    public int DeliveryDays { get; set; }
}
