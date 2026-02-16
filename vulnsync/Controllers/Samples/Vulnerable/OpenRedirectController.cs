
using Microsoft.AspNetCore.Mvc;
using VulnSync.Models.Vulnerable.Openredirect;

namespace VulnSync.Controllers.Samples.Vulnerable;

[Route("vulnerable/[controller]")]
public class OpenRedirect : Controller
{
    [HttpGet]
    public IActionResult VulnerableIndex()
    {
        return View("Vulnerable/Index");
    }

    [HttpGet("login")]
    public IActionResult VulnerableLoginPage()
    {
        return View("Vulnerable/login");
    }

    [HttpPost("login")]
    public IActionResult VulnerableLogin([FromBody] LoginModel userInfo)
    {
        if (userInfo.Username == "john" && userInfo.Password == "John@123")
        {
            return Ok(new {
                redirectUrl = userInfo.ReturnUrl,
                success = true,
                errorMessage = string.Empty
            });
        }

        return Ok(new {
            success = false,
            errorMessage = "Username or password is incorrect"
        });
    }

    [HttpGet("profile")]
    public IActionResult VulnerableProfile()
    {
        return View("Vulnerable/profile");
    }
    
}