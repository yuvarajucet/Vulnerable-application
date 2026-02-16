using Microsoft.AspNetCore.Mvc;
using VulnSync.Models.RoleManipulation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Markdig.Extensions.Alerts;

namespace VulnSync.Controllers.Samples.Vulnerable
{
    [Route("vulnerable/[controller]")]
    public class RoleManipulationController : Controller
    {

        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "users.json");

        // Read users from the JSON file
        private List<User> GetUsers()
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


        // Save users to the JSON file (if needed for further functionality)
        private void SaveUsers(List<User> users)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(users);
                var userDetailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "RoleManipulation", "UserDetails.json");
                System.IO.File.WriteAllText(userDetailsPath, jsonData);
            }
            catch (IOException ex)
            {
                System.Console.WriteLine($"Error saving data to file: {ex.Message}");
            }
        }
        
        [HttpPost("AdminSaveUsers")]
        public IActionResult AdminSaveUsers(User updatedUser)
        {
            // Load the existing user data (assuming GetUsers() reads users from a file)
            var users = GetUsers(); 

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            // Find the user to update based on Id
            var userToEdit = users.FirstOrDefault(u => u.Id == updatedUser.Id);

            if (userToEdit != null)
            {
                // Update user properties
                userToEdit.Username = updatedUser.Username;
                userToEdit.Address = updatedUser.Address;
                userToEdit.PersonalDetails = updatedUser.PersonalDetails;
                userToEdit.Email = updatedUser.Email;
                userToEdit.Role = updatedUser.Role.ToString();

                // Save the modified list back to the file
                SaveUsers(users); // Assuming SaveUsers() handles writing to your JSON file
            }
            else
            {
                return NotFound("User not found.");
            }

            // Redirect to a page after saving, for example:
            return RedirectToAction("UserDetails");
        }

        // GET: Display login page
        [HttpGet]
        public IActionResult Index()
        {
            return View("Vulnerable/Index");
        }

        [HttpPost("RoleManipulationLogin")]
        public IActionResult RoleManipulationLogin(string username, string password)
        {
            // Get the users list (replace this with the actual method to get users)
            var users = GetUsers(); 

            // Find the user that matches the username and password
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Check if the user is an Admin or User and redirect accordingly
                if (user.Role == "Admin")
                {
                    TempData["CurrentUser"] = user.Id;
                    return RedirectToAction("Dashboard");
                }
                else if (user.Role == "User")
                {
                    TempData["CurrentUser"] = user.Id;
                    return RedirectToAction("Dashboard");
                }
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
                // Pass the user details to the view based on their role
                if (user.Role == "Admin")
                {
                    TempData["CurrentCustomer"] = user.Id;
                    return View("Vulnerable/AdminDashboard", user); // Admin view
                }
                else if (user.Role == "User")
                {
                    TempData["CurrentCustomer"] = user.Id;
                    return View("Vulnerable/UserPanel", user); // User view
                }
            }

            // If no user is found, redirect to the index page (login)
            return RedirectToAction("Index");
        }

        
        [HttpGet("AdminBack")]
        public IActionResult AdminBack()
        {
            return View("Vulnerable/AdminDashboard");
        }

        // GET: View user details (for demonstration purposes)
        [HttpGet("UserDetails")]
        public IActionResult UserDetails()
        {
            var users = GetUsers(); //
            return View("Vulnerable/UserDetails", users);
        }

        [HttpGet("CompanyDetails")]
        public IActionResult CompanyDetails()
        {
            return View("Vulnerable/Company");
        }
        
        public IActionResult VulnerableLogout()
        {
            return View("Vulnerable/Index");
        }

        [HttpGet("EditUser")]
        public IActionResult EditUser(int id)
        {
            var users = GetUsers(); // Retrieve the list of users
            var user = users.FirstOrDefault(u => u.Id == id); // Find the user with the specified id

            if (user == null)
            {
                // If user is not found, return a "Not Found" view or an appropriate error message
                return NotFound();
            }

            // Pass the found user to the Edit view
            return View("Vulnerable/EditUser", user); // Assuming you have an "EditUser" view
        }

        // Admin panel
        [HttpPost("EditPersonalDetails")]
        public IActionResult EditPersonalDetails(User updatedUser)
        {
            // Ensure updatedUser has all the correct data from the form
            var users = GetUsers();
            var user = users.FirstOrDefault(u => u.Id == updatedUser.Id);

            if (user == null)
            {
                return NotFound();
            }

            // Update user details
            user.Username = updatedUser.Username;
            user.Address = updatedUser.Address;
            user.PersonalDetails = updatedUser.PersonalDetails;

            // Save the updated list of users back to the file
            SaveUsers(users);

            // Redirect to a page showing user details (or a list page)
            return RedirectToAction("UserDetails");
        }

        // POST: Add a new user
        [HttpPost("AddUser")]
        public IActionResult AddUser(string name, string email, string password, string role, string address, string personalDetails)
        {
            var users = GetUsers();
            int newId = users.Count > 0 ? users[^1].Id + 1 : 1;

            users.Add(new User
            {
                Id = newId,
                Username = name,
                Email = email,
                Password = password,
                Role = role,
            });

            SaveUsers(users);
            TempData["SuccessMessage"] = "User added and mail sent successfully.";  // Use TempData for one-time messages.
            
            return RedirectToAction("UserDetails");
        }

        // POST: Delete a user
        [HttpPost("DeleteUser")]
        public IActionResult DeleteUser(int id)
        {
            var users = GetUsers();
            var userToRemove = users.Find(u => u.Id == id);
            if (userToRemove != null)
            {
                users.Remove(userToRemove);
            }

            // Maintain at least 10 default users by adding placeholders if necessary
            while (users.Count < 10)
            {
                int newId = users.Count > 0 ? users[^1].Id + 1 : 1;
                users.Add(new User
                {
                    Id = newId,
                    Username = $"Default User {newId}",
                    Address = "Default Address",
                    PersonalDetails = $"Name: Default User {newId}, Mobile: N/A, Salary: $0, Address: Default Address, Bank Details: N/A"
                });
            }

            SaveUsers(users);
            return RedirectToAction("UserDetails");
        }
        
        
        // User control
        
        // GET: Edit User details view
        [HttpGet("CustomerEditOption")]
        public IActionResult CustomerEditOption(int id)
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
            var users = GetUsers(); // Get all users
            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt64(TempData["CurrentCustomer"])); // Find the user by ID

            if (user != null)
            {
                return View("Vulnerable/CustomerEditOption", user); // Pass the user to the view for editing
            }

            return RedirectToAction("Index"); // If no user found, redirect to login
        }

        // POST: Save edited user details
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
            var users = GetUsers(); // Get all users
            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt64(TempData["CurrentCustomer"])); // Find the user by ID

            if (user != null)
            {
                // Update user details
                user.Username = updatedUser.Username;
                user.Email = updatedUser.Email;
                user.Address = updatedUser.Address;
                user.PersonalDetails = updatedUser.PersonalDetails;
                user.Password = updatedUser.Password;

                // Save the updated list of users back to your data source (e.g., JSON, database)
                SaveUsers(users); // Implement the SaveUsers method to save to your data source
                TempData["CurrentUser"] = user.Id;
                // Redirect back to the dashboard after saving
                return RedirectToAction("Dashboard");
            }

            return RedirectToAction("Index"); // If user not found, redirect to login
        }
        
        
        // POST: Add a new user
        [Route("add/newUser")]
        [HttpPost("AddCustomerDetails")]
        public IActionResult AddCustomerDetails(int id, string role, string address, string personalDetails)
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

            var users = GetUsers();
            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt64(TempData["CurrentCustomer"])); 
            
            if(user != null)
            {
                user.Role = role;
                user.Address = address;
                user.PersonalDetails = personalDetails;
            }

            SaveUsers(users);
            TempData["CurrentUser"] = user.Id;
            return RedirectToAction("Dashboard");
        }

    }
}
