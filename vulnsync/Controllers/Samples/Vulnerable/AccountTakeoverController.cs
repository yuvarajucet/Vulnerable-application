using Microsoft.AspNetCore.Mvc;
using VulnSync.Models.AccountTakeover;
using System.Text.Json;

namespace VulnSync.Controllers.Samples.Vulnerable
{
    [Route("vulnerable/[controller]")]
    public class AccountTakeoverController : Controller
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "users.json");

        // Read users from the JSON file
        private List<User> GetUsers()
        {
            var userDetailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "AccountTakeover", "PasskeyDetails.json");
            System.Console.WriteLine($"Attempting to read file from path: {userDetailsPath}");

            if (!System.IO.File.Exists(userDetailsPath))
            {
                System.Console.WriteLine($"File not found: {userDetailsPath}");
                return new List<User>();
            }

            try
            {
                var jsonData = System.IO.File.ReadAllText(userDetailsPath);
                System.Console.WriteLine("File read successfully.");

                if (string.IsNullOrEmpty(jsonData))
                {
                    return new List<User>();
                }

                var users = JsonSerializer.Deserialize<List<User>>(jsonData);
                if (users == null || users.Count == 0)
                {
                    return new List<User>();
                }

                System.Console.WriteLine($"{users.Count} users loaded successfully.");
                return users;
            }
            catch (JsonException ex)
            {
                System.Console.WriteLine($"Error deserializing JSON data: {ex.Message}");
                return new List<User>();
            }
            catch (IOException ex)
            {
                System.Console.WriteLine($"Error reading the file: {ex.Message}");
                return new List<User>();
            }
        }

        // Save users to the JSON file (if needed for further functionality)
        private void SaveDetails(List<User> users)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(users);
                var userDetailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "AccountTakeover", "PasskeyDetails.json");
                System.IO.File.WriteAllText(userDetailsPath, jsonData);
            }
            catch (IOException ex)
            {
                System.Console.WriteLine($"Error saving data to file: {ex.Message}");
            }
        }

        // Register a new user
        [HttpGet]
        public IActionResult Index()
        {
            return View("Vulnerable/Index");
        }

        [HttpPost("AccountTakeoverlogin")]
        public IActionResult AccountTakeoverlogin(string username, string password)
        {
            // Get the users list (replace this with the actual method to get users)
            var users = GetUsers(); 

            // Find the user that matches the username and password
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                TempData["CurrentUser"] = user.UserId.ToString();
                return RedirectToAction("Dashboard");
            }
            
            ViewBag.Error = "Invalid username or password";
            return View("Vulnerable/Index");
        }
        
        [HttpPost("PasskeyLogin")]
        public IActionResult PasskeyLogin(string passkeypassword)
        {
            // Get the users list (replace this with the actual method to get users)
            var users = GetUsers(); 

            // Find the user that matches the username and password
            var user = users.FirstOrDefault(u => u.Passkeypassword == passkeypassword);

            if (user != null)
            {
                return View("Vulnerable/PasskeyLogin", user);
            }
            
            ViewBag.Error = "Invalid username or password";
            return View("Vulnerable/Index");
        }

        [HttpPost("PasskeyLoginCall")]
        public IActionResult PasskeyLoginCall(string id, string rawid, string type, string attestationObject, string clientDataJSON, string UserAgent, int userid, string passkeypassword)
        {
            var users = GetUsers(); 
            // Find the user that matches the username and password
            var user = users.FirstOrDefault(u => u.UserId == userid && u.Passkeypassword == passkeypassword);
            if (user != null)
            {
                TempData["CurrentUser"] = user.UserId.ToString();
                return RedirectToAction("Dashboard");
            }

            return RedirectToAction("Index");
        }
        
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            var users = GetUsers(); // Get all users
            if (TempData["redirection"] != null)
            {
                string currentUserId = TempData["redirection"].ToString();
                if (currentUserId != null)
                {
                    var user = users.FirstOrDefault(u => u.id == currentUserId); // Find the logged-in user by id
                    if (user != null)
                    {
                        if (user.rawId == null)
                        {
                            int id = user.UserId;
                            savepasskey(id);
                        }
                        TempData["CurrentUserid"]= user.UserId.ToString();
                        // Pass the user details to the view based on their role
                        return View("Vulnerable/Dashboard", user); // User view
                    }
                }
            }
            else
            {
                if (TempData["CurrentUser"] != null)
                {
                    string currentUser = TempData["CurrentUser"].ToString();
                    if (currentUser != null)
                    {
                        var user = users.FirstOrDefault(u => u.UserId == Convert.ToInt64(currentUser)); // Find the logged-in user by id
                        if (user != null)
                        {
                            if (user.rawId == null)
                            {
                                int id = user.UserId;
                                savepasskey(id);
                            }
                            TempData["CurrentUserid"]= user.UserId.ToString();
                            // Pass the user details to the view based on their role
                            return View("Vulnerable/Dashboard", user); // User view
                        }
                    }
                }
                else
                {
                    string currentUserId = TempData["CurrentUserid"].ToString();
                    TempData["CurrentUserid"] = currentUserId;
                    var user = users.FirstOrDefault(u => u.UserId == Convert.ToInt32(currentUserId)); // Find the logged-in user by id
                    if (user != null)
                    {
                        if (user.rawId == null)
                        {
                            int id = user.UserId;
                            savepasskey(id);
                        }
                        // Pass the user details to the view based on their role
                        return View("Vulnerable/Dashboard", user); // User view
                    }
                }
               
            }

            // If no user is found, redirect to the index page (login)
            return RedirectToAction("Index");
        }

        private void savepasskey(int id)
        {
            var users = GetUsers();
            var user = users.FirstOrDefault(u => u.UserId == id); // Find the logged-in user by id
            user.id = GenerateUniqueId();  // Generate a unique id
            user.rawId = user.id; // rawId is same as id
            user.type = "public-key";
            user.clientDataJSON = GenerateRandomString(64);  // Random string for clientDataJSON
            user.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:133.0) Gecko/20100101 Firefox/133.0"; // Fixed UserAgent
            SaveDetails(users);
        }
        
        [HttpPost("SaveUserDetail")]
        public IActionResult SaveUserDetail(User updatedUser)
        {
            var users = GetUsers(); // Get all users
            var user = users.FirstOrDefault(u => u.UserId == updatedUser.UserId); // Find the user by ID

            if (user != null)
            {
                // Update user details
                user.Email = updatedUser.Email;
                user.PersonalDetails = updatedUser.PersonalDetails;

                // Save the updated list of users back to your data source (e.g., JSON, database)
                SaveDetails(users); // Implement the SaveUsers method to save to your data source
                TempData["CurrentUser"] = user.UserId.ToString();
                // Redirect back to the dashboard after saving
                return RedirectToAction("Dashboard");
            }

            return RedirectToAction("Index"); // If user not found, redirect to login
        }
        
        [HttpGet("CustomerEditOption")]
        public IActionResult CustomerEditOption(int id)
        {
            var users = GetUsers(); // Get all users
            var user = users.FirstOrDefault(u => u.UserId == id); // Find the user by ID

            if (user != null)
            {
                return View("Vulnerable/Edit", user); // Pass the user to the view for editing
            }

            return RedirectToAction("Index"); // If no user found, redirect to login
        }
        
        [HttpPost("RegisterPasskey")]
        public IActionResult RegisterPasskey(string id, string rawid, string type, string attestationObject, string clientDataJSON, string UserAgent, int userid, string passkeypassword)
        {
            var users = GetUsers(); // Load users from JSON
            var user = users.FirstOrDefault(u => u.UserId == userid); // Find the user by ID

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                TempData["CurrentUser"] = user.UserId.ToString();
                return RedirectToAction("Dashboard");
            }

            // If the passkey is being enabled, generate new passkey details
            if (user.PasskeyStatus == "Disable Passkey")
            {
                user.PasskeyStatus = "Enable Passkey";
                
                // You can also set passkey fields to null or empty when disabling it
                user.Passkeyname = null;
                user.Passkeypassword = null;
                user.id = null;
                user.rawId = null;
                user.clientDataJSON = null;
                user.UserAgent = null;
                TempData["CurrentUser"] = user.UserId.ToString();
            }
            else
            {
                // When enabling passkey, generate new values
                user.PasskeyStatus = "Disable Passkey";
                user.Passkeypassword = passkeypassword;
                user.id = id;
                user.rawId = rawid;
                if (user.id == null)
                {
                    savepasskey(user.UserId);
                }
                TempData["redirection"] = id;
            }

            SaveDetails(users); // Save the updated list to the JSON file
            TempData["Success"] = "Passkey updated successfully.";
            return RedirectToAction("Dashboard");
        }

        // Method to generate a unique ID (you can also use GUID or any other method you prefer)
        private string GenerateUniqueId()
        {
            return Guid.NewGuid().ToString(); // Generates a new unique GUID as a string
        }

        // Method to generate a random string for attestationObject or clientDataJSON
        private string GenerateRandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        public IActionResult VulnerableLogout()
        {
            return View("Vulnerable/Index");
        }
    }
}
