using System;

namespace KOL_KOC_TAAA.Models
{
    public class MomoCreatePaymentRequest
    {
        public string partnerCode { get; set; } = null!;
        public string requestId { get; set; } = null!;
        public long amount { get; set; }
        public string orderId { get; set; } = null!;
        public string orderInfo { get; set; } = null!;
        public string redirectUrl { get; set; } = null!;
        public string ipnUrl { get; set; } = null!;
        public string requestType { get; set; } = "captureWallet";
        public string extraData { get; set; } = "";
        public string lang { get; set; } = "vi";
        public string signature { get; set; } = null!;
    }

    public class MomoCreatePaymentResponse
    {
        public string partnerCode { get; set; } = null!;
        public string requestId { get; set; } = null!;
        public string orderId { get; set; } = null!;
        public long amount { get; set; }
        public long responseTime { get; set; }
        public string message { get; set; } = null!;
        public int resultCode { get; set; }
        public string payUrl { get; set; } = null!;
        public string deeplink { get; set; } = null!;
        public string qrCodeUrl { get; set; } = null!;
    }

    public class MomoExecuteResponseModel
    {
        public string partnerCode { get; set; } = null!;
        public string orderId { get; set; } = null!;
        public string requestId { get; set; } = null!;
        public long amount { get; set; }
        public string orderInfo { get; set; } = null!;
        public string orderType { get; set; } = null!;
        public string transId { get; set; } = null!;
        public int resultCode { get; set; }
        public string message { get; set; } = null!;
        public string payType { get; set; } = null!;
        public long responseTime { get; set; }
        public string extraData { get; set; } = null!;
        public string signature { get; set; } = null!;
    }

    public class MomoOptionModel
    {
        public string PartnerCode { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
        public string NotifyUrl { get; set; } = null!;
    }
}
