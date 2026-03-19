using KOL_KOC_TAAA.Models;
using System.Threading.Tasks;

namespace KOL_KOC_TAAA.Services
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponse> CreatePaymentAsync(Guid bookingId, string bookingTitle, decimal amount);
        bool ValidateSignature(MomoExecuteResponseModel response);
    }
}
