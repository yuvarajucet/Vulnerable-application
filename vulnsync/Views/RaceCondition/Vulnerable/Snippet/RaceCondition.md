## Vulnerable sample for Race Condition
 
 ### Vulnerability Explanation:
 - The vulnerable code allows multiple users to invite new users simultaneously without proper synchronization.
 - This can lead to a race condition where the user count is checked and updated concurrently by multiple threads, potentially exceeding the free limit.
 - For example, if two users try to invite a new user at the same time, both may pass the user count check and proceed to add new users, resulting in the free limit being exceeded.

 ### Sample Code:
 ```csharp
    [HttpPost("InviteUser")]
    public IActionResult InviteUser(string username, string email)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
        {
            return BadRequest("User name and user email are required.");
        }
        
        var users = LoadUsersFromDatabase();
        if (users.Count > FreeLimit)
        {
            ViewBag.ErrorMessage = "Free limit reached. Upgrade to Pro to invite more users.";
            return View("Vulnerable/InviteUser", users);
        }
        
        var newUser = new UserInvite
        {
            Username = username,
            Email = email,
        };
        
        SaveUserToDatabase(newUser);
        
        var showAllUsers = LoadUsersFromDatabase();
        return View("Vulnerable/InviteUser", showAllUsers);
    }
```
