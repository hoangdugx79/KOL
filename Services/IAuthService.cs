using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;

namespace KOL_KOC_TAAA.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, User? User)> LoginAsync(LoginViewModel model);
    Task<(bool Success, string Message)> RegisterAsync(RegisterViewModel model);
    Task LogoutAsync();
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> SeedAdminAsync();
}
