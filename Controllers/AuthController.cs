using KOL_KOC_TAAA.Data;
using KOL_KOC_TAAA.Models;
using KOL_KOC_TAAA.ViewModels;
using KOL_KOC_TAAA.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KOL_KOC_TAAA.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(model);

        if (result.Success)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            // Redirect based on role
            if (result.User!.Roles.Any(r => r.Code == "Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            else if (result.User!.Roles.Any(r => r.Code == "KOL"))
            {
                return RedirectToAction("Index", "CreatorStudio");
            }
            else if (result.User!.Roles.Any(r => r.Code == "Customer"))
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.RegisterAsync(model);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Login");
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SeedAdmin()
    {
        await _authService.SeedAdminAsync();
        return Content("<h3>Cấu hình Admin thành công!</h3>" + 
                       "<p>Tài khoản hệ thống: <b>admin@kol.com</b> / mật khẩu: <b>admin123</b></p>" +
                       "<p>Các tài khoản admin@example.com cũng đã được nâng cấp.</p>" +
                       "<a href='/Auth/Login'>Đến trang Đăng nhập</a>", "text/html", System.Text.Encoding.UTF8);
    }
}
