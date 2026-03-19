using KOL_KOC_TAAA.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KOL_KOC_TAAA.Controllers;

public class CaseStudiesController : Controller
{
    public IActionResult Index()
    {
        var caseStudies = GetMockCaseStudies();
        return View(caseStudies);
    }

    public IActionResult Detail(string id)
    {
        var study = GetMockCaseStudies().FirstOrDefault(s => s.BrandName.ToLower().Replace(" ", "-") == id);
        if (study == null) return NotFound();
        return View(study);
    }

    private List<CaseStudyViewModel> GetMockCaseStudies()
    {
        return new List<CaseStudyViewModel>
        {
            new CaseStudyViewModel 
            { 
                BrandName = "Samsung Galaxy S24 launch", 
                CampaignGoal = "Product Launch & Awareness", 
                ResultHighlight = "+230% Engagement", 
                Description = "Kết hợp với 50 KOL Tech & Lifestyle tạo ra làn sóng viral trên TikTok.",
                ImageUrl = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?q=80&w=2070&auto=format&fit=crop"
            },
            new CaseStudyViewModel 
            { 
                BrandName = "Shopee Super Sale", 
                CampaignGoal = "Conversion & Sales", 
                ResultHighlight = "12M+ Impressions", 
                Description = "Chiến dịch KOC quy mô lớn thúc đẩy lượt tải app và mua hàng trực tiếp.",
                ImageUrl = "https://images.unsplash.com/photo-1607082348824-0a96f2a4b9da?q=80&w=2070&auto=format&fit=crop"
            },
            new CaseStudyViewModel 
            { 
                BrandName = "Unilever Green living", 
                CampaignGoal = "Sustainability Awareness", 
                ResultHighlight = "500K+ Organic Shares", 
                Description = "Chiến lược nội dung bền vững lan tỏa thông điệp sống xanh qua các Micro-KOL.",
                ImageUrl = "https://images.unsplash.com/photo-1542601906990-b4d3fb778b09?q=80&w=2013&auto=format&fit=crop"
            }
        };
    }
}
