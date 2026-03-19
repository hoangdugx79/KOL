using System.ComponentModel.DataAnnotations;

namespace KOL_KOC_TAAA.Areas.Admin.ViewModels;

public class KolCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập Email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
    [MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập Họ tên")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn Phân loại KOL")]
    public string InfluencerType { get; set; } = "Nano"; // Nano, Micro, Macro, Mega

    public string? Phone { get; set; }
    
    public string? Bio { get; set; }
    
    public string? LocationCity { get; set; }

    public string? LocationCountry { get; set; }

    public decimal? MinBudget { get; set; }
}

public class KolEditViewModel
{
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập Họ tên")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn Phân loại KOL")]
    public string InfluencerType { get; set; } = "Nano";

    public string? Phone { get; set; }
    
    public string? Bio { get; set; }
    
    public string? LocationCity { get; set; }

    public string? LocationCountry { get; set; }

    public decimal? MinBudget { get; set; }
}
