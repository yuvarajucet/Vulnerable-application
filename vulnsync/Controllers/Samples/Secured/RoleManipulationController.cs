using Microsoft.AspNetCore.Mvc;
using VulnSync.Models.RoleManipulation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace VulnSync.Controllers.Samples.Secured
{
    [Route("secured/[controller]")]
    public class RoleManipulationController : Controller
    {

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


        // Save users to the JSON file (if needed for further functionality)
        private void SecuredSaveUsers(List<User> users)
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
        
        // GET: Display login page
        [HttpGet]
        public IActionResult Index()
        {
            return View("Secured/Index");
        }
        
        [HttpPost("AdminSaveUsers")]
        public IActionResult SecuredAdminSaveUsers(User updatedUser)
        {
            // Load the existing user data (assuming GetUsers() reads users from a file)
            var users = SecuredGetUsers(); 

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
                SecuredSaveUsers(users); // Assuming SaveUsers() handles writing to your JSON file
            }
            else
            {
                return NotFound("User not found.");
            }

            // Redirect to a page after saving, for example:
            return RedirectToAction("SecuredUserDetails");
        }

        [HttpPost("SecuredRoleManipulationLogin")]
        public IActionResult SecuredRoleManipulationLogin(string username, string password)
        {
            // Get the users list (replace this with the actual method to get users)
            var users = SecuredGetUsers(); 

            // Find the user that matches the username and password
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                TempData["Role"] = user.Role;
                // Check if the user is an Admin or User and redirect accordingly
                if (user.Role == "Admin")
                {
                    TempData["CurrentUser"] = user.Id;
                    return RedirectToAction("SecuredDashboard");
                }
                else if (user.Role == "User")
                {
                    TempData["CurrentUser"] = user.Id;
                    return RedirectToAction("SecuredDashboard");
                }
            }

            // If no user is found or login fails, show error
            ViewBag.Error = "Invalid username or password";
            return View("Secured/Index");
        }
        
        // GET: Admin dashboard or User panel view
        [HttpGet("SecuredDashboard")]
        public IActionResult SecuredDashboard(int id)
        {
            var users = SecuredGetUsers(); // Get all users
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
                // Pass the user details to the view based on their role
                if (user.Role == "Admin" && TempData["Role"]?.ToString() == "Admin")
                {
                    TempData["CurrentCustomer"] = user.Id;
                    TempData.Remove("Role");
                    return View("Secured/AdminDashboard", user); // Admin view
                }
                else if (user.Role == "User" && TempData["Role"]?.ToString() == "User")
                {
                    TempData["CurrentCustomer"] = user.Id;
                    TempData.Remove("Role");
                    return View("Secured/UserPanel", user); // User view
                }
            }

            // If no user is found, redirect to the index page (login)
            return RedirectToAction("Index");
        }

        
        [HttpGet("SecuredAdminBack")]
        public IActionResult SecuredAdminBack()
        {
            return View("Secured/AdminDashboard");
        }

        // GET: View user details (for demonstration purposes)
        [HttpGet("SecuredUserDetails")]
        public IActionResult SecuredUserDetails()
        {
            var users = SecuredGetUsers(); //
            return View("Secured/UserDetails", users);
        }

        [HttpGet("SecuredCompanyDetails")]
        public IActionResult SecuredCompanyDetails()
        {
            return View("Secured/Company");
        }
        
        public IActionResult SecuredLogout()
        {
            return RedirectToAction("Index", "RoleManipulation");
        }

        [HttpGet("SecuredEditUser")]
        public IActionResult SecuredEditUser(int id)
        {
            var users = SecuredGetUsers(); // Retrieve the list of users
            var user = users.FirstOrDefault(u => u.Id == id); // Find the user with the specified id

            if (user == null)
            {
                // If user is not found, return a "Not Found" view or an appropriate error message
                return NotFound();
            }

            // Pass the found user to the Edit view
            return View("Secured/EditUser", user); // Assuming you have an "EditUser" view
        }

        // Admin panel
        [HttpPost("SecuredEditPersonalDetails")]
        public IActionResult SecuredEditPersonalDetails(User updatedUser)
        {
            // Ensure updatedUser has all the correct data from the form
            var users = SecuredGetUsers();
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
            SecuredSaveUsers(users);

            // Redirect to a page showing user details (or a list page)
            return RedirectToAction("SecuredUserDetails");
        }

        // POST: Add a new user
        [HttpPost("SecuredAddUser")]
        public IActionResult SecuredAddUser(string name, string email, string password, string role, string address, string personalDetails)
        {
            var users = SecuredGetUsers();
            int newId = users.Count > 0 ? users[^1].Id + 1 : 1;

            users.Add(new User
            {
                Id = newId,
                Username = name,
                Email = email,
                Password = password,
                Role = role,
            });

            SecuredSaveUsers(users);
            TempData["SuccessMessage"] = "User added and mail sent successfully.";  // Use TempData for one-time messages.
            return RedirectToAction("SecuredUserDetails");
        }

        // POST: Delete a user
        [HttpPost("SecuredDeleteUser")]
        public IActionResult SecuredDeleteUser(int id)
        {
            var users = SecuredGetUsers();
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

            SecuredSaveUsers(users);
            return RedirectToAction("SecuredUserDetails");
        }
        
        
        // User control
        
        // GET: Edit User details view
        [HttpGet("SecuredCustomerEditOption")]
        public IActionResult SecuredCustomerEditOption(int id)
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

            var users = SecuredGetUsers();// Get all users
            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt64(TempData["CurrentCustomer"])); // Find the user by ID

            if (user != null)
            {
                return View("Secured/CustomerEditOption", user); // Pass the user to the view for editing
            }

            return RedirectToAction("Index"); // If no user found, redirect to login
        }

        // POST: Save edited user details
        [HttpPost("SecuredSaveUser")]
        public IActionResult SecuredSaveUser(User updatedUser)
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

            var users = SecuredGetUsers(); // Get all users
            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt64(TempData["CurrentCustomer"])); // Find the user by ID

            if (user != null)
            {
                // Update user details
                user.Username = updatedUser.Username;
                user.Email = updatedUser.Email;
                user.Address = updatedUser.Address;
                user.PersonalDetails = updatedUser.PersonalDetails;

                // Save the updated list of users back to your data source (e.g., JSON, database)
                SecuredSaveUsers(users); // Implement the SaveUsers method to save to your data source
                TempData["CurrentUser"] = user.Id;
                // Redirect back to the dashboard after saving
                return RedirectToAction("SecuredDashboard");
            }

            return RedirectToAction("Index"); // If user not found, redirect to login
        }
        
        [HttpPost("AddCustomerDetails")]
        public IActionResult SecuredAddCustomerDetails(string address, string personalDetails)
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

            var users = SecuredGetUsers();
            var user = users.FirstOrDefault(u => u.Id == Convert.ToInt64(TempData["CurrentCustomer"]));  
            
            if(user != null)
            {
                user.Address = address;
                user.PersonalDetails = personalDetails;
            }

            SecuredSaveUsers(users);
            TempData["CurrentUser"] = user.Id;
            return RedirectToAction("SecuredDashboard");
        }

    }
}
