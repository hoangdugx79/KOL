using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KOL_KOC_TAAA.Services;

public class AuthService : IAuthService
{
    private readonly KolMarketplaceContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(KolMarketplaceContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(bool Success, string Message, User? User)> LoginAsync(LoginViewModel model)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            return (false, "Email hoặc mật khẩu không chính xác.", null);
        }

        if (user.Status != "active")
        {
            return (false, "Tài khoản của bạn đã bị vô hiệu hóa.", null);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName ?? user.Email)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Code));
        }

        var identity = new ClaimsIdentity(claims, "KolCookies");
        var principal = new ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext!.SignInAsync("KolCookies", principal, new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTime.UtcNow.AddHours(8)
        });

        // Set session
        _httpContextAccessor.HttpContext.Session.SetString("UserName", user.FullName ?? user.Email);
        _httpContextAccessor.HttpContext.Session.SetString("UserRole", string.Join(",", user.Roles.Select(r => r.Code)));

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (true, "Đăng nhập thành công.", user);
    }

    public async Task<(bool Success, string Message)> RegisterAsync(RegisterViewModel model)
    {
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            return (false, "Email này đã được sử dụng.");
        }

        string roleCode = model.Role.ToUpper() == "KOL" ? "KOL" : "CUSTOMER";
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Code.ToUpper() == roleCode);

        if (role == null)
        {
            role = new Role { Code = roleCode, Name = roleCode == "KOL" ? "KOL" : "Khách hàng" };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = model.Email,
            FullName = model.Email.Split('@')[0],
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Status = "active",
            Roles = new List<Role> { role },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(newUser);

        if (roleCode == "KOL")
        {
            _context.KolProfiles.Add(new KolProfile 
            { 
                UserId = newUser.Id, 
                InfluencerType = "Nano", 
                Bio = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            _context.UserWallets.Add(new UserWallet 
            { 
                UserId = newUser.Id, 
                Balance = 0, 
                LockedBalance = 0, 
                Currency = "VND",
                UpdatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
        return (true, "Đăng ký thành công.");
    }

    public async Task LogoutAsync()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync("KolCookies");
        _httpContextAccessor.HttpContext.Session.Clear();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> SeedAdminAsync()
    {
        var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Code == "Admin");
        if (adminRole == null)
        {
            adminRole = new Role { Code = "Admin", Name = "Quản trị viên" };
            _context.Roles.Add(adminRole);
            await _context.SaveChangesAsync();
        }

        // 1. Ensure system admin exists
        var systemAdmin = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == "admin@kol.com");
        if (systemAdmin == null)
        {
            systemAdmin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@kol.com",
                FullName = "System Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Status = "active",
                Roles = new List<Role> { adminRole },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Users.Add(systemAdmin);
        }
        else if (!systemAdmin.Roles.Any(r => r.Code == "Admin"))
        {
            systemAdmin.Roles.Add(adminRole);
        }

        // 2. Also promote common dev emails if they exist
        var devEmails = new[] { "admin@example.com", "test@test.com" };
        var devUsers = await _context.Users.Include(u => u.Roles)
            .Where(u => devEmails.Contains(u.Email))
            .ToListAsync();

        foreach (var user in devUsers)
        {
            if (!user.Roles.Any(r => r.Code == "Admin"))
            {
                user.Roles.Add(adminRole);
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
