using Microsoft.AspNetCore.Mvc;

namespace VulnSync.Controllers.Samples.Vulnerable;

[Route("Vulnerable/[controller]")]

public class ClickjackingController : Controller
{
    public IActionResult Index()
    {
        return View("Vulnerable/Index");
    }
    
    [HttpGet("ClickjackingDemo")]
    public IActionResult ClickjackingDemo()
    {
        return View("Vulnerable/ClickjackingDemo");
    }
}