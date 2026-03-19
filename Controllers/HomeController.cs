using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.Services;
using KOL_KOC_TAAA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace KOL_KOC_TAAA.Controllers
{
    public class HomeController : Controller
    {
        private readonly KolMarketplaceContext _context;

        public HomeController(KolMarketplaceContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var profiles = await _context.KolProfiles
                .Include(p => p.User)
                .Include(p => p.KolSocialAccounts)
                .Where(p => p.User.Status == "active")
                .Take(24)
                .ToListAsync();

            var mockIdols = MockIdolService.GetMockIdols();

            var allKols = profiles.Select(p =>
            {
                var topAccount = p.KolSocialAccounts.OrderByDescending(s => s.Followers ?? 0).FirstOrDefault();
                var mockMatch = mockIdols.FirstOrDefault(m => m.UserId == p.UserId);

                return new PublicKolProfileViewModel
                {
                    UserId = p.UserId,
                    FullName = p.User.FullName ?? p.User.Email,
                    AvatarUrl = p.User.AvatarUrl ?? mockMatch?.AvatarUrl,
                    Category = mockMatch?.Category ?? "all",
                    InfluencerType = p.InfluencerType,
                    Bio = p.Bio,
                    RatingAvg = p.RatingAvg,
                    RatingCount = p.RatingCount,
                    IsVerified = p.IsVerified,
                    MinBudget = p.MinBudget,
                    TopPlatform = topAccount?.Platform,
                    TotalFollowers = p.KolSocialAccounts.Sum(s => s.Followers ?? 0),
                    Platforms = p.KolSocialAccounts.Select(s => s.Platform).Distinct().ToList()
                };
            }).ToList();

            var viewModel = new HomeIndexViewModel
            {
                FeaturedKols = allKols.Where(k => k.IsVerified).OrderByDescending(k => k.TotalFollowers).Take(8).ToList(),
                TrendingKols = allKols.OrderBy(x => Guid.NewGuid()).Take(10).ToList(),
                RecommendedKols = allKols.Take(12).ToList(),
                Stats = new PlatformStatsViewModel
                {
                    ActiveKols = await _context.KolProfiles.CountAsync() + 150,
                    CompletedCampaigns = 1240,
                    TotalRevenue = 8500000000,
                    TotalReach = "45M+"
                },
                PartnerBrands = new List<BrandLogoViewModel>
                {
                    new BrandLogoViewModel { Name = "Samsung", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/2/24/Samsung_Logo.svg" },
                    new BrandLogoViewModel { Name = "Unilever", LogoUrl = "https://upload.wikimedia.org/wikipedia/en/b/b1/Unilever.svg" },
                    new BrandLogoViewModel { Name = "Shopee", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/f/fe/Shopee.svg" },
                    new BrandLogoViewModel { Name = "Lazada", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/d/d1/Lazada_logo.svg" },
                    new BrandLogoViewModel { Name = "Grab", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/0/03/Grab_logo.svg" }
                },
                CaseStudies = new List<CaseStudyViewModel>
                {
                    new CaseStudyViewModel { BrandName = "Samsung Galaxy S24 launch", CampaignGoal = "Product Launch & Awareness", ResultHighlight = "+230% Engagement", Description = "Kết hợp với 50 KOL Tech & Lifestyle tạo ra làn sóng viral trên TikTok." },
                    new CaseStudyViewModel { BrandName = "Shopee Super Sale", CampaignGoal = "Conversion & Sales", ResultHighlight = "12M+ Impressions", Description = "Chiến dịch KOC quy mô lớn thúc đẩy lượt tải app và mua hàng trực tiếp." }
                },
                Testimonials = new List<TestimonialViewModel>
                {
                    new TestimonialViewModel { Name = "Nguyễn Văn A", Role = "Marketing Manager @ Samsung", Content = "Nền tảng giúp chúng tôi tiết kiệm 70% thời gian tìm kiếm và kết nối với KOL phù hợp." },
                    new TestimonialViewModel { Name = "Lê Thị B", Role = "Founder @ Beauty Brand", Content = "Quy trình thanh toán Escrow và quản lý sản phẩm rất minh bạch và an toàn." }
                }
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Search(string? query, string? type, string? city, string? category)
        {
            var kolsQuery = _context.KolProfiles
                .Include(p => p.User)
                .Where(p => p.User.Status == "active")
                .AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                kolsQuery = kolsQuery.Where(p =>
                    (p.User.FullName != null && p.User.FullName.Contains(query)) ||
                    (p.Bio != null && p.Bio.Contains(query)));
            }

            if (!string.IsNullOrEmpty(type))
            {
                kolsQuery = kolsQuery.Where(p => p.InfluencerType == type);
            }

            if (!string.IsNullOrEmpty(city))
            {
                kolsQuery = kolsQuery.Where(p => p.LocationCity == city);
            }

            var mockIdols = MockIdolService.GetMockIdols();

            var results = await kolsQuery
                .Select(p => new {
                    Profile = p,
                    TotalFollowers = p.KolSocialAccounts.Sum(s => s.Followers ?? 0),
                    Platforms = p.KolSocialAccounts.Select(s => s.Platform).Distinct().ToList()
                })
                .ToListAsync();

            var finalResults = results.Select(r => {
                var mockMatch = mockIdols.FirstOrDefault(m => m.UserId == r.Profile.UserId);
                return new PublicKolProfileViewModel
                {
                    UserId = r.Profile.UserId,
                    FullName = r.Profile.User.FullName ?? r.Profile.User.Email,
                    AvatarUrl = r.Profile.User.AvatarUrl ?? mockMatch?.AvatarUrl,
                    Category = mockMatch?.Category ?? "all",
                    InfluencerType = r.Profile.InfluencerType,
                    LocationCity = r.Profile.LocationCity,
                    RatingAvg = r.Profile.RatingAvg,
                    RatingCount = r.Profile.RatingCount,
                    IsVerified = r.Profile.IsVerified,
                    MinBudget = r.Profile.MinBudget,
                    TotalFollowers = r.TotalFollowers,
                    Platforms = r.Platforms
                };
            });

            if (!string.IsNullOrEmpty(category))
            {
                finalResults = finalResults.Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.Query    = query;
            ViewBag.Type     = type;
            ViewBag.City     = city;
            ViewBag.Category = category;

            return View(finalResults.ToList());
        }

        public async Task<IActionResult> KolDetail(Guid id)
        {
            var p = await _context.KolProfiles
                .Include(p => p.User)
                .Include(p => p.KolSocialAccounts)
                .Include(p => p.KolPortfolios)
                .Include(p => p.RateCards.Where(r => r.IsActive))
                    .ThenInclude(r => r.RateCardItems)
                .FirstOrDefaultAsync(p => p.UserId == id);

            // Nếu không tìm thấy trong DB → thử tìm trong mock data
            if (p == null || p.User.Status != "active")
            {
                var mockIdol = MockIdolService.GetMockIdols().FirstOrDefault(m => m.UserId == id);
                if (mockIdol == null) return NotFound();

                // Gắn SocialAccounts và Portfolios giả từ danh sách Platforms/Category
                mockIdol.SocialAccounts = MockIdolService.BuildSocialAccounts(id, mockIdol.Platforms, mockIdol.TotalFollowers);
                mockIdol.Portfolios     = MockIdolService.BuildMockPortfolios(id, mockIdol.Category);
                mockIdol.LocationCountry = "Việt Nam";
                ViewBag.IsMockDetail = true;
                return View(mockIdol);
            }

            var model = new PublicKolProfileViewModel
            {
                UserId = p.UserId,
                FullName = p.User.FullName ?? p.User.Email,
                AvatarUrl = p.User.AvatarUrl,
                InfluencerType = p.InfluencerType,
                Bio = p.Bio,
                LocationCity = p.LocationCity,
                LocationCountry = p.LocationCountry,
                RatingAvg = p.RatingAvg,
                RatingCount = p.RatingCount,
                IsVerified = p.IsVerified,
                MinBudget = p.MinBudget,
                SocialAccounts = p.KolSocialAccounts.ToList(),
                Portfolios = p.KolPortfolios.ToList(),
                RateCards = p.RateCards.ToList()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
