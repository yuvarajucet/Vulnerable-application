using Microsoft.AspNetCore.Mvc;
using vulnsync.Models.RaceCondition;
using Microsoft.Data.Sqlite;

namespace vulnsync.Controllers.Samples.Secured;

[Route("Secured/[controller]")]
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
        return View("Secured/Login");
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _userService.AuthenticateUser(username, password);
        var existingUsers = LoadUsersFromDatabase();
        if (user != null)
        {
            return View("Secured/InviteUser", existingUsers);
        }
        else
        {
            ViewBag.ErrorMessage = "Invalid username or password";
            return View("Secured/Login");
        }
    }

    [HttpGet("AllUsers")]
    public IActionResult AllUsers()
    {
        var existingUsers = LoadUsersFromDatabase();
        return View("Secured/InviteUser", existingUsers);
    }

    [HttpPost("InviteUser")]
    public IActionResult InviteUser(string username, string email)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
        {
            return BadRequest("User name and user email are required.");
        }
        var users = LoadUsersFromDatabase();
        var existingUser = FindUserByEmail(email);
        if (existingUser != null)
        {
            ViewBag.ErrorMessage = "A user with this email already exists. Please use a different email.";
            return View("Secured/InviteUser", users);
        }

        if (users.Count > FreeLimit)
        {
            ViewBag.ErrorMessage = "Free limit reached. Upgrade to Pro to invite more users.";
            return View("Secured/InviteUser", users);
        }

        var newUser = new UserInvite
        {
            Username = username,
            Email = email,
        };

        SaveUserToDatabase(newUser);

        _ = Task.Run(() => { 
            var _random = new Random();
            int sleepTime = _random.Next(500, 1001);
            System.Threading.Thread.Sleep(sleepTime);
            BackgroundVerificationAndRevert(newUser);
        });

        var showAllUsers = LoadUsersFromDatabase();
        return View("Secured/InviteUser", showAllUsers);
    }

    private void BackgroundVerificationAndRevert(UserInvite newUser)
    {
        try
        {
            var users = LoadUsersFromDatabase();

            if (users.Count > FreeLimit)
            {
                LogAction($"Free user limit exceeded. Current count: {users.Count}, Free limit: {FreeLimit}");

                // Remove all users exceeding the limit
                var usersToRevert = users.Skip(FreeLimit).ToList();
                usersToRevert.ForEach(user => RevertUserInvite(user));

                LogAction($"All users exceeding the free limit have been reverted.");
            }
            else
            {
                LogAction($"User with email {newUser.Email} passed verification. Current count: {users.Count}");
            }
        }
        catch (Exception ex)
        {
            LogAction($"Background verification failed: {ex.Message}");
        }
    }

    private void RevertUserInvite(UserInvite user)
    {
        try
        {
            DeleteUserFromDatabase(user.Email);
            LogAction($"User with email {user.Email} has been successfully reverted.");
        }
        catch (Exception ex)
        {
            LogAction($"Error reverting user with email {user.Email}: {ex.Message}");
        }
    }

    private void LogAction(string message)
    {
        Console.WriteLine($"[{DateTime.UtcNow}] {message}");
    }

    [HttpPost("DeleteUser")]
    public IActionResult DeleteUser(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("email is required.");
        }

        var users = LoadUsersFromDatabase();
        var userToDelete = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (userToDelete == null)
        {
            ViewBag.ErrorMessage = "User not found.";
            return View("Secured/InviteUser", users);
        }

        DeleteUserFromDatabase(email);
        var updatedUsers = LoadUsersFromDatabase();
        return View("Secured/InviteUser", updatedUsers);
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

    private UserInvite FindUserByEmail(string email)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var command = new SqliteCommand("SELECT Username, Email FROM UserInvites WHERE Email = @Email", connection);
            command.Parameters.AddWithValue("@Email", email);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new UserInvite
                    {
                        Username = reader["Username"]?.ToString() ?? string.Empty,
                        Email = reader["Email"]?.ToString() ?? string.Empty
                    };
                }
            }
        }
        return null;
    }

    private void DeleteUserFromDatabase(string email)
    {
        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = new SqliteCommand("DELETE FROM UserInvites WHERE Email = @Email", connection);
                command.Parameters.AddWithValue("@Email", email);
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            LogAction($"Error deleting user with email {email}: {ex.Message}");
        }
    }
}
