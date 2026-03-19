using System.ComponentModel.DataAnnotations;

namespace KOL_KOC_TAAA.ViewModels;

public class ContractDraftViewModel
{
    public Guid BookingId { get; set; }
    public string KolName { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "VND";

    [Required(ErrorMessage = "Tiêu đề hợp đồng không được để trống")]
    [Display(Name = "Tiêu đề hợp đồng")]
    public string Title { get; set; } = "Hợp đồng cung cấp dịch vụ KOL/KOC";

    [Required(ErrorMessage = "Nội dung điều khoản không được để trống")]
    [Display(Name = "Điều khoản hợp đồng")]
    public string TermsText { get; set; } = null!;
}

public class ContractDetailsViewModel
{
    public Guid ContractId { get; set; }
    public string Title { get; set; } = null!;
    public string TermsText { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public List<SignatureViewModel> Signatures { get; set; } = new();
    public bool CanSign { get; set; }
}

public class SignatureViewModel
{
    public string UserName { get; set; } = null!;
    public DateTime SignedAt { get; set; }
    public string? Role { get; set; }
}
