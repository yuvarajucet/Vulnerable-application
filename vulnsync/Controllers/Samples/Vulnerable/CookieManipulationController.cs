using Microsoft.AspNetCore.Mvc;
using VulnSync.Models.RoleManipulation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using Markdig.Extensions.Alerts;

namespace VulnSync.Controllers.Samples.Vulnerable
{
    [Route("vulnerable/[controller]")]
    public class CookieManipulationController : Controller
    {

        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "users.json");

        // Read users from the JSON file
        private List<User> GetUsers()
        {
            var userDetailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "CookieManipulation", "UserDetails.json");
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


        // Save users to the JSON file (if needed for further functionality)
        private void SaveUsers(List<User> users)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(users);
                var userDetailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "CookieManipulation", "UserDetails.json");
                System.IO.File.WriteAllText(userDetailsPath, jsonData);
            }
            catch (IOException ex)
            {
                System.Console.WriteLine($"Error saving data to file: {ex.Message}");
            }
        }

        // GET: Display login page
        [HttpGet]
        public IActionResult Index()
        {
            return View("Vulnerable/Index");
        }

        [HttpPost("CookieManipulationLogin")]
        public IActionResult CookieManipulationLogin(string username, string password)
        {
            // Get the users list (replace this with the actual method to get users)
            var users = GetUsers(); 

            // Find the user that matches the username and password
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

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
            
            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt64(TempData["CurrentUser"])); // Find the logged-in user by id
            if (user != null)
            {
                if (user.Role == "User")
                {
                    TempData["CurrentCustomer"] = user.Id;
                    return View("Vulnerable/UserPanel", user); // User view
                }
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
        [HttpGet("CustomerEditOption")]
        public IActionResult CustomerEditOption(int id)
        {
            var users = GetUsers(); // Get all users
            var user = users.FirstOrDefault(u => u.Id == id); // Find the user by ID

            if (user != null)
            {
                return View("Vulnerable/CustomerEditOption", user); // Pass the user to the view for editing
            }

            return RedirectToAction("Index"); // If no user found, redirect to login
        }

        [HttpPost("SaveUser")]
        public IActionResult SaveUser(User updatedUser)
        {
            if (TempData["CurrentCustomer"] == null)
            {
                TempData["CurrentCustomer"] = TempData["Customer"];
                TempData["Customer"] = TempData["CurrentCustomer"];
            }
            else
            {
                TempData["Customer"] = TempData["CurrentCustomer"];
            }

            // Generate OTP for the user
            var otp = GenerateOTP();
            int userId = Convert.ToInt32(TempData["CurrentCustomer"]);
            TempData["CurrentCustomer"] = userId;
            TempData["PendingUser"] = updatedUser.Email;
            if (Request.Cookies.ContainsKey("OTPVerification") && Request.Cookies["OTPVerification"] == "Q29tcGxldGVk")
            {
                return RedirectToAction("SaveUserAfterOTP");
            }
            else
            {
                TempData["OTP"] = otp;
                // Store OTP in a JSON file
                SaveOTPToFile(userId, otp);
            }
            // Redirect to OTP verification page
            return View("Vulnerable/VerifyOTP");
        }

        // POST: Verify OTP
        [HttpPost("VerifyOTP")]
        public IActionResult VerifyOTP(string enteredOtp)
        {
            if (TempData["CurrentCustomer"] == null)
            {
                TempData["CurrentCustomer"] = TempData["User"];
                TempData["User"] = TempData["CurrentCustomer"];
            }
            else
            {
                TempData["User"] = TempData["CurrentCustomer"];
            }
            
            var userId = Convert.ToInt32(TempData["User"]);
            TempData["User"] = userId;

            // Retrieve stored OTP
            string storedOtp = GetOTPFromFile(userId);

            if (storedOtp == enteredOtp)
            {
                // Create a cookie to indicate OTP verification is complete
                Response.Cookies.Append("OTPVerification", "Q29tcGxldGVk", new CookieOptions { HttpOnly = true, Expires = DateTime.UtcNow.AddMinutes(15) });

                TempData["UserDetail"] = userId;
                return RedirectToAction("SaveUserAfterOTP");
            }

            ViewBag.Error = "Invalid OTP. Please try again.";
            return View("Vulnerable/VerifyOTP");
        }
        
        [HttpGet("SaveUserAfterOTP")]
        public IActionResult SaveUserAfterOTP()
        {
            if (!Request.Cookies.ContainsKey("OTPVerification") || Request.Cookies["OTPVerification"] != "Q29tcGxldGVk")
            {
                return View("Vulnerable/Index"); // Redirect to login if OTP verification is not complete
            }

            var users = GetUsers(); // Get all users
            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt32(TempData["UserDetail"])); // Find the user by ID
            if (user != null)
            {
                user.Email = TempData["PendingUser"].ToString();

                // Save the updated list of users back to your data source
                SaveUsers(users); // Implement the SaveUsers method to save to your data source
                TempData["CurrentUser"] = user.Id;

                return RedirectToAction("Dashboard");
            }

            return RedirectToAction("Index");
        }

        // Helper: Generate OTP
        private int GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999); // Generate a 6-digit OTP
        }
        
        // Helper: Save OTP to JSON (using System.Text.Json)
        private void SaveOTPToFile(int userId, int otp)
        {
            var otpData = new Dictionary<long, int>();

            if (System.IO.File.Exists("otps.json"))
            {
                var json = System.IO.File.ReadAllText("Database/CookieManipulation/otps.json");
                otpData = JsonSerializer.Deserialize<Dictionary<long, int>>(json) ?? new Dictionary<long, int>();
            }

            otpData[userId] = otp;

            // Serialize the updated dictionary to JSON and save it
            var updatedJson = JsonSerializer.Serialize(otpData);
            System.IO.File.WriteAllText("Database/CookieManipulation/otps.json", updatedJson);
        }

        // Helper: Retrieve OTP from JSON (using System.Text.Json)
        private string GetOTPFromFile(long userId)
        {
            const string filePath = "Database/CookieManipulation/otps.json"; // Explicitly define the file path

            try
            {
                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Read the content of the JSON file
                    var json = System.IO.File.ReadAllText(filePath);

                    // Deserialize the JSON into a dictionary with string OTPs
                    var otpData = JsonSerializer.Deserialize<Dictionary<long, int>>(json);

                    // Check if the data is not null and contains the userId
                    if (otpData != null && otpData.ContainsKey(userId))
                    {
                        return otpData[userId].ToString(); // Return the OTP as a string
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging (you can replace this with actual logging)
                Console.WriteLine($"Error reading OTP file: {ex.Message}");
            }

            return null; // Return null if the OTP is not found or an error occurs
        }

    }
}
