using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace vulnsync.Controllers.Samples.Secured;

[Route("Secured/[controller]")]
public class HHInjectionController : Controller
{
    private readonly UserService _userService;
    private string _idorConnectionString;
    private readonly IConfiguration _configuration;

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
    }

    public HHInjectionController(IConfiguration configuration)
    {
        _configuration = configuration;
        _idorConnectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database/IDOR/database.db")}";
        _userService = new UserService(_idorConnectionString) ?? throw new ArgumentNullException(nameof(_userService));
        _userService.SeedUsers();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("Secured/Login");
    }

    [HttpGet("ResetPasswordSec")]
    public IActionResult ResetPasswordSec()
    {
        return View("Secured/PasswordReset");
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _userService.AuthenticateUser(username, password);

        if (user != null)
        {
            return View("Secured/Profile", user);
        }
        else
        {
            ViewBag.ErrorMessage = "Invalid username or password";
            return View("Secured/Login");
        }
    }

    [HttpPost]
    [Route("GenerateResetLinkSec")]
    public JsonResult GenerateResetLinkSec([FromBody] ResetPasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            return Json(new { success = false, message = "Email is required!" });
        }

        string token = GenerateToken(); // Declare token outside

        using (var connection = new SqliteConnection(_idorConnectionString))
        {
            connection.Open();

            // Check if the user exists
            var command = new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", connection);
            command.Parameters.AddWithValue("@Email", request.Email);

            int userExists = Convert.ToInt32(command.ExecuteScalar());

            if (userExists == 0)
            {
                return Json(new { success = false, message = "User not found!" });
            }

            // Check if a token already exists for the email
            var checkTokenCommand = new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", connection);
            checkTokenCommand.Parameters.AddWithValue("@Email", request.Email);
            int tokenExists = Convert.ToInt32(checkTokenCommand.ExecuteScalar());

            if (tokenExists > 0)
            {
                // Update existing token
                var updateCommand = new SqliteCommand("UPDATE Users SET Token = @Token WHERE Email = @Email", connection);
                updateCommand.Parameters.AddWithValue("@Token", token);
                updateCommand.Parameters.AddWithValue("@Email", request.Email);
                updateCommand.ExecuteNonQuery();
            }
            else
            {
                // Insert new token if none exists
                var insertCommand = new SqliteCommand("INSERT INTO Users (Email, Token) VALUES (@Email, @Token)", connection);
                insertCommand.Parameters.AddWithValue("@Email", request.Email);
                insertCommand.Parameters.AddWithValue("@Token", token);
                insertCommand.ExecuteNonQuery();
            }
        }
        string trustedBaseUrl = _configuration["AppSettings:Domain"];
        // Construct the reset link
        string resetLink = $"{trustedBaseUrl}/Vulnerable/HHInjection/ResetPassword?token={token}&email={Uri.EscapeDataString(request.Email)}";

        return Json(new { success = true, resetLink = resetLink });
    }

    private string GenerateToken()
    {
        byte[] tokenData = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenData);
        }
        return Convert.ToBase64String(tokenData).Replace("+", "").Replace("/", "").Replace("=", "");
    }

    [HttpGet]
    [Route("ResetPassword")]
    public IActionResult ResetPassword(string token, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            ViewBag.ErrorMessage = "Invalid or expired reset link.";
            return View("ResetPasswordVuln");
        }

        ViewBag.Email = email;
        ViewBag.Token = token;
        return View("Secured/ResetPasswordForm");
    }

    [HttpPost("ResetPassword")]
    public IActionResult ResetPassword(string token, string email, string newPassword, string confirmPassword)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            ViewBag.ErrorMessage = "Invalid or expired reset link.";
            return View("ResetPasswordForm");
        }

        if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
        {
            ViewBag.ErrorMessage = "Password fields cannot be empty.";
            return View("ResetPasswordForm");
        }

        if (newPassword != confirmPassword)
        {
            ViewBag.ErrorMessage = "Passwords do not match!";
            return View("ResetPasswordForm");
        }

        using (var connection = new SqliteConnection(_idorConnectionString))
        {
            connection.Open();
            var command = new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email AND Token = @Token", connection);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Token", token);

            int isValidToken = Convert.ToInt32(command.ExecuteScalar());

            if (isValidToken == 0)
            {
                ViewBag.ErrorMessage = "Invalid token.";
                return View("ResetPasswordForm");
            }

            var base64EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(newPassword));

            var updateCommand = new SqliteCommand("UPDATE Users SET Password = @Password WHERE Email = @Email", connection);
            updateCommand.Parameters.AddWithValue("@Password", base64EncodedPassword);
            updateCommand.Parameters.AddWithValue("@Email", email);
            updateCommand.ExecuteNonQuery();
        }

        TempData.Add("Message", "Your password has been reset successfully.");
        return RedirectToAction("Login");
    }
}



