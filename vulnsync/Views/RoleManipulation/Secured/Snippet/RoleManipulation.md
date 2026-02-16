### <u>Role Manipulation</u>

##### **Description**
Will see the Secure role manipulation application.


In this application, the admin has an option to invite users via email. When a user receives an invite, they can click the link to log in. After logging in, the invited user must fill in their personal details to complete profile creation

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize] // Ensure only authorized users can access this endpoint
public IActionResult UpdateInviteUserDetails(string address, string personalDetails)
{
    // Get the user ID from the current authenticated user context
    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Unauthorized("User ID not found in claims.");
    }
    
    // Parse the user ID from claims
    if (!int.TryParse(userIdClaim.Value, out int userId))
    {
        return BadRequest("Invalid user ID.");
    }
    
    // Validate the input parameters
    if (id <= 0 || string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(personalDetails))
    {
        return BadRequest("Invalid input parameters.");
    }
    
    // Use a method to sanitize input to prevent injection attacks
    address = SanitizeInput(address);
    personalDetails = SanitizeInput(personalDetails);
    
    // Retrieve the list of users securely
    var users = SecuredGetUsers();
    if (users == null)
    {
        return NotFound("User list not found.");
    }
    
    // Find the user by ID, adding a null check
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user == null)
    {
        return NotFound($"User not found.");
    }
    
    // Additional security check to ensure that the logged-in user has permission to update the specified user's details
    if (!UserHasPermissionToModify(id))
    {
        return Forbid("You do not have permission to modify this user's details.");
    }
    
    // Update user details with validated input
    user.Address = address;
    user.PersonalDetails = personalDetails;
    
    // Save updated users list securely
    SecuredSaveUsers(users);
    
    // Redirect to a secure dashboard page with appropriate status
    return RedirectToAction("SecuredDashboard", new { id = id });
}

// Example input sanitization function
private string SanitizeInput(string input)
{
    // Sanitize input to prevent XSS or other injection attacks
    return input.Replace("<", "").Replace(">", "").Trim();
}

// Example authorization check function
private bool UserHasPermissionToModify(int userId)
{
    // Logic to determine if the current user is authorized to modify this user's details
    // This could involve checking the logged-in user's role, ownership of data, etc.
    return true; // Replace with real permission logic
}

``````
<br/>

Attacker or user not able to change role in this process.
