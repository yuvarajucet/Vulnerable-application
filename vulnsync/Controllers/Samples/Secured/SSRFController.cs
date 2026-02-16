using Microsoft.AspNetCore.Mvc;
using VulnSync.Models.RoleManipulation;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

namespace VulnSync.Controllers.Samples.Secured
{
    [Route("secured/[controller]")]
    public class SSRFController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly IDataProtector _protector;

        // Combined constructor to inject both HttpClient and Data Protection
        public SSRFController(HttpClient httpClient, IDataProtectionProvider provider)
        {
            _httpClient = httpClient;
            _protector = provider.CreateProtector("UserIdProtector");
        }

        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "users.json");

        // Read users from the JSON file
        private List<User> SecuredGetUsers()
        {
            var userDetailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "RoleManipulation", "UserDetails.json");
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
                    System.Console.WriteLine("The JSON file is empty.");
                    return new List<User>();
                }

                // Log the content for debugging (careful with sensitive data)
                System.Console.WriteLine($"JSON Data: {jsonData}");

                var users = JsonSerializer.Deserialize<List<User>>(jsonData);
                if (users == null || users.Count == 0)
                {
                    System.Console.WriteLine("No users found or deserialization failed.");
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

        // GET: Display login page
        [HttpGet]
        public IActionResult Index()
        {
            return View("Secured/Index");
        }

        [HttpPost("SSRFLogin")]
        public IActionResult SecuredSSRFLogin(string username, string password)
        {
            // Get the users list (replace this with the actual method to get users)
            var users = SecuredGetUsers(); 

            // Find the user that matches the username and password
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                TempData["CurrentUser"] = user.Id;
                return RedirectToAction("SecuredDashboard");
            }

            // If no user is found or login fails, show error
            ViewBag.Error = "Invalid username or password";
            return View("Secured/Index");
        }
        
        // GET: Admin dashboard or User panel view
        [HttpGet("SecuredDashboard")]
        public IActionResult SecuredDashboard()
        {
            var users = SecuredGetUsers();
            if (TempData["CurrentUser"] == null)
            {
                TempData["CurrentUser"] = TempData["Reload"];
                TempData["Reload"] = TempData["CurrentUser"];
            }
            else
            {
                string currentUser = TempData["CurrentUser"].ToString();
                TempData["Reload"] = currentUser;
            }
            
            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt64(TempData["CurrentUser"])); // Find the logged-in user by id
            if (user != null)
            {
                string encryptedUserId = _protector.Protect(user.Id.ToString()); // Encrypt user ID
                ViewBag.EncryptedUserId = encryptedUserId; // Pass encrypted ID to View
                TempData["CurrentCustomer"] = user.Id;
                return View("Secured/Dashboard", user);
            }

            // If no user is found, redirect to the index page (login)
            return RedirectToAction("Index");
        }

        // GET: View user details (for demonstration purposes)
        [HttpGet("SecuredUserDetails")]
        public IActionResult SecuredUserDetail()
        {
            var users = SecuredGetUsers(); //
            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt64(TempData["CurrentCustomer"]));
            TempData["CurrentCustomer"] = user.Id;
            return View("Secured/UserPanel", user);
        }
        
        public IActionResult SecuredLogout()
        {
            return View("Secured/Index");
        }
        
        [HttpPost("SecuredGetLicenseDetails")]
        public async Task<IActionResult> SecuredGetLicenseDetails(string licenseDetail)
        {
            if (string.IsNullOrWhiteSpace(licenseDetail))
            {
                return BadRequest("❌ Bad Request: Access denied.");
            }

            if (licenseDetail.Contains("@"))
            {
                return BadRequest("❌ Bad Request: Access denied.");
            }

            int index = licenseDetail.IndexOf("/api/license");
            if (index != -1)
            {
                licenseDetail = "/api/license"; // Keep only the base part
            }
            
            try
            {
                // Define an allowed full URL list (Can be moved to appsettings.json)
                var allowedUrls = new HashSet<string>
                {
                    "http://localhost:5290/api/license",
                };

                string getDetail = "http://localhost:5290"+licenseDetail; // Target API URL

                // Validate the full URL
                if (!allowedUrls.Contains(getDetail))
                {
                    return BadRequest("❌ Forbidden: Untrusted URL.");
                }

                using (var request = new HttpRequestMessage(HttpMethod.Get, getDetail))
                {
                    request.Headers.Add("Origin", "https://localhost:7280"); 
                    request.Headers.Add("Referer", "https://localhost:7280/dashboard"); 

                    HttpResponseMessage response = await _httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string userData = await response.Content.ReadAsStringAsync();
                        ViewBag.LicenseData = userData;
                        return View("Secured/License");
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, $"⚠️ Error: {response.StatusCode}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
                return StatusCode(500, "❌ Internal Server Error: Unable to reach the license server.");
            }
        }
    }
}
