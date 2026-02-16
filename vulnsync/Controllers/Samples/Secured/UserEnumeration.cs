using Microsoft.AspNetCore.Mvc;

namespace VulnSync.Controllers.Samples.Secured;

[Route("secured/[controller]")]
public class UserEnumeration : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View("Secured/Index");
    }
}