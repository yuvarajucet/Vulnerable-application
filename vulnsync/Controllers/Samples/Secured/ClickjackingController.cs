using Microsoft.AspNetCore.Mvc;

namespace VulnSync.Controllers.Samples.Secured;

[Route("secured/[controller]")]

public class ClickjackingController : Controller
{
    public IActionResult Index()
    {
        return View("Secured/Index");
    }
    
    [HttpGet("ClickjackingDemo")]
    public IActionResult ClickjackingDemo()
    {
        // Apply X-Frame-Options header to prevent the page from being loaded into any iframe
        HttpContext.Response.Headers.Add("X-Frame-Options", "deny");
        return View("Secured/ClickjackingDemo");
    }
}