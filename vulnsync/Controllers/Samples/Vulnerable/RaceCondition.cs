using Microsoft.AspNetCore.Mvc;
using vulnsync.Models.RaceCondition;
using Microsoft.Data.Sqlite;

namespace vulnsync.Controllers.Samples.Vulnerable;

[Route("Vulnerable/[controller]")]
public class RaceConditionController : Controller
{
    private readonly UserService _userService;
    private readonly UserService _userServiceForRaceCondition;
    private string _connectionString;
    private string _idorConnectionString;
    private const int FreeLimit = 5;

    public RaceConditionController()
    {
        _connectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database/RaceCondition/database.db")}";
        _idorConnectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database/IDOR/database.db")}";

        _userService = new UserService(_idorConnectionString) ?? throw new ArgumentNullException(nameof(_userService));
        _userService.SeedUsers();

        _userServiceForRaceCondition = new UserService(_connectionString) ?? throw new ArgumentNullException(nameof(_userServiceForRaceCondition));
        _userServiceForRaceCondition.SeedUserInvites();
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
        var existingUsers = LoadUsersFromDatabase();
        if (user != null)
        {
            return View("Vulnerable/InviteUser", existingUsers);
        }
        else
        {
            ViewBag.ErrorMessage = "Invalid username or password";
            return View("Vulnerable/Login");
        }
    }

    [HttpGet("AllUsers")]
    public IActionResult AllUsers()
    {
        var existingUsers = LoadUsersFromDatabase();
        return View("Vulnerable/InviteUser", existingUsers);
    }

    [HttpPost("InviteUserVulnerable")]
    public IActionResult InviteUserVulnerable(string username, string email)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
        {
            return BadRequest("User name and user email are required.");
        }

        var users = LoadUsersFromDatabase();
        if (users.Count > FreeLimit)
        {
            ViewBag.ErrorMessage = "Free limit reached. Upgrade to Pro to invite more users.";
            return View("Vulnerable/InviteUser", users);
        }

        var newUser = new UserInvite
        {
            Username = username,
            Email = email,
        };

        SaveUserToDatabase(newUser);

        var showAllUsers = LoadUsersFromDatabase();
        return View("Vulnerable/InviteUser", showAllUsers);
    }

    [HttpPost("DeleteUserVulnerable")]
    public IActionResult DeleteUserVulnerable(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username is required.");
        }

        var users = LoadUsersFromDatabase();
        var userToDelete = users.FirstOrDefault(u => u.Username == username);
        if (userToDelete == null)
        {
            ViewBag.ErrorMessage = "User not found.";
            return View("Vulnerable/InviteUser", users);
        }

        DeleteUserFromDatabase(username);
        var updatedUsers = LoadUsersFromDatabase();
        return View("Vulnerable/InviteUser", updatedUsers);
    }

    private List<UserInvite> LoadUsersFromDatabase()
    {
        var users = new List<UserInvite>();
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var command = new SqliteCommand("SELECT Username, Email FROM UserInvites", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(new UserInvite
                    {

                        Username = reader["Username"]?.ToString() ?? string.Empty,
                        Email = reader["Email"]?.ToString() ?? string.Empty
                    });
                }
            }
        }
        return users;
    }

    private void SaveUserToDatabase(UserInvite user)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var command = new SqliteCommand("INSERT INTO UserInvites (Username, Email) VALUES (@Username, @Email)", connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.ExecuteNonQuery();
        }
    }

    private void DeleteUserFromDatabase(string username)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var command = new SqliteCommand("DELETE FROM UserInvites WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);
            command.ExecuteNonQuery();
        }
    }
}

