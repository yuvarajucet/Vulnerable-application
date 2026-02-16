
using Microsoft.AspNetCore.Mvc;
using VulnSync.Models.Vulnerable.Openredirect;

namespace VulnSync.Controllers.Samples.Secured;

[Route("secured/[controller]")]
public class OpenRedirect : Controller
{
    [HttpGet]
    public IActionResult SecuredIndex()
    {
        return View("Secured/Index");
    }

    [HttpGet("login")]
    public IActionResult SecuredLoginPage()
    {
        return View("Secured/login");
    }

    [HttpPost("login")]
    public IActionResult SecuredLogin([FromBody] LoginModel userInfo)
    {
        if (userInfo.Username == "john" && userInfo.Password == "John@123")
        {
            return Ok(new {
                redirectUrl = userInfo.ReturnUrl,
                success = true,
                errorMessage = string.Empty,
                isValidurl = IsValidRedirectUrl(userInfo.ReturnUrl)
            });
        }

        return Ok(new {
            success = false,
            errorMessage = "Username or password is incorrect"
        });
    }

    [HttpGet("profile")]
    public IActionResult SecuredProfile()
    {
        return View("Secured/profile");
    }

    [HttpGet("warning")]
    public IActionResult Warning() 
    {
        return View("Secured/warning");
    }

// -------------------------- Helper methods ----------------------------------------

   private bool IsValidRedirectUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        Uri redirectUri;
        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out redirectUri))
        {
            return false;
        }

        if (!redirectUri.IsAbsoluteUri)
        {
            return true;
        }

        var requestHost = HttpContext.Request.Host.ToString();
        return string.Equals(redirectUri.Host, requestHost, StringComparison.OrdinalIgnoreCase);
    }
}