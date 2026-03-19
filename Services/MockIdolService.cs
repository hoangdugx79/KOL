using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;

namespace KOL_KOC_TAAA.Services;

/// <summary>
/// Danh sách idol/KOL nổi tiếng Việt Nam trên nhiều lĩnh vực.
/// </summary>
public static class MockIdolService
{
    /// <summary>
    /// Tạo mock portfolio items cá nhân hóa cho từng idol – mỗi người có 6 items riêng biệt.
    /// </summary>
    public static List<KolPortfolio> BuildMockPortfolios(Guid kolUserId, string? category)
    {
        // Ánh xạ userId → danh sách (title, description, pexelsUrl) riêng cho từng idol
        var portfolioMap = new Dictionary<string, (string title, string desc, string img)[]>
        {
            // ── Võ Hà Linh (001) ─────────────────────────────────────────────
            ["001"] = new[]
            {
                ("Test Son 50 Hãng Cùng Lúc",   "Review thần tốc 50 loại son môi từ bình dân đến cao cấp",    "https://images.pexels.com/photos/2533266/pexels-photo-2533266.jpeg?w=600&h=600&fit=crop"),
                ("Skincare 7 Bước Da Hàn",       "Bóc phốt sự thật quy trình 7 bước skincare Hàn Quốc",       "https://images.pexels.com/photos/3762879/pexels-photo-3762879.jpeg?w=600&h=600&fit=crop"),
                ("Collab BST Limited Võ Hà Linh","Ra mắt BST collab độc quyền với thương hiệu nội địa",        "https://images.pexels.com/photos/3735218/pexels-photo-3735218.jpeg?w=600&h=600&fit=crop"),
                ("Sự Thật Về Serum Viral",       "Trung thực 100% về các serum đang gây sốt TikTok",           "https://images.pexels.com/photos/3373716/pexels-photo-3373716.jpeg?w=600&h=600&fit=crop"),
                ("Tutorial Makeup Đi Biển",      "Look đi biển không trôi suốt 8 tiếng – test thực tế",        "https://images.pexels.com/photos/2065195/pexels-photo-2065195.jpeg?w=600&h=600&fit=crop"),
                ("Vlog Đi Sự Kiện Beauty World", "Hậu trường show mỹ phẩm quốc tế tại Bangkok",               "https://images.pexels.com/photos/1300402/pexels-photo-1300402.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Chloe Nguyễn (002) ────────────────────────────────────────────
            ["002"] = new[]
            {
                ("Forbes Under 30 Asia Journey", "Hành trình từ makeup blogger đến Forbes Under 30",           "https://images.pexels.com/photos/2639947/pexels-photo-2639947.jpeg?w=600&h=600&fit=crop"),
                ("Full Glam Night Look",         "Tutorial complete glam look cho tiệc cuối năm",              "https://images.pexels.com/photos/2795743/pexels-photo-2795743.jpeg?w=600&h=600&fit=crop"),
                ("Collab MAC Cosmetics VN",      "Đại sứ thương hiệu MAC Việt Nam – BST Thu Đông 2025",        "https://images.pexels.com/photos/3059609/pexels-photo-3059609.jpeg?w=600&h=600&fit=crop"),
                ("My Studio Tour – NYC",         "Tour phòng makeup studio triệu đô tại New York",             "https://images.pexels.com/photos/2693212/pexels-photo-2693212.jpeg?w=600&h=600&fit=crop"),
                ("Skin Prep Secrets",            "Bí mật chuẩn bị da nền trước khi makeup chuẩn pro",          "https://images.pexels.com/photos/1519088/pexels-photo-1519088.jpeg?w=600&h=600&fit=crop"),
                ("GRWM – Chanel Show Paris",     "Get Ready With Me trước show Chanel Haute Couture Paris",    "https://images.pexels.com/photos/2700898/pexels-photo-2700898.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Changmakeup (003) ─────────────────────────────────────────────
            ["003"] = new[]
            {
                ("Hóa Thân Thành 10 Nhân Vật",  "Transform 10 nhân vật nổi tiếng bằng makeup",                "https://images.pexels.com/photos/1138391/pexels-photo-1138391.jpeg?w=600&h=600&fit=crop"),
                ("Drugstore vs High End Test",   "Bộ makeup 500K vs 5 triệu – ai win?",                        "https://images.pexels.com/photos/2699950/pexels-photo-2699950.jpeg?w=600&h=600&fit=crop"),
                ("SFX Makeup Halloween",         "Special effects makeup kinh dị cực đỉnh Halloween 2025",     "https://images.pexels.com/photos/3379257/pexels-photo-3379257.jpeg?w=600&h=600&fit=crop"),
                ("Makeup Cho Cô Dâu Ngày Cưới",  "Bridal makeup hoàn chỉnh – bền màu 12 tiếng",                "https://images.pexels.com/photos/1038916/pexels-photo-1038916.jpeg?w=600&h=600&fit=crop"),// reuse ok
                ("Unboxing PR Package YSL",      "Mở hộp PR package khổng lồ từ YSL Beauty",                  "https://images.pexels.com/photos/934063/pexels-photo-934063.jpeg?w=600&h=600&fit=crop"),
                ("My Journey 10 Năm Làm đẹp",   "Nhìn lại hành trình 10 năm và bài học đắt giá",             "https://images.pexels.com/photos/1987301/pexels-photo-1987301.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Ninh Tito (004) ───────────────────────────────────────────────
            ["004"] = new[]
            {
                ("Ăn Phở Khắp 63 Tỉnh Thành",   "Hành trình 63 tỉnh tìm tô phở ngon nhất Việt Nam",           "https://images.pexels.com/photos/1640774/pexels-photo-1640774.jpeg?w=600&h=600&fit=crop"),
                ("Bữa Ăn 5 Tô – Mukbang",        "Challenge ăn 5 tô bún bò Huế một mình",                      "https://images.pexels.com/photos/1279330/pexels-photo-1279330.jpeg?w=600&h=600&fit=crop"),
                ("Review Michelin Việt Nam 2025", "Thử tất cả nhà hàng Michelin Guide Vietnam 2025",            "https://images.pexels.com/photos/958545/pexels-photo-958545.jpeg?w=600&h=600&fit=crop"),
                ("Street Food Sài Gòn Lúc 3AM",  "Khám phá ẩm thực đêm khuya Sài Gòn không ai biết",          "https://images.pexels.com/photos/1109197/pexels-photo-1109197.jpeg?w=600&h=600&fit=crop"),
                ("Collab Haidilao Vietnam",       "Lần đầu ăn lẩu Haidilao với phục vụ đặc biệt",             "https://images.pexels.com/photos/2116094/pexels-photo-2116094.jpeg?w=600&h=600&fit=crop"),
                ("Bánh Mì Tour Hội An",           "Thử 15 tiệm bánh mì nổi tiếng nhất Hội An",                 "https://images.pexels.com/photos/1435735/pexels-photo-1435735.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Ẩm Thực Mẹ Làm (005) ─────────────────────────────────────────
            ["005"] = new[]
            {
                ("Canh Chua Cá Lóc Miền Nam",    "Công thức canh chua chuẩn vị Nam Bộ từ mẹ dạy",             "https://images.pexels.com/photos/699953/pexels-photo-699953.jpeg?w=600&h=600&fit=crop"),
                ("Tết Nhà Mình Nấu Gì",          "Mâm cỗ Tết 15 món truyền thống cúng ông bà",                "https://images.pexels.com/photos/1640773/pexels-photo-1640773.jpeg?w=600&h=600&fit=crop"),
                ("100 Triệu Views Bánh Xèo",     "Video bánh xèo đạt 100 triệu views – công thức gốc",        "https://images.pexels.com/photos/1279330/pexels-photo-1279330.jpeg?w=600&h=600&fit=crop"),
                ("Cơm Bình Dân Ngon Ơi Là Ngon", "Loạt món cơm văn phòng đơn giản mà ngon bất ngờ",           "https://images.pexels.com/photos/1640774/pexels-photo-1640774.jpeg?w=600&h=600&fit=crop"),
                ("Dưa Cải – Dưa Muối Gia Truyền","Cách muối dưa không bao giờ bị nhớt, chua ngon đều",        "https://images.pexels.com/photos/1435735/pexels-photo-1435735.jpeg?w=600&h=600&fit=crop"),
                ("Nước Chấm Vạn Năng",           "1 nước chấm dùng cho 50 món – bí quyết gia đình",           "https://images.pexels.com/photos/2116094/pexels-photo-2116094.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Dương Quốc Nam (006) ──────────────────────────────────────────
            ["006"] = new[]
            {
                ("Ăn Sáng Rẻ Bèo Sài Gòn",      "10 quán ăn sáng dưới 30k ngon nhất Sài Gòn",                "https://images.pexels.com/photos/1109197/pexels-photo-1109197.jpeg?w=600&h=600&fit=crop"),
                ("Quán Bụi 10 Năm Không Đổi",   "Hẻm nhỏ Sài Gòn – quán ăn kiệt cùng thời gian",            "https://images.pexels.com/photos/958545/pexels-photo-958545.jpeg?w=600&h=600&fit=crop"),
                ("Buffet Hải Sản 200K",          "Review buffet hải sản 200k – có đáng tiền không?",           "https://images.pexels.com/photos/699953/pexels-photo-699953.jpeg?w=600&h=600&fit=crop"),
                ("Thử Ăn Theo Foodtok 1 Tuần",   "7 ngày ăn theo trend TikTok Food và đây là kết quả",        "https://images.pexels.com/photos/1640773/pexels-photo-1640773.jpeg?w=600&h=600&fit=crop"),
                ("Bún Riêu Bà Già Đầu Hẻm",     "Ký sự quán bún riêu 40 năm không đổi công thức",            "https://images.pexels.com/photos/2116094/pexels-photo-2116094.jpeg?w=600&h=600&fit=crop"),
                ("Lẩu Thái 100K Có Ngon?",       "Challenge ăn lẩu Thái giá rẻ vs đắt tiền",                  "https://images.pexels.com/photos/1279330/pexels-photo-1279330.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Vinh Vật Vờ (007) ─────────────────────────────────────────────
            ["007"] = new[]
            {
                ("Đập Hộp iPhone 16 Pro Max VN", "Mở hộp iPhone 16 Pro Max bản Việt Nam ngày đầu ra mắt",     "https://images.pexels.com/photos/788946/pexels-photo-788946.jpeg?w=600&h=600&fit=crop"),
                ("Điện Thoại 2 Triệu Dùng Tốt?", "Thách thức: sống sót 1 tháng với điện thoại 2 triệu",       "https://images.pexels.com/photos/404280/pexels-photo-404280.jpeg?w=600&h=600&fit=crop"),
                ("Setup Chiến Gaming PC Mới",     "Vật Vờ khoe setup PC gaming toàn RGB cực đỉnh",             "https://images.pexels.com/photos/2582937/pexels-photo-2582937.jpeg?w=600&h=600&fit=crop"),
                ("MacBook Pro M4 Đáng Mua?",     "Review MacBook Pro M4 sau 1 tháng dùng thực tế",            "https://images.pexels.com/photos/18105/pexels-photo.jpg?w=600&h=600&fit=crop"),
                ("SmartWatch Nào Ngon Tầm 3 Tr", "Top 5 smartwatch tốt nhất dưới 3 triệu 2025",              "https://images.pexels.com/photos/1714208/pexels-photo-1714208.jpeg?w=600&h=600&fit=crop"),
                ("Tai Nghe ANC Đỉnh Nhất 2025",  "So sánh 6 tai nghe ANC bán chạy nhất năm 2025",            "https://images.pexels.com/photos/3945667/pexels-photo-3945667.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Duy Thẩm (008) ────────────────────────────────────────────────
            ["008"] = new[]
            {
                ("Laptop Gaming 20 Triệu 2025",  "Top 3 laptop gaming tốt nhất tầm 20 triệu – test thực tế",  "https://images.pexels.com/photos/18105/pexels-photo.jpg?w=600&h=600&fit=crop"),
                ("Build PC Tầm 15 Triệu",        "Hướng dẫn build PC gaming/đồ họa 15 triệu chi tiết nhất",   "https://images.pexels.com/photos/2582937/pexels-photo-2582937.jpeg?w=600&h=600&fit=crop"),
                ("Monitor 2K vs 4K Dân Văn Phòng","Chọn màn hình nào cho dân văn phòng và creator?",          "https://images.pexels.com/photos/1714208/pexels-photo-1714208.jpeg?w=600&h=600&fit=crop"),
                ("RAM vs SSD: Nên Nâng Cái Nào", "Khoa học máy tính: nâng RAM hay SSD mang lại hiệu năng hơn?","https://images.pexels.com/photos/1779487/pexels-photo-1779487.jpeg?w=600&h=600&fit=crop"),
                ("Bộ Phụ Kiện Bàn Làm Việc 5Tr", "Review bộ phụ kiện keyboard+mouse+mousepad tầm 5 triệu",    "https://images.pexels.com/photos/3945667/pexels-photo-3945667.jpeg?w=600&h=600&fit=crop"),
                ("Review ASUS ROG Flow X13",     "Laptop 2-in-1 gaming đỉnh của đỉnh năm 2025",               "https://images.pexels.com/photos/5082579/pexels-photo-5082579.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Tài Xài Tech (009) ────────────────────────────────────────────
            ["009"] = new[]
            {
                ("Điện Thoại Tầm Giá 5 Triệu",  "Top pick điện thoại 5 triệu ngon nhất tháng này",            "https://images.pexels.com/photos/404280/pexels-photo-404280.jpeg?w=600&h=600&fit=crop"),
                ("Laptop SV Dưới 10 Triệu",     "Laptop sinh viên chiến game lẫn học tốt dưới 10 triệu",      "https://images.pexels.com/photos/18105/pexels-photo.jpg?w=600&h=600&fit=crop"),
                ("So Sánh Xiaomi Redmi vs POCO", "Battle Redmi Note 14 vs POCO X7 – ai hơn?",                 "https://images.pexels.com/photos/788946/pexels-photo-788946.jpeg?w=600&h=600&fit=crop"),
                ("Tai Nghe TWS Dưới 500K",       "Tai nghe true wireless ngon nhất dưới 500 nghìn",            "https://images.pexels.com/photos/3945667/pexels-photo-3945667.jpeg?w=600&h=600&fit=crop"),
                ("Điện Thoại Cũ Có Đáng Mua?",  "iPhone 14 Pro cũ vs Samsung S24 mới cùng giá – chọn gì?",   "https://images.pexels.com/photos/1300402/pexels-photo-1300402.jpeg?w=600&h=600&fit=crop"),
                ("Loa Bluetooth Để Bàn 1 Triệu", "Top 3 loa bluetooth để bàn tốt nhất tầm 1 triệu 2025",      "https://images.pexels.com/photos/1714208/pexels-photo-1714208.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Quang Hải (010) ───────────────────────────────────────────────
            ["010"] = new[]
            {
                ("Hành Trình Sang Pháp Đá Bóng", "Vlog ngày đầu gia nhập CLB bóng đá Pháp của Quang Hải",     "https://images.pexels.com/photos/46798/the-ball-stadion-football-the-pitch-46798.jpeg?w=600&h=600&fit=crop"),
                ("Training Day – Trước Trận Đấu","Hậu trường buổi tập cường độ cao trước trận ASEAN Cup",     "https://images.pexels.com/photos/1552249/pexels-photo-1552249.jpeg?w=600&h=600&fit=crop"),
                ("Hỏi Đáp Cùng Quang Hải",       "Trả lời 50 câu hỏi từ fan về bóng đá và cuộc sống riêng",   "https://images.pexels.com/photos/1199590/pexels-photo-1199590.jpeg?w=600&h=600&fit=crop"),
                ("Collab Nike Vietnam 2025",      "Chiến dịch quảng cáo Nike – Dám Ước Mơ – Dám Chiến Đấu",   "https://images.pexels.com/photos/2529148/pexels-photo-2529148.jpeg?w=600&h=600&fit=crop"),
                ("Bữa Ăn Dinh Dưỡng Cầu Thủ",   "Một ngày ăn uống của cầu thủ chuyên nghiệp – meal plan",    "https://images.pexels.com/photos/1640777/pexels-photo-1640777.jpeg?w=600&h=600&fit=crop"),
                ("Freestyle Football Challenge",  "Thử thách kỹ thuật bóng đá đường phố cùng fan",            "https://images.pexels.com/photos/159616/football-american-football-runner-player-159616.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Tống Huy Hoàng (011) ──────────────────────────────────────────
            ["011"] = new[]
            {
                ("Full Week Training Vlog",      "7 ngày tập gym theo giáo án HLV chuyên nghiệp",              "https://images.pexels.com/photos/1552249/pexels-photo-1552249.jpeg?w=600&h=600&fit=crop"),
                ("Cách Tăng 5kg Cơ Đúng Cách",  "Hướng dẫn tăng cơ khoa học không tăng mỡ trong 3 tháng",    "https://images.pexels.com/photos/841130/pexels-photo-841130.jpeg?w=600&h=600&fit=crop"),
                ("Review Gym Ở Hà Nội – 5 Cái",  "Thử 5 phòng gym lớn nhất Hà Nội – đâu xứng đáng tiền hơn?", "https://images.pexels.com/photos/1673130/pexels-photo-1673130.jpeg?w=600&h=600&fit=crop"),
                ("Á Quân Thể Hình Chia Sẻ",     "Hành trình từ tay mơ đến Á quân thể hình toàn quốc",        "https://images.pexels.com/photos/3076516/pexels-photo-3076516.jpeg?w=600&h=600&fit=crop"),
                ("Protein Shake Tự Làm Tại Nhà", "Công thức 5 protein shake không cần mua thêm supplement",   "https://images.pexels.com/photos/1640777/pexels-photo-1640777.jpeg?w=600&h=600&fit=crop"),
                ("Upper Body Day – Chest Focus",  "Bài tập ngực cho người mới bắt đầu – kỹ thuật chuẩn",       "https://images.pexels.com/photos/3822906/pexels-photo-3822906.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Thùy Tiên (012) ───────────────────────────────────────────────
            ["012"] = new[]
            {
                ("Miss Grand International 2021",  "Hành trình từ TP.HCM đến vương miện Miss Grand quốc tế",       "https://images.pexels.com/photos/1462637/pexels-photo-1462637.jpeg?w=600&h=600&fit=crop"),
                ("Workout Routine Mỗi Sáng",      "Bài tập 30 phút mỗi sáng giúp Thùy Tiên giữ vóc dáng",       "https://images.pexels.com/photos/3822906/pexels-photo-3822906.jpeg?w=600&h=600&fit=crop"),
                ("Collab L'Oréal Paris VN 2025",  "Hình ảnh đại sứ thương hiệu L'Oréal Paris Việt Nam",          "https://images.pexels.com/photos/3373716/pexels-photo-3373716.jpeg?w=600&h=600&fit=crop"),
                ("Skincare Hàng Ngày Của Tôi",    "Quy trình chăm sóc da zero-waste của hoa hậu Thùy Tiên",      "https://images.pexels.com/photos/3762879/pexels-photo-3762879.jpeg?w=600&h=600&fit=crop"),
                ("Thiện Nguyện – Gieo Yêu Thương","Dự án cộng đồng mang trường học đến vùng cao Việt Nam",       "https://images.pexels.com/photos/1499327/pexels-photo-1499327.jpeg?w=600&h=600&fit=crop"),
                ("Q&A: Một Ngày Của Hoa Hậu",     "24 giờ trong đời Thùy Tiên – từ tập luyện đến sự kiện",      "https://images.pexels.com/photos/1462637/pexels-photo-1462637.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Khoai Lang Thang (013) ────────────────────────────────────────
            ["013"] = new[]
            {
                ("Ăn Đường Phố 60 Quốc Gia",     "Tổng hợp best street food từ 60 quốc gia đã đến",           "https://images.pexels.com/photos/2161467/pexels-photo-2161467.jpeg?w=600&h=600&fit=crop"),
                ("Đi Nhật 7 Ngày 15 Triệu",      "Itinerary Nhật Bản tiết kiệm tuyệt đối – mọi tips & tricks", "https://images.pexels.com/photos/2614818/pexels-photo-2614818.jpeg?w=600&h=600&fit=crop"),
                ("Phượt Tây Tạng Bằng Xe Máy",  "Hành trình vượt Tây Tạng 2000km bằng xe máy",              "https://images.pexels.com/photos/1366919/pexels-photo-1366919.jpeg?w=600&h=600&fit=crop"),
                ("Homestay 100K/Đêm Châu Âu",   "Những homestay rẻ không tưởng ở Paris, Rome, Barcelona",    "https://images.pexels.com/photos/2675268/pexels-photo-2675268.jpeg?w=600&h=600&fit=crop"),
                ("Backpack Đông Nam Á 30 Ngày",  "Myanmar, Lào, Campuchia – hành trình 30 ngày 30 triệu",      "https://images.pexels.com/photos/1591361/pexels-photo-1591361.jpeg?w=600&h=600&fit=crop"),
                ("Khám Phá Mù Cang Chải Mùa Vàng","Checklist đi Mù Cang Chải mùa lúa chín đẹp nhất",         "https://images.pexels.com/photos/1774551/pexels-photo-1774551.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Giang Ơi (014) ────────────────────────────────────────────────
            ["014"] = new[]
            {
                ("Self Development – Sách Tôi Đọc","Top 10 cuốn sách thay đổi cuộc đời tôi năm 2025",          "https://images.pexels.com/photos/1591056/pexels-photo-1591056.jpeg?w=600&h=600&fit=crop"),
                ("Du Lịch Solo Châu Âu 1 Mình",  "Cô gái đi một mình 15 nước Châu Âu – hành trình empowering","https://images.pexels.com/photos/2675268/pexels-photo-2675268.jpeg?w=600&h=600&fit=crop"),
                ("Morning Routine Của Tôi",       "Buổi sáng 5AM – bí quyết productivity của Giang Ơi",        "https://images.pexels.com/photos/1591361/pexels-photo-1591361.jpeg?w=600&h=600&fit=crop"),
                ("Cafe Đẹp Để Làm Việc – Hà Nội", "Top 7 quán cafe aesthetic nhất Hà Nội cho WFH",             "https://images.pexels.com/photos/1166648/pexels-photo-1166648.jpeg?w=600&h=600&fit=crop"),
                ("Vlog Kyoto – Mùa Lá Đỏ",       "3 ngày tại Kyoto mùa momiji – bình yên nhất đời",           "https://images.pexels.com/photos/2614818/pexels-photo-2614818.jpeg?w=600&h=600&fit=crop"),
                ("Q&A: Giang Ơi Kể Thật",        "Trả lời thật 100% câu hỏi về sự nghiệp, tình cảm, cuộc sống","https://images.pexels.com/photos/2161467/pexels-photo-2161467.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Châu Bùi (015) ────────────────────────────────────────────────
            ["015"] = new[]
            {
                ("Collab Chanel – Paris Show",    "Hậu trường Châu Bùi tại Fashion Week Paris với Chanel",      "https://images.pexels.com/photos/934063/pexels-photo-934063.jpeg?w=600&h=600&fit=crop"),
                ("OOTD Street Style Hà Nội",     "Bộ ảnh đường phố Hà Nội – mix đồ vintage & high fashion",    "https://images.pexels.com/photos/2065195/pexels-photo-2065195.jpeg?w=600&h=600&fit=crop"),
                ("Vlog Seoul Fashion Week 2025", "4 ngày dự Seoul Fashion Week cùng Dior và Loewe",            "https://images.pexels.com/photos/2614818/pexels-photo-2614818.jpeg?w=600&h=600&fit=crop"),
                ("Louis Vuitton Campaign",       "Chiến dịch hình ảnh cùng Louis Vuitton – biểu tượng thời trang","https://images.pexels.com/photos/1770064/pexels-photo-1770064.jpeg?w=600&h=600&fit=crop"),
                ("Unboxing Luxury Haul 2025",    "Mở hộp BST luxury: Chanel, LV, Gucci một lúc",              "https://images.pexels.com/photos/3735218/pexels-photo-3735218.jpeg?w=600&h=600&fit=crop"),
                ("Sustainable Fashion Journey", "Châu Bùi và hành trình thời trang bền vững, có trách nhiệm", "https://images.pexels.com/photos/1462637/pexels-photo-1462637.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Độ Mixi (016) ─────────────────────────────────────────────────
            ["016"] = new[]
            {
                ("PUBG Mobile – 20 Kill Solo",    "Gameplay PUBG solo 20 kill – khoảnh khắc lịch sử kênh",     "https://images.pexels.com/photos/3165335/pexels-photo-3165335.jpeg?w=600&h=600&fit=crop"),
                ("Setup Stream 2025 – Reveal",   "Reveal setup triệu đô của Độ Mixi sau 2 năm giấu",          "https://images.pexels.com/photos/2582937/pexels-photo-2582937.jpeg?w=600&h=600&fit=crop"),
                ("10 Triệu Subs – Hành Trình",   "Nhìn lại từ 0 đến 10 triệu subscribers – cảm ơn cộng đồng", "https://images.pexels.com/photos/1174746/pexels-photo-1174746.jpeg?w=600&h=600&fit=crop"),
                ("Vlog 3 Con Và Game",            "Cân bằng cuộc sống gia đình 3 con và streamer triệu subs",  "https://images.pexels.com/photos/1779487/pexels-photo-1779487.jpeg?w=600&h=600&fit=crop"),
                ("Tournament Hosting – 500Tr",   "Tổ chức giải đấu PUBG 500 triệu đồng cho cộng đồng",        "https://images.pexels.com/photos/442576/pexels-photo-442576.jpeg?w=600&h=600&fit=crop"),
                ("Collab Samsung Gaming 2025",   "Chiến dịch Samsung Galaxy – Gaming Without Limits",         "https://images.pexels.com/photos/1038916/pexels-photo-1038916.jpeg?w=600&h=600&fit=crop"),
            },
            // ── ViruSs (017) ──────────────────────────────────────────────────
            ["017"] = new[]
            {
                ("Reaction MV Sơn Tùng MTP",    "React MV \"Waiting For You\" ngay lúc ra mắt – triệu view",  "https://images.pexels.com/photos/442576/pexels-photo-442576.jpeg?w=600&h=600&fit=crop"),
                ("ViruSs – Ca Sĩ Thật Sự",      "MV ca nhạc chính thức \"Chờ Người\" – ViruSs hát thật",      "https://images.pexels.com/photos/1174746/pexels-photo-1174746.jpeg?w=600&h=600&fit=crop"),
                ("LMHT – Thách Đấu Với Pro",     "1v1 với tuyển thủ LMHT chuyên nghiệp Việt Nam",              "https://images.pexels.com/photos/3165335/pexels-photo-3165335.jpeg?w=600&h=600&fit=crop"),
                ("Gameshow Siêu Trí Tuệ Việt",   "Tham gia gameshow IQ – ViruSs thể hiện bất ngờ",             "https://images.pexels.com/photos/1779487/pexels-photo-1779487.jpeg?w=600&h=600&fit=crop"),
                ("5 Triệu Subs Party Vlog",      "Buổi tiệc hoành tráng mừng 5 triệu subscribers",            "https://images.pexels.com/photos/1038916/pexels-photo-1038916.jpeg?w=600&h=600&fit=crop"),
                ("Sự Thật Về ViruSs – Phỏng Vấn","Câu chuyện từ streamer bình thường đến hiện tượng triệu view","https://images.pexels.com/photos/2582937/pexels-photo-2582937.jpeg?w=600&h=600&fit=crop"),
            },
            // ── Thầy Giáo Ba (018) ────────────────────────────────────────────
            ["018"] = new[]
            {
                ("Phân Tích Meta LMHT Mùa 15",   "Breakdown toàn bộ meta rank Thách Đấu mùa 15 chi tiết",      "https://images.pexels.com/photos/1174746/pexels-photo-1174746.jpeg?w=600&h=600&fit=crop"),
                ("Coaching Session Miễn Phí",    "1 giờ coaching live với 3 học viên rank Kim Cương – Thách Đấu","https://images.pexels.com/photos/3165335/pexels-photo-3165335.jpeg?w=600&h=600&fit=crop"),
                ("Top 10 Highlight Tháng 3/25",  "Nhữngpha xử lý đỉnh nhất làng LMHT Việt tháng 3/2025",      "https://images.pexels.com/photos/442576/pexels-photo-442576.jpeg?w=600&h=600&fit=crop"),
                ("Review Chuột Gaming Chơi LMHT","Top 3 chuột gaming phù hợp nhất cho game thủ LMHT 2025",     "https://images.pexels.com/photos/2582937/pexels-photo-2582937.jpeg?w=600&h=600&fit=crop"),
                ("Thử Thách Leo Rank 1 Ngày",    "Từ Kim Cương lên Thách Đấu trong 24 giờ – có thể không?",    "https://images.pexels.com/photos/1779487/pexels-photo-1779487.jpeg?w=600&h=600&fit=crop"),
                ("Collab Garena VN – Giải Đấu",  "Hợp tác Garena tổ chức giải LMHT community server Việt Nam", "https://images.pexels.com/photos/1038916/pexels-photo-1038916.jpeg?w=600&h=600&fit=crop"),
            },
        };

        // Tìm key từ userId (2 ký tự cuối của segment cuối)
        var idStr = kolUserId.ToString(); // "00000000-0000-0000-0000-000000000001"
        var lastSeg = idStr.Split('-').Last(); // "000000000001"
        var key = int.TryParse(lastSeg, out var idx) && idx >= 1 && idx <= 18
            ? idx.ToString("D3")
            : null;

        var items = (key != null && portfolioMap.TryGetValue(key, out var val))
            ? val
            : Array.Empty<(string, string, string)>();

        return items.Select((item, i) => new KolPortfolio
        {
            Id          = Guid.NewGuid(),
            KolUserId   = kolUserId,
            Title       = item.Item1,
            Description = item.Item2,
            MediaUrl    = item.Item3,
            CreatedAt   = DateTime.UtcNow.AddDays(-(60 - i * 10)),
        }).ToList();
    }


    public static List<KolSocialAccount> BuildSocialAccounts(Guid kolUserId, List<string> platforms, long? totalFollowers)
    {
        if (platforms == null || platforms.Count == 0) return new();

        // Phân bổ followers theo tỷ lệ cho từng platform
        long total = totalFollowers ?? 100_000;
        var weights = new Dictionary<string, double>
        {
            ["YouTube"]   = 0.40,
            ["TikTok"]    = 0.35,
            ["Instagram"] = 0.15,
            ["Facebook"]  = 0.08,
            ["Twitter"]   = 0.02,
            ["X"]         = 0.02,
        };

        var accounts = new List<KolSocialAccount>();
        double remaining = 1.0;
        for (int i = 0; i < platforms.Count; i++)
        {
            var plat = platforms[i];
            double weight = (i == platforms.Count - 1)
                ? remaining
                : (weights.TryGetValue(plat, out var w) ? w : 0.1);
            remaining -= weight;

            accounts.Add(new KolSocialAccount
            {
                Id          = Guid.NewGuid(),
                KolUserId   = kolUserId,
                Platform    = plat,
                Username    = "@idol_demo",
                ProfileUrl  = $"https://{plat.ToLower()}.com",
                Followers   = (long)(total * weight),
                EngagementRate = 4.2m + (i * 0.3m),
                IsVerified  = i == 0,
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow
            });
        }
        return accounts;
    }

    public static List<PublicKolProfileViewModel> GetMockIdols()
    {
        return new List<PublicKolProfileViewModel>
        {
            // ─── Làm đẹp & Thời trang ───────────────────────────────────────
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                FullName       = "Võ Hà Linh",
                AvatarUrl      = "https://yt3.googleusercontent.com/ytc/AIdro_lDJWGmW3qdpoh0AKz0zGEzkwKvqM9KthGfhi3fZgT66Ls=s400-c-k-c0x00ffffff-no-rj",
                InfluencerType = "Mega",
                Bio            = "Beauty KOL hàng đầu Việt Nam, chuyên review mỹ phẩm trung thực. Nổi tiếng với slogan \"Trung thực là thương hiệu\".",
                LocationCity   = "Hà Nội",
                RatingAvg      = 4.9m, RatingCount = 1240,
                IsVerified     = true,  MinBudget = 30_000_000,
                TotalFollowers = 3_800_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "TikTok", "Instagram", "Facebook" },
                Category       = "BeautyFashion",
                ContactEmail   = "vohalinhmedia@gmail.com",
                ContactPhone   = "0912 345 678",
                AvgResponseTime = "Trong 1 giờ",
                CompletedCampaigns = 156,
                CompletionRate = 99.2m
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                FullName       = "Chloe Nguyễn",
                AvatarUrl      = "https://yt3.googleusercontent.com/ytc/AIdro_k1rGK0MkQwNSIequknZ7b8WabTnjqvEfN1Stb7yGHmQlA=s400-c-k-c0x00ffffff-no-rj",
                InfluencerType = "Mega",
                Bio            = "Makeup artist và beauty vlogger nổi tiếng, có mặt trên Forbes Under 30 châu Á. Chuyên về các kỹ thuật trang điểm đỉnh cao.",
                LocationCity   = "TP. Hồ Chí Minh",
                RatingAvg      = 4.9m, RatingCount = 980,
                IsVerified     = true,  MinBudget = 25_000_000,
                TotalFollowers = 2_500_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Instagram", "TikTok" },
                Category       = "BeautyFashion",
                ContactEmail   = "chloenguyen.collab@gmail.com",
                ContactPhone   = "0903 111 222",
                AvgResponseTime = "Trong 3 giờ",
                CompletedCampaigns = 89,
                CompletionRate = 98.5m
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                FullName       = "Changmakeup",
                AvatarUrl      = "https://yt3.googleusercontent.com/ytc/AIdro_kWOVxiZqEDJJbSHD9aRzSQAnT0AM7cjk7MlNynjtHDzw=s400-c-k-c0x00ffffff-no-rj",
                InfluencerType = "Macro",
                Bio            = "Trần Thu Chang – Beauty YouTuber, chuyên tutorial makeup sáng tạo và review mỹ phẩm cao cấp lẫn bình dân.",
                LocationCity   = "TP. Hồ Chí Minh",
                RatingAvg      = 4.8m, RatingCount = 720,
                IsVerified     = true,  MinBudget = 15_000_000,
                TotalFollowers = 1_600_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Instagram", "TikTok" },
                Category       = "BeautyFashion",
                ContactEmail   = "changmakeup.booking@gmail.com",
                ContactPhone   = "0977 654 321"
            },

            // ─── Ẩm thực ────────────────────────────────────────────────────
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                FullName       = "Ninh Tito",
                AvatarUrl      = "https://yt3.googleusercontent.com/WEHCMR1Ri0GwRRsUJjvkNL65xbMmP9E82StTxx81p07hAIafEtYYFtEHjyenUEW3iK1BcF6X=s400-c-k-c0x00ffffff-no-rj",
                InfluencerType = "Mega",
                Bio            = "Food reviewer nổi tiếng nhất Việt Nam với hàng nghìn video review ẩm thực đường phố. Kênh YouTube 3M+ subscribers.",
                LocationCity   = "Hà Nội",
                RatingAvg      = 4.9m, RatingCount = 1500,
                IsVerified     = true,  MinBudget = 20_000_000,
                TotalFollowers = 4_200_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Facebook", "TikTok" },
                Category       = "FoodBeverage",
                ContactEmail   = "ninhtito.media@gmail.com",
                ContactPhone   = "0934 567 890",
                AvgResponseTime = "Trong 2 giờ",
                CompletedCampaigns = 210,
                CompletionRate = 100
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                FullName       = "Ẩm Thực Mẹ Làm",
                AvatarUrl      = "https://i.pravatar.cc/400?img=45",
                InfluencerType = "Mega",
                Bio            = "Kênh nấu ăn gia đình lớn nhất Việt Nam. Hàng triệu người theo dõi công thức nấu món ngon chuẩn vị Việt hàng ngày.",
                LocationCity   = "TP. Hồ Chí Minh",
                RatingAvg      = 4.9m, RatingCount = 2200,
                IsVerified     = true,  MinBudget = 18_000_000,
                TotalFollowers = 5_800_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Facebook" },
                Category       = "FoodBeverage",
                ContactEmail   = "amthucmelam.booking@gmail.com",
                ContactPhone   = "0985 222 333"
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                FullName       = "Dương Quốc Nam (Foodngon)",
                AvatarUrl      = "https://i.pravatar.cc/400?img=17",
                InfluencerType = "Macro",
                Bio            = "Food vlogger chuyên khám phá ẩm thực đường phố Sài Gòn và các tỉnh miền Nam. Nổi tiếng với phong cách hài hước, gần gũi.",
                LocationCity   = "TP. Hồ Chí Minh",
                RatingAvg      = 4.8m, RatingCount = 650,
                IsVerified     = true,  MinBudget = 10_000_000,
                TotalFollowers = 1_100_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Facebook", "TikTok" },
                Category       = "FoodBeverage",
                ContactEmail   = "foodngon.collab@gmail.com",
                ContactPhone   = "0901 888 999"
            },

            // ─── Công nghệ ──────────────────────────────────────────────────
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000007"),
                FullName       = "Vinh Vật Vờ",
                AvatarUrl      = "https://yt3.ggpht.com/mVmjJ__Y4AORgEn5oWqaRrrpXSiC6b7J8krtBIQ08eHlcWeN-Hd1Zf-ENWcV5Tde6_e1LBqoZA=s400-c-k-c0x00ffffff-no-rj-mo",
                InfluencerType = "Mega",
                Bio            = "Vật Vờ Studio – kênh review công nghệ hài hước, gần gũi lớn nhất Việt Nam. Nổi tiếng với cách review sản phẩm dễ hiểu và vui nhộn.",
                LocationCity   = "TP. Hồ Chí Minh",
                RatingAvg      = 4.9m, RatingCount = 1420,
                IsVerified     = true,  MinBudget = 40_000_000,
                TotalFollowers = 4_200_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Facebook", "TikTok" },
                Category       = "Tech",
                ContactEmail   = "vinhvatvo.media@gmail.com",
                ContactPhone   = "0901 234 567"
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000008"),
                FullName       = "Duy Thẩm",
                AvatarUrl      = "https://yt3.googleusercontent.com/ytc/AIdro_mVQtD8KzTLUqLc27dcXUba9d-X1eMwp0iJNaC_r4fF7A=s400-c-k-c0x00ffffff-no-rj-mo",
                InfluencerType = "Macro",
                Bio            = "Ngô Đức Duy – Tech reviewer chuyên review laptop, PC gaming và thiết bị văn phòng theo phong cách thực tế, chi tiết và dễ hiểu.",
                LocationCity   = "Hà Nội",
                RatingAvg      = 4.8m, RatingCount = 670,
                IsVerified     = true,  MinBudget = 15_000_000,
                TotalFollowers = 1_100_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Facebook", "TikTok" },
                Category       = "Tech",
                ContactEmail   = "duytham.tech@gmail.com",
                ContactPhone   = "0934 567 123"
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000009"),
                FullName       = "Tài Xài Tech",
                AvatarUrl      = "https://yt3.googleusercontent.com/ytc/AIdro_kwncsgKRR8tT5vB7ueSqInVsGq7w_mTp1fx4hCbRTw3Q=s400-c-k-c0x00ffffff-no-rj-mo",
                InfluencerType = "Macro",
                Bio            = "Kênh review công nghệ theo hướng thực dụng: sản phẩm tốt nhất trong tầm giá, phù hợp với người dùng phổ thông Việt Nam.",
                LocationCity   = "TP. Hồ Chí Minh",
                RatingAvg      = 4.7m, RatingCount = 380,
                IsVerified     = true,  MinBudget = 8_000_000,
                TotalFollowers = 620_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "TikTok", "Facebook" },
                Category       = "Tech",
                ContactEmail   = "taixaitech.collab@gmail.com",
                ContactPhone   = "0977 888 999"
            },

            // ─── Thể thao & Fitness ─────────────────────────────────────────
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000010"),
                FullName       = "Quang Hải",
                AvatarUrl      = "https://yt3.ggpht.com/yeh1yHaY2VXNt0fTQL9alsFv9TVeKQFS0c7hFsF9x6rhT3Esm-sSN9GT_1LDTDs6XqysYUhgYGY=s400-c-k-c0x00ffffff-no-rj-mo",
                InfluencerType = "Mega",
                Bio            = "Tiền vệ xuất sắc nhất Đông Nam Á, biểu tượng bóng đá Việt Nam. Brand ambassador cho nhiều thương hiệu thể thao lớn.",
                LocationCity   = "Hà Nội",
                RatingAvg      = 4.9m, RatingCount = 2100,
                IsVerified     = true,  MinBudget = 80_000_000,
                TotalFollowers = 5_200_000, TopPlatform = "Instagram",
                Platforms      = new() { "Instagram", "Facebook", "TikTok" },
                Category       = "SportsFitness",
                ContactEmail   = "quanghai.management@gmail.com",
                ContactPhone   = "0955 444 555"
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000011"),
                FullName       = "Tống Huy Hoàng (gym_tong)",
                AvatarUrl      = "https://yt3.ggpht.com/9JFshQc4lcrWY1mX6Iy74KiCuFUVBALP9hVKd8V5ddQi3vwDmazRnYsb5iq4fAcI_7j2AM86=s400-c-k-c0x00ffffff-no-rj-mo",
                InfluencerType = "Macro",
                Bio            = "Huấn luyện viên thể hình nổi tiếng, cựu á quân thể hình quốc gia. Chia sẻ kiến thức gym thực chiến cho người Việt.",
                LocationCity   = "TP. Hồ Chí Minh",
                RatingAvg      = 4.8m, RatingCount = 480,
                IsVerified     = true,  MinBudget = 8_000_000,
                TotalFollowers = 760_000, TopPlatform = "Instagram",
                Platforms      = new() { "Instagram", "YouTube", "TikTok" },
                Category       = "SportsFitness",
                ContactEmail   = "gymtong.booking@gmail.com",
                ContactPhone   = "0944 555 666"
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000012"),
                FullName       = "Thùy Tiên",
                AvatarUrl      = "https://yt3.ggpht.com/uPC9HrAmFQ7vwz19GgOIjdYQJOC0sUNIiir523xZ-d_r8mv-H34IDfKSfwd9-iviqVMbi2zVKg=s400-c-k-c0x00ffffff-no-rj-mo",
                InfluencerType = "Mega",
                Bio            = "Nguyễn Thúc Thùy Tiên – Miss Grand International 2021, fitness & lifestyle influencer truyền cảm hứng sức khỏe và vẻ đẹp tích cực cho giới trẻ Việt.",
                LocationCity   = "TP. Hồ Chí Minh",
                RatingAvg      = 4.9m, RatingCount = 2400,
                IsVerified     = true,  MinBudget = 50_000_000,
                TotalFollowers = 6_800_000, TopPlatform = "Instagram",
                Platforms      = new() { "Instagram", "TikTok", "YouTube", "Facebook" },
                Category       = "SportsFitness",
                ContactEmail   = "thuytien.management@gmail.com",
                ContactPhone   = "0909 888 777"
            },

            // ─── Du lịch ────────────────────────────────────────────────────
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000013"),
                FullName       = "Khoai Lang Thang",
                AvatarUrl      = "https://yt3.googleusercontent.com/EFB13mILKGh6KbJnTUayPFRw11s4iKhz6GtpbfTl2AAwmUo0FFB2jbxpOup4j5w0gAhYKyqudR4=s400-c-k-c0x00ffffff-no-rj",
                InfluencerType = "Mega",
                Bio            = "Travel vlogger hàng đầu Việt Nam với phong trào \"ăn thật ngon, đi thật nhiều\". YouTube 3M+ subscribers, đã đi 60+ quốc gia.",
                LocationCity   = "TP. Hồ Chí Minh",
                RatingAvg      = 4.9m, RatingCount = 1800,
                IsVerified     = true,  MinBudget = 40_000_000,
                TotalFollowers = 4_500_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Facebook", "TikTok", "Instagram" },
                Category       = "Travel",
                ContactEmail   = "khoailangthang.collab@gmail.com",
                ContactPhone   = "0933 999 000"
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000014"),
                FullName       = "Giang Ơi",
                AvatarUrl      = "https://upload.wikimedia.org/wikipedia/commons/3/3c/Giang_Oi.jpg",
                InfluencerType = "Macro",
                Bio            = "Lifestyle & travel blogger, nổi tiếng với những video du lịch chất lượng cao và nội dung self-development truyền cảm hứng.",
                LocationCity   = "Hà Nội",
                RatingAvg      = 4.9m, RatingCount = 920,
                IsVerified     = true,  MinBudget = 20_000_000,
                TotalFollowers = 2_100_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Instagram", "TikTok" },
                Category       = "Travel",
                ContactEmail   = "giangoi.booking@gmail.com",
                ContactPhone   = "0908 123 456"
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000015"),
                FullName       = "Châu Bùi",
                AvatarUrl      = "https://yt3.ggpht.com/Gh5yISTTEXdLavNGHkV7eJgnU_r487_feZG4hSMIovAK1H0gKgN3zRRVh_EBhnpWwaM7yZY3xNQ=s400-c-k-c0x00ffffff-no-rj-mo",
                InfluencerType = "Macro",
                Bio            = "Fashion & lifestyle influencer hàng đầu Việt Nam. Brand ambassador toàn cầu: Chanel, Dior, Louis Vuitton. Top 10 Best Dressed Asia 2024.",
                LocationCity   = "TP. Hồ Chí Minh",
                RatingAvg      = 4.9m, RatingCount = 1100,
                IsVerified     = true,  MinBudget = 30_000_000,
                TotalFollowers = 3_200_000, TopPlatform = "Instagram",
                Platforms      = new() { "Instagram", "YouTube", "TikTok", "Facebook" },
                Category       = "Travel",
                ContactEmail   = "chaubui.collab@gmail.com",
                ContactPhone   = "0912 345 678"
            },

            // ─── Gaming ─────────────────────────────────────────────────────
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000016"),
                FullName       = "Độ Mixi",
                AvatarUrl      = "https://yt3.ggpht.com/YaAFWY03ER0DfF77HAyMqNlRxnJiSEDq_I7ZF0MlcgRcVzOhIhZfB8QlwNhAuVXZesi2I2zy=s400-c-k-c0x00ffffff-no-rj-mo",
                InfluencerType = "Mega",
                Bio            = "Nguyễn Văn Đức – streamer/YouTuber gaming số 1 Việt Nam. Nổi tiếng với PUBG, Free Fire. YouTube 10M+ subscribers.",
                LocationCity   = "Hà Nội",
                RatingAvg      = 5.0m, RatingCount = 4200,
                IsVerified     = true,  MinBudget = 100_000_000,
                TotalFollowers = 12_000_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Facebook", "TikTok" },
                Category       = "Gaming",
                ContactEmail   = "domixi.management@gmail.com",
                ContactPhone   = "0919 246 810",
                AvgResponseTime = "Theo lịch trình",
                CompletedCampaigns = 340,
                CompletionRate = 97.8m
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000017"),
                FullName       = "ViruSs",
                AvatarUrl      = "https://yt3.googleusercontent.com/ytc/AIdro_mQy__9mdqQSxyYFAUpo5MdGD6QyHtj5-rUPTKqOlUq8g=s400-c-k-c0x00ffffff-no-rj-mo",
                InfluencerType = "Mega",
                Bio            = "Nguyễn Trung Kiên – streamer, ca sĩ kiêm diễn viên. Nổi tiếng với LMHT và những màn reaction hài hước, cộng đồng 5M+ fan.",
                LocationCity   = "TP. Hồ Chí Minh",
                RatingAvg      = 4.9m, RatingCount = 2800,
                IsVerified     = true,  MinBudget = 60_000_000,
                TotalFollowers = 7_500_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Facebook", "TikTok", "Instagram" },
                Category       = "Gaming",
                ContactEmail   = "viruss.booking@gmail.com",
                ContactPhone   = "0882 135 799"
            },
            new()
            {
                UserId         = Guid.Parse("00000000-0000-0000-0000-000000000018"),
                FullName       = "Thầy Giáo Ba",
                AvatarUrl      = "https://yt3.googleusercontent.com/a0_cx-_0C__vVw6xb3lP64_Z_vQL55wjZEjayJNX-MKaAgLgneYorFrligf5QAycRYiqBN3hOA=s400-c-k-c0x00ffffff-no-rj",
                InfluencerType = "Mega",
                Bio            = "Streamer LMHT rank Thách Đấu nổi tiếng với phong cách phân tích meta, coaching và những clip highlight cực đỉnh.",
                LocationCity   = "Hà Nội",
                RatingAvg      = 4.9m, RatingCount = 1600,
                IsVerified     = true,  MinBudget = 50_000_000,
                TotalFollowers = 5_000_000, TopPlatform = "YouTube",
                Platforms      = new() { "YouTube", "Facebook", "TikTok" },
                Category       = "Gaming",
                ContactEmail   = "thaygiaoba.media@gmail.com",
                ContactPhone   = "0906 864 200"
            },
        };
    }
}
