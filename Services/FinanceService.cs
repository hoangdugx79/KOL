using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Services;

public class FinanceService : IFinanceService
{
    private readonly KolMarketplaceContext _context;

    public FinanceService(KolMarketplaceContext context)
    {
        _context = context;
    }

    public async Task<UserWallet> GetOrCreateWalletAsync(Guid userId)
    {
        var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
        {
            wallet = new UserWallet
            {
                UserId = userId,
                Balance = 0,
                LockedBalance = 0,
                Currency = "VND",
                UpdatedAt = DateTime.UtcNow
            };
            _context.UserWallets.Add(wallet);
            await _context.SaveChangesAsync();
        }
        return wallet;
    }

    public async Task<List<WalletLedger>> GetLedgerHistoryAsync(Guid userId, int limit = 20)
    {
        return await _context.WalletLedgers
            .Where(l => l.WalletUserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<(decimal Total, List<Booking> Bookings)> GetPendingEscrowAsync(Guid kolUserId)
    {
        var escrowStatuses = new[] { "in_progress", "contract_signed", "work_submitted", "approved" };
        var bookings = await _context.Bookings
            .Include(b => b.CustomerUser)
            .Where(b => b.KolUserId == kolUserId && escrowStatuses.Contains(b.Status))
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
        var total = bookings.Sum(b => b.TotalAmount);
        return (total, bookings);
    }

    public async Task<bool> ProcessPaymentAsync(Guid bookingId, Guid customerId, decimal amount)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.KolUser)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null || booking.Status != "approved") return false;

            // Mark as paid (demo - no real payment gateway)
            booking.Status = "paid";
            booking.UpdatedAt = DateTime.UtcNow;

            // Auto-release funds to KOL wallet
            var kolId = booking.KolUserId;
            var wallet = await GetOrCreateWalletAsync(kolId);
            wallet.Balance += amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            _context.WalletLedgers.Add(new WalletLedger
            {
                Id = Guid.NewGuid(),
                WalletUserId = kolId,
                Amount = amount,
                TransactionType = "income",
                Description = $"Thanh toán cho đơn hàng #{bookingId.ToString()[..8].ToUpper()}",
                CreatedAt = DateTime.UtcNow
            });

            booking.Status = "completed"; // Final state after payment + release

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> ReleaseEscrowToKolAsync(Guid bookingId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.KolUser)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            
            if (booking == null || booking.Status != "completed") return false;

            var kolId = booking.KolUserId;
            var amountToRelease = booking.TotalAmount;

            var wallet = await GetOrCreateWalletAsync(kolId);
            wallet.Balance += amountToRelease;
            wallet.UpdatedAt = DateTime.UtcNow;

            _context.WalletLedgers.Add(new WalletLedger
            {
                Id = Guid.NewGuid(),
                WalletUserId = kolId,
                Amount = amountToRelease,
                TransactionType = "income",
                Description = $"Thanh toán cho đơn hàng #{bookingId.ToString()[..8].ToUpper()}",
                CreatedAt = DateTime.UtcNow
            });

            booking.Status = "completed"; // Final state
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<PayoutRequest> RequestPayoutAsync(Guid userId, decimal amount, string bankAccountInfo)
    {
        var wallet = await GetOrCreateWalletAsync(userId);
        if (wallet.Balance < amount) throw new Exception("Insufficient balance");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Lock the amount
            wallet.Balance -= amount;
            wallet.LockedBalance += amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            var request = new PayoutRequest
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = amount,
                Currency = "VND",
                Status = "pending",
                BankInfoJson = bankAccountInfo, // Using this field for simplicity
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PayoutRequests.Add(request);
            
            _context.WalletLedgers.Add(new WalletLedger
            {
                Id = Guid.NewGuid(),
                WalletUserId = userId,
                Amount = -amount,
                TransactionType = "payout",
                Description = "Yêu cầu rút tiền đang chờ duyệt",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return request;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
