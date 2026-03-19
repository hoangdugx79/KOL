using System.ComponentModel.DataAnnotations;

namespace KOL_KOC_TAAA.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập Email")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập Email")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất {1} ký tự.")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng xác nhận Mật khẩu")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    public string ConfirmPassword { get; set; } = null!;

    [Required]
    public string Role { get; set; } = "Customer"; // Default is Customer, could be Kol
}
