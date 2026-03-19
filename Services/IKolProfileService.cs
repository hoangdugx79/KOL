using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;

namespace KOL_KOC_TAAA.Services;

public interface IKolProfileService
{
    Task<KolProfile?> GetProfileByUserIdAsync(Guid userId);
    Task<bool> UpdateProfileAsync(Guid userId, KolProfileEditViewModel model);
    Task<List<KolSocialAccount>> GetSocialAccountsAsync(Guid userId);
    Task<bool> AddSocialAccountAsync(Guid userId, KolSocialAccountViewModel model);
    Task<bool> RemoveSocialAccountAsync(Guid userId, Guid socialId);
    Task<List<RateCard>> GetRateCardsAsync(Guid userId);
    Task<bool> CreateRateCardAsync(Guid userId, RateCardViewModel model);
    Task<bool> AddRateCardItemAsync(Guid userId, Guid rateCardId, AddRateCardItemViewModel model);
    Task<bool> DeleteRateCardAsync(Guid userId, Guid rateCardId);
    Task<bool> UpdateRateCardNotesAsync(Guid userId, Guid rateCardId, string? notes);
    Task<StudioDashboardViewModel> GetStudioDashboardAsync(Guid userId);
    
    // Portfolio
    Task<List<KolPortfolio>> GetPortfoliosAsync(Guid userId);
    Task<bool> AddPortfolioAsync(Guid userId, PortfolioItemViewModel model);
    Task<bool> DeletePortfolioAsync(Guid userId, Guid portfolioId);
}
