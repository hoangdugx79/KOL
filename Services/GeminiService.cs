using System.Text;
using System.Text.Json;

namespace KOL_KOC_TAAA.Services;

public interface IGeminiService
{
    Task<string> GetChatResponseAsync(string userMessage, string? systemContext = null);
}

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string Model = "gemini-3-flash-preview";

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey is missing");
    }

    public async Task<string> GetChatResponseAsync(string userMessage, string? systemContext = null)
    {
        string systemPrompt = @"Bạn là Chuyên gia Trợ lý AI cao cấp của hệ thống KOL Market (Phát triển bởi Nguyen Duc Dung). 
Nhiệm vụ của bạn là hỗ trợ Brand (Nhãn hàng) và KOL/KOC giao dịch hiệu quả trên nền tảng.

KẾ HOẠCH NGHIỆP VỤ HỆ THỐNG:
1. Quy trình chính: Brand tìm KOL -> Xem Profile/Rate Card -> Gửi Booking Request -> Đàm phán qua Chat -> Chấp nhận -> Tạo Hợp đồng -> Thanh toán -> Thực hiện công việc (Deliverables) -> Duyệt bài -> Hoàn thành.
2. Thanh toán: Hỗ trợ thanh toán qua Ví MoMo (Tự động) và Chuyển khoản ngân hàng (Thủ công). Hệ thống giữ tiền trung gian và chỉ trả cho KOL sau khi Brand duyệt sản phẩm (Deliverables).
3. Các module: Dashboard, Deliverables, Wallet, Contracts.
4. Thông tin hỗ trợ: Số điện thoại hotline: 0965534645.

PHONG CÁCH TRẢ LỜI:
- Chuyên nghiệp, am hiểu sâu về Marketing và KOL/KOC.
- Nếu người dùng thắc mắc về con số thực tế hoặc yêu cầu tư vấn, hãy sử dụng dữ liệu THÔNG TIN HỆ THỐNG THỰC TẾ dưới đây (nếu có) để trả lời một cách chính xác nhất.";

        if (!string.IsNullOrEmpty(systemContext))
        {
            systemPrompt += "\n\nTHÔNG TIN HỆ THỐNG THỰC TẾ (DỮ LIỆU ĐỘNG):\n" + systemContext;
        }

        var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={_apiKey}";

        var requestBody = new
        {
            system_instruction = new { parts = new[] { new { text = systemPrompt } } },
            contents = new[]
            {
                new { role = "user", parts = new[] { new { text = userMessage } } }
            },
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 1024
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseContent);
        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "Xin lỗi, tôi không thể xử lý yêu cầu lúc này.";
    }
}
