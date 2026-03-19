using KOL_KOC_TAAA.Services;
using KOL_KOC_TAAA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace KOL_KOC_TAAA.Controllers;

public class DiscoveryController : Controller
{
    private readonly IDiscoveryService _discoveryService;

    public DiscoveryController(IDiscoveryService discoveryService)
    {
        _discoveryService = discoveryService;
    }

    public async Task<IActionResult> Index(DiscoveryFilterViewModel filters, int page = 1)
    {
        var model = await _discoveryService.GetDiscoveryResultsAsync(filters, page);
        
        // Fetch extra sections for the landing layout
        model.TrendingKols = await _discoveryService.GetTrendingKolsAsync(6);
        model.NewRisingKols = await _discoveryService.GetNewRisingKolsAsync(6);
        
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> SearchPartial(DiscoveryFilterViewModel filters, int page = 1)
    {
        var model = await _discoveryService.GetDiscoveryResultsAsync(filters, page);
        return PartialView("_KolGridPartial", model.Kols);
    }
}
