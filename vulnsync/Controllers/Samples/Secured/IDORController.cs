using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using VulnSync.Models.IDOR;

namespace VulnSync.Controllers.Samples.Secured;

[Route("Secured/[controller]")]
public class IDORController : Controller
{
    private readonly UserService _userService;
    private string _connectionString;
    
    public IDORController()
    {
        _connectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database/IDOR/database.db")}";
        _userService = new UserService(_connectionString) ?? throw new ArgumentNullException(nameof(_userService));
        _userService.SeedUsers();
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        return View("Secured/Login");
    }
    
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _userService.AuthenticateUser(username, password);
        if (user != null)
        {
            HttpContext.Session.SetInt32("UserId", user.Id);
            return View("Secured/Profile", user);
        }
        else
        {
            ViewBag.ErrorMessage = "Invalid username or password";
            return View("Secured/Login");
        }
    }
    
    [HttpGet("userdetails")]
    public IActionResult SecuredUserDetails(int userId)
    {
        int? currentUserId = HttpContext.Session.GetInt32("UserId"); // Retrieve UserId from session

        if (currentUserId == null || currentUserId != userId)
        {
            return View("Secured/Unauthorized"); // UserId mismatch or not logged in
        }

        var user = _userService.GetUserDetails(userId);
        if (user != null)
        {
            return View("Secured/UserDetails", user);
        }
        else
        {
            ViewBag.ErrorMessage = "User not found";
            return RedirectToAction("ErrorPage");
        }
    }
}