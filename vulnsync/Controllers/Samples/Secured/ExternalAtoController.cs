using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using VulnSync.Models.ExternalAto;


namespace VulnSync.Controllers.Samples.Secured;

[Route("/secured/[controller]")]
public class ExternalAtoController : Controller
{
    private string _connectionString;

    public ExternalAtoController()
    {
        this._connectionString = $"DataSource={Path.Combine(Directory.GetCurrentDirectory(), "Database/ExternalAto/SecuredDatabase.db")}";
        this.SeedUsers();
    }
    
    [HttpGet]
    public IActionResult SecuredExternalAto()
    {
        return View("Secured/ExternalAto");
    }


    [HttpPost]
    public IActionResult SecuredLogin(string username, string password)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            
            string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND Password = @Password";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                var base64EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
                command.Parameters.AddWithValue("@Password", base64EncodedPassword);
                
                var count = Convert.ToInt32(command.ExecuteScalar());
                
                if (count == 1)
                {
                    // User authenticated successfully
                    ViewBag.Username = username;
                    return View("Secured/Profile");
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid username or password";
                    return View("Secured/ExternalAto");
                }
            }
        }
    }

    /*----------------------------- SSO handler -------------------------------------------*/
    [HttpGet("/secured/oauth/login")]
    public IActionResult SecuredOAuthLogin(string state, string code)
    {
        return View("Secured/OAuthLogin");
    }
    
    [HttpPost("oauth/callback")]
    public IActionResult VulnerableOAuthCallback(string username, string email)
    {
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
        {
            DatabaseResponseModel result = UpdateNewUser(username, email, "NaN");
            if (result.IsUserCreated || result.IsUserExisit)
            {
                ViewBag.Username = username;
                return View("Secured/Profile");
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to create new user";
            }
        }
        return View("Secured/ExternalAto");
    }

    
    /*----------------------------------------------- Helper Methods -------------------------------------------*/
    private void SeedUsers()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var tableCheckQuery = "CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY, Username TEXT, Email TEXT, Password TEXT)";
            using (var command = new SqliteCommand(tableCheckQuery, connection))
            {
                command.ExecuteNonQuery();
            }
            
            var insertQuery = @"INSERT INTO Users (Username, Email, Password) SELECT @Username, @Email, @Password WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username = @Username)";

            var users = new[]
            {
                new { Username = "yuvaraj", Email = "yuvaraj@gmail.com",  Password = "UGFzc3dvcmQ=" },
                new { Username = "demo", Email = "demo@gmail.com", Password = "UGFzc3dvcmQ=" },
                new { Username = "test", Email = "test@gmail.com", Password = "UGFzc3dvcmQ=" }
            };

            foreach (var user in users)
            {
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    private DatabaseResponseModel UpdateNewUser(string username,string email, string password)
    {
        var checkQuery = @"SELECT 1 FROM Users WHERE Username = @Username";
        var inserQuery = @"INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)";
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(checkQuery, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                int isUserExisit = Convert.ToInt32(command.ExecuteScalar());
                if (isUserExisit == 1)
                {
                    return new DatabaseResponseModel { IsUserExisit = true, IsUserCreated = false };
                }

            }

            using (var command = new SqliteCommand(inserQuery, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);
                int isUserCreated = Convert.ToInt32(command.ExecuteNonQuery());
                if (isUserCreated == 1)
                {
                    return new DatabaseResponseModel { IsUserExisit = false, IsUserCreated = true };
                }
            }
        }

        return new DatabaseResponseModel();
    }


}