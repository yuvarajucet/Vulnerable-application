using Microsoft.AspNetCore.Mvc;

namespace VulnSync.Controllers.Samples.Vulnerable;

[Route("vulnerable/[controller]")]
public class SqlInjectionController : Controller
{
        private readonly UserService _userService;
        
        private string _connectionString;

        public SqlInjectionController()
        {
            _connectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database/SqlInjection/database.db")}";
            _userService = new UserService(_connectionString) ?? throw new ArgumentNullException(nameof(_userService));
            _userService.SeedUsers();
        }

        // GET: Display login page
        [HttpGet]
        public IActionResult Index()
        {
            return View("Vulnerable/Index");
        }

        [HttpPost("SqlInjectionLogin")]
        public IActionResult SqlInjectionLogin(string username, string password)
        {
            // Get the users list (replace this with the actual method to get users)
            var user = _userService.AuthenticateUser(username, password);

            if (user != null)
            {
                TempData["CurrentUser"] = user.Id;
                return RedirectToAction("Dashboard");
            }

            // If no user is found or login fails, show error
            ViewBag.Error = "Invalid username or password";
            return View("Vulnerable/Index");
        }
        
        // GET: Admin dashboard or User panel view
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            if (TempData["CurrentUser"] == null)
            {
                TempData["CurrentUser"] = TempData["Reload"];
            }
            else
            {
                int currentUser = Convert.ToInt32(TempData["CurrentUser"]);
                TempData["CurrentUser"] = currentUser;
                TempData["Reload"] = currentUser;
            }
            
            var user = _userService.GetUserDetails(Convert.ToInt32(TempData["CurrentUser"]));
            if (user != null)
            {
                TempData["CurrentCustomer"] = user.Email;
                return View("Vulnerable/Dashboard", user); // User view
            }

            // If no user is found, redirect to the index page (login)
            return RedirectToAction("Index");
        }
        
        [HttpGet("VulnerableLogout")]
        public IActionResult VulnerableLogout()
        {
            Response.Cookies.Delete("OTPVerification");
            return View("Vulnerable/Index");
        }
        
        // GET: Edit User details view
        [HttpGet("Profile")]
        public IActionResult Profile(string username)
        {
            if (username == null)
            {
                return View("Secured/Index");
            }
            
            var user = _userService.Profile(username);
             
            if (user != null && user.Count != 0)
            {
                return View("Vulnerable/Profile", user); // Return only the correct user
            }

            return RedirectToAction("Index"); // If no user found, redirect to login
        }

        
        
}