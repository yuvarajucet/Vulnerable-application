using Microsoft.AspNetCore.Mvc;
using VulnSync.Models.RoleManipulation;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

namespace VulnSync.Controllers.Samples.Vulnerable
{
    [Route("vulnerable/[controller]")]
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

        private List<User> GetUsers()
        {
            var userDetailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "RoleManipulation", "UserDetails.json");

            if (!System.IO.File.Exists(userDetailsPath))
            {
                return new List<User>();
            }

            try
            {
                var jsonData = System.IO.File.ReadAllText(userDetailsPath);
                return JsonSerializer.Deserialize<List<User>>(jsonData) ?? new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Vulnerable/Index");
        }

        [HttpPost("SSRFLogin")]
        public IActionResult SSRFLogin(string username, string password)
        {
            var users = GetUsers();
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                TempData["CurrentUser"] = user.Id;
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid username or password";
            return View("Vulnerable/Index");
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            var users = GetUsers();

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

            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt64(TempData["CurrentUser"]));

            if (user != null)
            {
                string encryptedUserId = _protector.Protect(user.Id.ToString()); // Encrypt user ID
                ViewBag.EncryptedUserId = encryptedUserId; // Pass encrypted ID to View
                TempData["CurrentCustomer"] = user.Id;
                return View("Vulnerable/Dashboard");
            }

            return RedirectToAction("Index");
        }

        [HttpGet("UserDetails")]
        public IActionResult UserDetail()
        {
            var users = GetUsers();
            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt64(TempData["CurrentCustomer"]));
            TempData["CurrentCustomer"] = user?.Id;
            return View("Vulnerable/UserPanel", user);
        }
        
        public IActionResult VulnerableLogout()
        {
            return View("Vulnerable/Index");
        }

        [HttpPost("GetLicenseDetails")]
        public async Task<IActionResult> GetLicenseDetails(string licenseDetail)
        {
            
            if (string.IsNullOrWhiteSpace(licenseDetail))
            {
                return BadRequest("❌ Bad Request: User API URL is required.");
            }

            if (licenseDetail.Contains("/api/license"))
            {
                int index = licenseDetail.IndexOf("/api/license");
                if (index != -1)
                {
                    licenseDetail = "/api/license"; // Keep only the base part
                }
            }
            
            try
            {
                string getDetail = "http://localhost:5290" + licenseDetail;
                var request = new HttpRequestMessage(HttpMethod.Get, getDetail);
                request.Headers.Add("Origin", "https://localhost:7280");
                request.Headers.Add("Referer", "https://localhost:7280/dashboard");

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string userData = await response.Content.ReadAsStringAsync();
                    ViewBag.LicenseData = userData;
                    return View("Vulnerable/License");
                }

                return StatusCode((int)response.StatusCode, $"⚠️ Error: {response.StatusCode}");
            }
            catch (HttpRequestException)
            {
                return BadRequest("❌ Bad Request: Unable to reach the user server.");
            }
        }
    }
}
