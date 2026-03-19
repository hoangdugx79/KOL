using KOL_KOC_TAAA.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace KOL_KOC_TAAA.Services
{
    public class MomoService : IMomoService
    {
        private readonly HttpClient _httpClient;
        private readonly MomoOptionModel _options;

        public MomoService(HttpClient httpClient, IOptions<MomoOptionModel> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<MomoCreatePaymentResponse> CreatePaymentAsync(Guid bookingId, string bookingTitle, decimal amount)
        {
            var requestId = Guid.NewGuid().ToString();
            var orderId = bookingId.ToString() + "_" + DateTime.UtcNow.Ticks.ToString().Substring(10);
            
            var request = new MomoCreatePaymentRequest
            {
                partnerCode = _options.PartnerCode,
                requestId = requestId,
                amount = (long)amount,
                orderId = orderId,
                orderInfo = "Thanh toán cho: " + bookingTitle,
                redirectUrl = _options.ReturnUrl,
                ipnUrl = _options.NotifyUrl,
                requestType = "captureWallet",
                extraData = "",
                lang = "vi"
            };

            var rawSignature = $"accessKey={_options.AccessKey}&amount={request.amount}&extraData={request.extraData}&ipnUrl={request.ipnUrl}&orderId={request.orderId}&orderInfo={request.orderInfo}&partnerCode={request.partnerCode}&redirectUrl={request.redirectUrl}&requestId={request.requestId}&requestType={request.requestType}";
            
            request.signature = ComputeHmacSha256(rawSignature, _options.SecretKey);

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_options.BaseUrl + "/v2/gateway/api/create", content);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MomoCreatePaymentResponse>(responseContent)!;
        }

        public bool ValidateSignature(MomoExecuteResponseModel response)
        {
            var rawSignature = $"accessKey={_options.AccessKey}&amount={response.amount}&extraData={response.extraData}&message={response.message}&orderId={response.orderId}&orderInfo={response.orderInfo}&orderType={response.orderType}&partnerCode={response.partnerCode}&payType={response.payType}&requestId={response.requestId}&responseTime={response.responseTime}&resultCode={response.resultCode}&transId={response.transId}";
            
            var computedSignature = ComputeHmacSha256(rawSignature, _options.SecretKey);
            return computedSignature.Equals(response.signature, StringComparison.OrdinalIgnoreCase);
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmacsha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
