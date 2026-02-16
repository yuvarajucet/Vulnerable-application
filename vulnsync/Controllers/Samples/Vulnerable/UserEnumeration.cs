using Microsoft.AspNetCore.Mvc;

namespace VulnSync.Controllers.Samples.Vulnerable;

[Route("vulnerable/[controller]")]
public class UserEnumeration : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View("Vulnerable/Index");;
    }
}