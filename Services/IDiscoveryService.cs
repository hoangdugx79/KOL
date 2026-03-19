using System.Collections.Generic;
using System.Threading.Tasks;
using KOL_KOC_TAAA.ViewModels;

namespace KOL_KOC_TAAA.Services;

public interface IDiscoveryService
{
    Task<DiscoveryViewModel> GetDiscoveryResultsAsync(DiscoveryFilterViewModel filters, int page = 1, int pageSize = 12);
    Task<List<PublicKolProfileViewModel>> GetTrendingKolsAsync(int count = 10);
    Task<List<PublicKolProfileViewModel>> GetNewRisingKolsAsync(int count = 10);
}
