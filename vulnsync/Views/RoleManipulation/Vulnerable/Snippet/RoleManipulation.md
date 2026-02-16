### <u>Role Manipulation</u>

##### **Description**
Will see the vulernable role manipulation application.


In this application, the admin has an option to invite users via email. When a user receives an invite, they can click the link or use the provided details, including a username and temporary password, to log in. After logging in, the invited user must fill in their personal details to complete profile creation.

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]
public IActionResult AddInviteUserDetails(string role, string address, string personalDetails)
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
    
    var users = GetUsers();
    var user = users.FirstOrDefault(u => u.Id == id); 
    //user easy change the role and gain the access for admin portal
    if(user != null)
    {
        user.Role = role;
        user.Address = address;
        user.PersonalDetails = personalDetails;
    }
    SaveUsers(users);
    return RedirectToAction("Dashboard", new { id = id});
}
``````
<br/>

Attack Technique: In this flow, while the user is entering their personal details to complete profile creation, the role is passed along with the personal details. This allows an attacker to easily modify the role to 'admin' instead of 'user' in order to gain unauthorized access to the admin panel.

