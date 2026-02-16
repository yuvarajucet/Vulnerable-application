using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace VulnSync.Controllers.Samples.Secured;

[Route("secured/[controller]")]
public class IprErrController : Controller
{
    private string _idorConnectionString;

    private readonly ILogger<IprErrController> _logger;

    public IprErrController(ILogger<IprErrController> logger)
    {
        _logger = logger;
        _idorConnectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database/IDOR/database.db")}";
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("Secured/Login");
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        try
        {
            using (SqliteConnection conn = new SqliteConnection(_idorConnectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    var base64EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
                    cmd.Parameters.AddWithValue("@password", base64EncodedPassword); // ❗ Hash passwords in a real application!

                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        return Content("Login successful");
                    }
                    else
                    {
                        return Content("Invalid username or password.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during login"); // ✅ Logs error internally
            return Content("An unexpected error occurred. Please try again later."); // ✅ Generic message
        }
    }
}