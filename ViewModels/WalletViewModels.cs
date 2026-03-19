namespace KOL_KOC_TAAA.ViewModels;

public class WalletIndexViewModel
{
    public decimal Balance { get; set; }
    public decimal LockedBalance { get; set; }
    public string Currency { get; set; } = "VND";
    public List<WalletTransactionViewModel> Transactions { get; set; } = new();
}

public class WalletTransactionViewModel
{
    public decimal Amount { get; set; }
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class PayoutRequestViewModel
{
    public decimal Amount { get; set; }
    public string BankInfo { get; set; } = null!;
}
