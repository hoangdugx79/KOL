using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace KOL_KOC_TAAA.Services;

public class ContractService : IContractService
{
    private readonly KolMarketplaceContext _context;
    private readonly INotificationService _notificationService;

    public ContractService(KolMarketplaceContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Contract> CreateDraftContractAsync(Guid bookingId, Guid creatorId)
    {
        var booking = await _context.Bookings
            .Include(b => b.BookingItems)
            .Include(b => b.CustomerUser)
            .Include(b => b.KolUser.User)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null) throw new Exception("Booking not found");

        // Check for existing contracts to determine version
        var latestVersion = await _context.Contracts
            .Where(c => c.BookingId == bookingId)
            .OrderByDescending(c => c.Version)
            .Select(c => c.Version)
            .FirstOrDefaultAsync();

        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Version = latestVersion + 1,
            Title = $"Hợp đồng Dịch vụ #{bookingId.ToString()[..8].ToUpper()} (V{latestVersion + 1})",
            TermsText = GenerateDefaultTerms(booking),
            Status = "draft",
            CreatedByUserId = creatorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        // Notify the other party
        if (booking != null)
        {
            var receiverId = booking.KolUserId == creatorId ? booking.CustomerUserId : booking.KolUserId;
            await _notificationService.SendNotificationAsync(receiverId,
                "contract_created",
                "Hợp đồng mới đã được soạn",
                $"Một dự thảo hợp đồng mới đã được tạo cho đơn hàng #{bookingId.ToString()[..8].ToUpper()}");
        }

        return contract;
    }

    public async Task<Contract?> GetContractAsync(Guid contractId)
    {
        return await _context.Contracts
            .Include(c => c.Booking)
                .ThenInclude(b => b.CustomerUser)
            .Include(c => c.Booking)
                .ThenInclude(b => b.KolUser)
                    .ThenInclude(k => k.User)
            .Include(c => c.ContractSignatures)
                .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(c => c.Id == contractId);
    }

    public async Task<List<Contract>> GetBookingContractsAsync(Guid bookingId)
    {
        return await _context.Contracts
            .Where(c => c.BookingId == bookingId)
            .OrderByDescending(c => c.Version)
            .ToListAsync();
    }

    public async Task<bool> UpdateContractTermsAsync(Guid contractId, string termsText)
    {
        var contract = await _context.Contracts.FindAsync(contractId);
        if (contract == null || contract.Status != "draft") return false;

        contract.TermsText = termsText;
        contract.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SignContractAsync(Guid contractId, Guid userId, string signatureImageOrData)
    {
        var contract = await _context.Contracts
            .Include(c => c.ContractSignatures)
            .FirstOrDefaultAsync(c => c.Id == contractId);

        if (contract == null || (contract.Status != "active" && contract.Status != "draft" && contract.Status != "partially_signed")) return false;

        // Check if user already signed
        if (contract.ContractSignatures.Any(s => s.UserId == userId)) return true;

        int currentSignaturesCount = contract.ContractSignatures.Count;

        var signature = new ContractSignature
        {
            Id = Guid.NewGuid(),
            ContractId = contractId,
            UserId = userId,
            SignatureData = signatureImageOrData,
            SignedAt = DateTime.UtcNow,
            IpAddress = "127.0.0.1" // Placeholder
        };

        _context.ContractSignatures.Add(signature);
        
        // If both parties signed, move to active (simplified logic: check signatures count)
        // Need to know who are the parties. Booking has Customer and KOL.
        var booking = await _context.Bookings.FindAsync(contract.BookingId);
        if (booking != null)
        {
             // Signatures count reaches 2 (Customer + KOL)
             if (currentSignaturesCount + 1 >= 2)
             {
                 contract.Status = "signed";
                 booking.Status = "contract_signed";
             }
             else
             {
                 contract.Status = "partially_signed";
             }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> FinalizeContractAsync(Guid contractId)
    {
        var contract = await _context.Contracts.FindAsync(contractId);
        if (contract == null) return false;

        contract.Status = "active";
        await _context.SaveChangesAsync();
        return true;
    }

    public string GenerateDefaultTerms(Booking booking)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# HỢP ĐỒNG CUNG CẤP DỊCH VỤ QUẢNG CÁO");
        sb.AppendLine($"Ngày tạo: {DateTime.Now:dd/MM/yyyy}");
        sb.AppendLine();
        sb.AppendLine("## 1. CÁC BÊN LIÊN QUAN");
        sb.AppendLine($"- **BÊN A (Nhãn hàng):** {booking.CustomerUser?.FullName}");
        sb.AppendLine($"- **BÊN B (KOL/KOC):** {booking.KolUser?.User?.FullName}");
        sb.AppendLine();
        sb.AppendLine("## 2. NỘI DUNG DỊCH VỤ VÀ THỜI GIAN THỰC HIỆN");
        foreach (var item in booking.BookingItems)
        {
            sb.AppendLine($"- {item.ServiceType} trên {item.Platform}: {item.Quantity} gói - Đơn giá: {item.UnitPrice.ToString("N0")} VND");
        }
        if (booking.BookingRequest?.ProposedStartDate != null && booking.BookingRequest?.ProposedEndDate != null)
        {
            sb.AppendLine();
            sb.AppendLine($"- **Thời gian bắt đầu:** {booking.BookingRequest.ProposedStartDate.Value.ToString("dd/MM/yyyy")}");
            sb.AppendLine($"- **Thời gian kết thúc:** {booking.BookingRequest.ProposedEndDate.Value.ToString("dd/MM/yyyy")}");
        }
        sb.AppendLine();
        sb.AppendLine($"**TỔNG GIÁ TRỊ HỢP ĐỒNG:** {booking.TotalAmount.ToString("N0")} {booking.Currency}");
        sb.AppendLine();
        sb.AppendLine("## 3. ĐIỀU KHOẢN THANH TOÁN");
        sb.AppendLine("- Bên A thanh toán thông qua nền tảng KOL Marketplace.");
        sb.AppendLine("- Nền tảng sẽ giữ tiền tạm ký (Escrow) và giải ngân cho Bên B sau khi hoàn thành.");
        sb.AppendLine();
        sb.AppendLine("## 4. CAM KẾT VÀ BẢO MẬT");
        sb.AppendLine("- Bên B cam kết thực hiện đúng tiến độ và chất lượng như Brief.");
        sb.AppendLine("- Hai bên cam kết bảo mật thông tin chiến dịch.");

        return sb.ToString();
    }
}
