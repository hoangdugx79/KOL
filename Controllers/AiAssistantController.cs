using KOL_KOC_TAAA.Services;
using KOL_KOC_TAAA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AiAssistantController : ControllerBase
{
    private readonly IGeminiService _geminiService;
    private readonly KolMarketplaceContext _context;

    public AiAssistantController(IGeminiService geminiService, KolMarketplaceContext context)
    {
        _geminiService = geminiService;
        _context = context;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        if (string.IsNullOrEmpty(request.Message))
            return BadRequest("Message cannot be empty.");

        try
        {
            // Lấy dữ liệu thực tế từ Database
            var dbKols = await _context.KolProfiles.CountAsync();
            var minPrice = await _context.RateCardItems.AnyAsync() ? await _context.RateCardItems.MinAsync(i => i.UnitPrice) : 5_000_000m;
            var avgPrice = await _context.RateCardItems.AnyAsync() ? await _context.RateCardItems.AverageAsync(i => i.UnitPrice) : 20_000_000m;

            // Cộng thêm mock idols
            var mockIdols = MockIdolService.GetMockIdols();
            var totalKols = dbKols + mockIdols.Count;

            // Danh sách idol nổi tiếng theo lĩnh vực
            var idolList = string.Join("\n", mockIdols.Select(m =>
                $"  • {m.FullName} ({m.InfluencerType}, {m.Category}) – {(m.TotalFollowers >= 1_000_000 ? $"{m.TotalFollowers / 1_000_000.0:F1}M" : $"{m.TotalFollowers / 1_000}K")} followers, nền tảng chính: {m.TopPlatform}, giá từ {m.MinBudget:N0} VND"));

            var categories = mockIdols.Select(m => m.Category).Distinct().Select(c => c switch {
                "BeautyFashion" => "Làm đẹp & Thời trang",
                "FoodBeverage"  => "Ẩm thực",
                "Tech"          => "Công nghệ",
                "SportsFitness" => "Thể thao & Fitness",
                "Travel"        => "Du lịch",
                "Gaming"        => "Gaming",
                _ => c
            });

            var systemContext =
                $"- Tổng số Idol trên nền tảng: {totalKols} KOL/KOC nổi tiếng\n" +
                $"- Các lĩnh vực: {string.Join(", ", categories)}\n" +
                $"- Mức giá thấp nhất: {minPrice:N0} VND/chiến dịch\n" +
                $"- Mức giá trung bình: {avgPrice:N0} VND\n" +
                $"- Tính năng: Booking tự động, Thanh toán MoMo, Hợp đồng điện tử, Portfolio cá nhân\n" +
                $"- Danh sách Idol nổi bật:\n{idolList}\n" +
                $"- Liên hệ hỗ trợ: 0965534645";

            var response = await _geminiService.GetChatResponseAsync(request.Message, systemContext);
            return Ok(new { response });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Lỗi khi xử lý yêu cầu AI: " + ex.Message });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = null!;
    }
}
