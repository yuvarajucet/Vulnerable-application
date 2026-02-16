using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using VulnSync.Models.IDOR;

namespace VulnSync.Controllers.Samples.Vulnerable;

[Route("Vulnerable/[controller]")]
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
        return View("Vulnerable/Login");
    }
    
    [HttpPost]
    public IActionResult Login(string username, string password)
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
    
    [HttpGet("userdetails")]
    public IActionResult VulnerableUserDetails(int userId)
    {
        var user = _userService.GetUserDetails(userId);
        if (user != null)
        {
            return View("Vulnerable/UserDetails", user);
        }
        else
        {
            ViewBag.ErrorMessage = "User not found";
            return RedirectToAction("ErrorPage");
        }
    }
}