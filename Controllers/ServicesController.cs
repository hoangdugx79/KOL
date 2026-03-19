using Microsoft.AspNetCore.Mvc;

namespace KOL_KOC_TAAA.Controllers;

public class ServicesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Route("services/{type}")]
    public IActionResult Detail(string type)
    {
        ViewBag.ServiceType = type;
        return View();
    }
}
