using Microsoft.EntityFrameworkCore;
using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Services;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.Hubs;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IKolProfileService, KolProfileService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IFinanceService, FinanceService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDiscoveryService, DiscoveryService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddHttpClient<IGeminiService, GeminiService>();

builder.Services.AddSignalR();

// Configure MoMo Options
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("Momo"));
builder.Services.AddHttpClient<IMomoService, MomoService>();

builder.Services.AddAuthentication("KolCookies")
    .AddCookie("KolCookies", options =>
    {
        options.Cookie.Name = "KolAuthCookie";
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<KolMarketplaceContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<KolMarketplaceContext>();
        
        var adminRole = context.Roles.FirstOrDefault(r => r.Code == "Admin");
        if (adminRole == null)
        {
            adminRole = new Role { Code = "Admin", Name = "Quản trị viên" };
            context.Roles.Add(adminRole);
            context.SaveChanges();
        }

        var adminUser = context.Users.Include(u => u.Roles).FirstOrDefault(u => u.Email == "admin@kol.com");
        if (adminUser == null)
        {
            adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@kol.com",
                FullName = "Hệ thống Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Status = "active",
                Roles = new List<Role> { adminRole }
            };
            context.Users.Add(adminUser);
            context.SaveChanges();
        }
        else
        {
            if (!adminUser.Roles.Any(r => r.Code == "Admin"))
            {
                adminUser.Roles.Add(adminRole);
            }
            // Luôn đặt lại mật khẩu thành admin123 để đảm bảo đăng nhập thành công
            adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Có lỗi xảy ra khi tạo tài khoản Admin mặc định.");
    }
}

// Seed KOL/KOC data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<KolMarketplaceContext>();
        SeedKolData(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Có lỗi xảy ra khi seed dữ liệu KOL.");
    }
}

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<ChatHub>("/chatHub");

app.Run();

// ===== KOL Seed =====
static void SeedKolData(KolMarketplaceContext context)
{
    // Only seed if no KOL profiles exist (REMOVED FORCE SEEDING)
    // if (context.KolProfiles.Any()) return;

    var kolRole = context.Roles.FirstOrDefault(r => r.Code == "KOL");
    if (kolRole == null)
    {
        kolRole = new Role { Code = "KOL", Name = "KOL/KOC" };
        context.Roles.Add(kolRole);
        context.SaveChanges();
    }

    var mockIdols = KOL_KOC_TAAA.Services.MockIdolService.GetMockIdols();

    foreach (var kol in mockIdols)
    {
        var standardizedEmail = $"{kol.FullName.Replace(" ", "").ToLower()}@kol.vn";
        var existingUser = context.Users.FirstOrDefault(u => u.Email == standardizedEmail);
        
        if (existingUser == null)
        {
            existingUser = new User
            {
                Id = kol.UserId,
                Email = standardizedEmail,
                FullName = kol.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("kol@123"),
                Status = "active",
                AvatarUrl = kol.AvatarUrl,
                Roles = new List<Role> { kolRole }
            };
            context.Users.Add(existingUser);
            context.SaveChanges();
        }

        var existingProfile = context.KolProfiles.FirstOrDefault(p => p.UserId == existingUser.Id);
        if (existingProfile == null)
        {
            existingProfile = new KolProfile
            {
                UserId = existingUser.Id,
                InfluencerType = kol.InfluencerType,
                Bio = kol.Bio ?? "Bio mặc định",
                LocationCity = kol.LocationCity,
                LocationCountry = kol.LocationCountry ?? "Việt Nam",
                MinBudget = kol.MinBudget ?? 0,
                IsVerified = true,
                RatingAvg = kol.RatingAvg,
                RatingCount = kol.RatingCount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.KolProfiles.Add(existingProfile);
            context.SaveChanges();
        }

        if (kol.SocialAccounts != null && kol.SocialAccounts.Any())
        {
            foreach (var acc in kol.SocialAccounts)
            {
                var exists = context.KolSocialAccounts.Any(s =>
                    s.KolUserId == existingUser.Id && s.Platform == acc.Platform);
                if (!exists)
                {
                    context.KolSocialAccounts.Add(new KolSocialAccount
                    {
                        Id = Guid.NewGuid(),
                        KolUserId = existingUser.Id,
                        Platform = acc.Platform,
                        Username = acc.Username,
                        Followers = acc.Followers,
                        EngagementRate = acc.EngagementRate,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            context.SaveChanges();
        }
    }

    var oldKols = new[]
    {
        new {
            Name = "Châu Bùi",
            Email = "chaubui@kol.vn",
            Type = "KOL",
            Bio = "Fashion & Lifestyle influencer nổi tiếng Việt Nam.",
            City = "Hà Nội",
            Accounts = new[] {
                (Platform: "Instagram", Username: "chaubuivl", Followers: 1_800_000L, EngRate: 3.2m),
                (Platform: "TikTok",    Username: "chaubuivl",  Followers: 2_600_000L, EngRate: 5.1m),
                (Platform: "YouTube",   Username: "Châu Bùi",   Followers:   650_000L, EngRate: 2.8m)
            }
        },
        new {
            Name = "Hà Linh Official",
            Email = "halinh_old@kol.vn",
            Type = "KOL",
            Bio = "Beauty & Makeup artist chia sẻ tips làm đẹp.",
            City = "TP.HCM",
            Accounts = new[] {
                (Platform: "TikTok",    Username: "halinofficial",   Followers: 8_000_000L, EngRate: 7.4m),
                (Platform: "Instagram", Username: "halinofficial",   Followers: 1_200_000L, EngRate: 4.0m),
                (Platform: "YouTube",   Username: "Hà Linh Official",Followers:   900_000L, EngRate: 3.5m)
            }
        },
        new {
            Name = "Khoai Lang Thang",
            Email = "khoailangthang@kol.vn",
            Type = "KOL",
            Bio = "Travel vlogger khám phá ẩm thực và địa điểm du lịch.",
            City = "Đà Nẵng",
            Accounts = new[] {
                (Platform: "YouTube",   Username: "Khoai Lang Thang",Followers: 3_500_000L, EngRate: 4.2m),
                (Platform: "Facebook",  Username: "KhoaiLangThang",  Followers: 1_900_000L, EngRate: 2.9m),
                (Platform: "TikTok",    Username: "khoailangthang",  Followers: 1_200_000L, EngRate: 4.8m)
            }
        },
        new {
            Name = "Call Me Duy",
            Email = "callmeduy@kol.vn",
            Type = "KOL",
            Bio = "Comedian & Entertainer với nội dung hài hước đa dạng.",
            City = "TP.HCM",
            Accounts = new[] {
                (Platform: "YouTube",   Username: "CallMeDuy",   Followers: 4_200_000L, EngRate: 5.6m),
                (Platform: "TikTok",    Username: "callmeduy_vn",Followers: 5_100_000L, EngRate: 8.3m),
                (Platform: "Instagram", Username: "callmeduy",   Followers:   900_000L, EngRate: 3.7m)
            }
        },
        new {
            Name = "Hương Tịt",
            Email = "huongtit@kol.vn",
            Type = "KOC",
            Bio = "Beauty reviewer & KOC chia sẻ review mỹ phẩm chân thực.",
            City = "Hà Nội",
            Accounts = new[] {
                (Platform: "TikTok",    Username: "huongtit",   Followers: 3_700_000L, EngRate: 6.9m),
                (Platform: "YouTube",   Username: "Hương Tịt", Followers: 1_100_000L, EngRate: 4.1m),
                (Platform: "Instagram", Username: "huongtit",  Followers:   600_000L, EngRate: 3.5m)
            }
        },
        new {
            Name = "Linh Ngọc Đàm",
            Email = "linhngocdamofficial@kol.vn",
            Type = "KOL",
            Bio = "Gaming streamer & beauty content creator.",
            City = "Hà Nội",
            Accounts = new[] {
                (Platform: "Facebook",  Username: "LinhNgocDamFanpage", Followers: 5_500_000L, EngRate: 4.4m),
                (Platform: "YouTube",   Username: "Linh Ngọc Đàm",      Followers: 3_200_000L, EngRate: 3.9m),
                (Platform: "TikTok",    Username: "linhngocdamoffcl",    Followers: 2_100_000L, EngRate: 5.5m)
            }
        },
        new {
            Name = "Quỳnh Anh Shyn",
            Email = "quynhanhshyn@kol.vn",
            Type = "KOL",
            Bio = "Fashion influencer & style icon của giới trẻ Việt.",
            City = "Hà Nội",
            Accounts = new[] {
                (Platform: "Instagram", Username: "quynhanhshyn",Followers: 1_500_000L, EngRate: 3.8m),
                (Platform: "TikTok",    Username: "quynhanhshyn",Followers: 2_800_000L, EngRate: 6.1m),
                (Platform: "YouTube",   Username: "Quỳnh Anh Shyn",Followers: 800_000L, EngRate: 2.9m)
            }
        },
        new {
            Name = "MisThy",
            Email = "misthy@kol.vn",
            Type = "KOL",
            Bio = "Gaming streamer & content creator hàng đầu Việt Nam.",
            City = "TP.HCM",
            Accounts = new[] {
                (Platform: "YouTube",   Username: "MisThy",     Followers: 5_900_000L, EngRate: 4.7m),
                (Platform: "Facebook",  Username: "MisThyGaming",Followers: 4_200_000L, EngRate: 3.5m),
                (Platform: "TikTok",    Username: "misthy_official",Followers: 3_100_000L, EngRate: 6.0m)
            }
        },
        new {
            Name = "Chloe Nguyễn",
            Email = "chloenguyenvn_old@kol.vn",
            Type = "KOL",
            Bio = "Beauty & luxury lifestyle influencer.",
            City = "TP.HCM",
            Accounts = new[] {
                (Platform: "Instagram", Username: "chloenguyenvn",Followers: 700_000L,   EngRate: 5.2m),
                (Platform: "YouTube",   Username: "Chloe Nguyễn", Followers: 1_800_000L, EngRate: 4.3m),
                (Platform: "TikTok",    Username: "chloenguyenvn",Followers: 900_000L,   EngRate: 5.8m)
            }
        },
        new {
            Name = "Trinh Phạm",
            Email = "trinhpham@kol.vn",
            Type = "KOC",
            Bio = "Lifestyle blogger & KOC chia sẻ cuộc sống và sản phẩm.",
            City = "Hà Nội",
            Accounts = new[] {
                (Platform: "TikTok",    Username: "trinhpham.elle",Followers: 1_600_000L, EngRate: 5.9m),
                (Platform: "Instagram", Username: "trinhpham.elle",Followers:   450_000L, EngRate: 4.4m),
                (Platform: "YouTube",   Username: "Trinh Phạm",  Followers:   320_000L, EngRate: 3.1m)
            }
        }
    };

    foreach (var oldKol in oldKols)
    {
        var existingOldUser = context.Users.FirstOrDefault(u => u.Email == oldKol.Email);
        if (existingOldUser == null)
        {
            existingOldUser = new User
            {
                Id = Guid.NewGuid(),
                Email = oldKol.Email,
                FullName = oldKol.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("kol@123"),
                Status = "active",
                AvatarUrl = "https://ui-avatars.com/api/?name=" + Uri.EscapeDataString(oldKol.Name) + "&background=random",
                Roles = new List<Role> { kolRole }
            };
            context.Users.Add(existingOldUser);
            context.SaveChanges();
        }

        var existingOldProfile = context.KolProfiles.FirstOrDefault(p => p.UserId == existingOldUser.Id);
        if (existingOldProfile == null)
        {
            existingOldProfile = new KolProfile
            {
                UserId = existingOldUser.Id,
                InfluencerType = oldKol.Type,
                Bio = oldKol.Bio,
                LocationCity = oldKol.City,
                LocationCountry = "Việt Nam",
                MinBudget = 1000000,
                IsVerified = true,
                RatingAvg = 4.8m + (decimal)new Random().NextDouble() * 0.2m,
                RatingCount = new Random().Next(50, 500),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.KolProfiles.Add(existingOldProfile);
            context.SaveChanges();
        }

        foreach (var acc in oldKol.Accounts)
        {
            var existsAccount = context.KolSocialAccounts.Any(s =>
                s.KolUserId == existingOldUser.Id && s.Platform == acc.Platform);
            if (!existsAccount)
            {
                // Kiểm tra xem Username đã bị tài khoản khác ở platform đó chiếm dụng chưa
                string checkUsername = acc.Username;
                var usernameTaken = context.KolSocialAccounts.Any(s => s.Platform == acc.Platform && s.Username == checkUsername);
                if (usernameTaken)
                {
                    checkUsername = acc.Username + "_" + new Random().Next(1000, 9999).ToString();
                }

                context.KolSocialAccounts.Add(new KolSocialAccount
                {
                    Id = Guid.NewGuid(),
                    KolUserId = existingOldUser.Id,
                    Platform = acc.Platform,
                    Username = checkUsername,
                    Followers = acc.Followers,
                    EngagementRate = acc.EngRate,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
        context.SaveChanges();
    }
}
