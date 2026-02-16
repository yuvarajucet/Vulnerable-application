using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace VulnSync.Controllers.Samples.Vulnerable;

[Route("vulnerable/[controller]")]
public class IprErrController : Controller
{
    private string _idorConnectionString;

    public IprErrController()
    {
        _idorConnectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database/IDOR/database.db")}";
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("Vulnerable/Login");
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        try
        {
            using (SqliteConnection conn = new SqliteConnection(_idorConnectionString))
            {
                var base64EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
                conn.Open();
                string query = $"SELECT * FROM Users WHERE Username = '{username}' AND Password = '{base64EncodedPassword}'";
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return Content("Login successful");
                    }
                    else
                    {
                        return Content("Invalid credentials.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return Content($"Error: {ex.Message}, Stacktrace: {ex.StackTrace}"); // ❌ Vulnerability: Exposes database and system details
        }
    }
}