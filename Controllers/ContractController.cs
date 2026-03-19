using KOL_KOC_TAAA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KOL_KOC_TAAA.Controllers;

[Authorize]
public class ContractController : Controller
{
    private readonly IContractService _contractService;

    public ContractController(IContractService contractService)
    {
        _contractService = contractService;
    }

    private Guid GetCurrentUserId()
    {
        var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idString, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var contract = await _contractService.GetContractAsync(id);
        if (contract == null) return NotFound();

        ViewBag.CurrentUserId = GetCurrentUserId();
        return View(contract);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sign(Guid contractId, string signatureData)
    {
        if (string.IsNullOrEmpty(signatureData))
        {
            TempData["ErrorMessage"] = "Vui lòng cung cấp chữ ký.";
            return RedirectToAction(nameof(Details), new { id = contractId });
        }

        var userId = GetCurrentUserId();
        var result = await _contractService.SignContractAsync(contractId, userId, signatureData);

        if (result)
        {
            TempData["SuccessMessage"] = "Ký hợp đồng thành công!";
        }
        else
        {
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi ký hợp đồng.";
        }

        return RedirectToAction(nameof(Details), new { id = contractId });
    }

    [HttpGet]
    public async Task<IActionResult> CreateDraft(Guid bookingId)
    {
        var userId = GetCurrentUserId();
        try
        {
            var contract = await _contractService.CreateDraftContractAsync(bookingId, userId);
            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Không thể tạo hợp đồng: " + ex.Message;
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
