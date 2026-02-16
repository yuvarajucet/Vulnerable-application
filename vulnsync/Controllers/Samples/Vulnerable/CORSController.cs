using Microsoft.AspNetCore.Mvc;

namespace vulnsync.Controllers.Samples.Vulnerable;

[Route("Vulnerable/[controller]")]
public class CORSController : Controller
{
    private readonly UserService _userService;
    private string _idorConnectionString;

    public CORSController()
    {
        _idorConnectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database/IDOR/database.db")}";
        _userService = new UserService(_idorConnectionString) ?? throw new ArgumentNullException(nameof(_userService));
        _userService.SeedUsers();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("Vulnerable/Login");
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync(string username, string password)
    {
        var user = _userService.AuthenticateUser(username, password);

        if (user != null)
        {
            return View("Vulnerable/Profile", user);
        }
        else
        {
            ViewBag.ErrorMessage = "Invalid username or password";
            return View("Vulnerable/Login");
        }
    }
}



