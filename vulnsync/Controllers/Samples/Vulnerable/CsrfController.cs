using Microsoft.AspNetCore.Mvc;

namespace VulnSync.Controllers.Samples.Vulnerable;

[Route("Vulnerable/[controller]")]
public class CsrfController : Controller
{
    private readonly UserService _userService;
    private string _connectionString;

    public CsrfController()
    {
        _connectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database/IDOR/database.db")}";
        _userService = new UserService(_connectionString) ?? throw new ArgumentNullException(nameof(_userService));
        _userService.SeedUsers();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("Vulnerable/Login");
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _userService.AuthenticateUser(username, password);
        if (user != null)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.None, 
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            };

            Response.Cookies.Append("UserId", user.Id.ToString(), cookieOptions);

            return View("Vulnerable/Profile", user);
        }
        else
        {
            ViewBag.ErrorMessage = "Invalid username or password";
            return View("Vulnerable/Login");
        }
    }

    [HttpGet("Updatepassword")]
    public IActionResult UpdatePassword()
    {
        return View("Vulnerable/Updatepassword");
    }

    [HttpPost("Updatepassword")]
    public IActionResult UpdatePassword(string newPassword, string confirmPassword)
    {
        string userIdCookie = HttpContext.Request.Cookies["UserId"];
        int? currentUserId = null;

        if (!string.IsNullOrEmpty(userIdCookie))
        {
            if (int.TryParse(userIdCookie, out int userId))
            {
                currentUserId = userId;
            }
        }

        if (currentUserId == null)
        {
            return Unauthorized("User is not logged in.");
        }

        if (!_userService.UpdatePassword(currentUserId.Value, newPassword, confirmPassword, out string errorMessage))
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Vulnerable/Updatepassword");
        }

        HttpContext.Response.Cookies.Delete("UserId");
        HttpContext.Session.Clear(); 
        return RedirectToAction("Login", "Csrf");
    }
}