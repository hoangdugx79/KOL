using KOL_KOC_TAAA.Models;

namespace KOL_KOC_TAAA.Services;

public interface IContractService
{
    Task<Contract> CreateDraftContractAsync(Guid bookingId, Guid creatorId);
    Task<Contract?> GetContractAsync(Guid contractId);
    Task<List<Contract>> GetBookingContractsAsync(Guid bookingId);
    
    Task<bool> UpdateContractTermsAsync(Guid contractId, string termsText);
    Task<bool> SignContractAsync(Guid contractId, Guid userId, string signatureImageOrData);
    
    Task<bool> FinalizeContractAsync(Guid contractId);
    
    string GenerateDefaultTerms(Booking booking);
}
