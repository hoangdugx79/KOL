using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;

namespace KOL_KOC_TAAA.Services;

public interface IFinanceService
{
    // Wallet Management
    Task<UserWallet> GetOrCreateWalletAsync(Guid userId);
    Task<List<WalletLedger>> GetLedgerHistoryAsync(Guid userId, int limit = 20);
    
    // Escrow Info
    Task<(decimal Total, List<Booking> Bookings)> GetPendingEscrowAsync(Guid kolUserId);

    // Payment Handling (Simulated/Escrow)
    Task<bool> ProcessPaymentAsync(Guid bookingId, Guid customerId, decimal amount);
    Task<bool> ReleaseEscrowToKolAsync(Guid bookingId);
    
    // Payout Flow
    Task<PayoutRequest> RequestPayoutAsync(Guid userId, decimal amount, string bankAccountInfo);
}
