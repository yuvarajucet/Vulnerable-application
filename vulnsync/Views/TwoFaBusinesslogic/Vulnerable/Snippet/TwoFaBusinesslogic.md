## Vulnerable 2FA Business Logic

### Type 1: Skip 2FA with on this device
```csharp
    /// <summary>
    /// Validates the two-factor authentication code in a vulnerable way
    /// WARNING: This is an example of insecure implementation for educational purposes
    /// </summary>
    /// <param name="code">The 2FA verification code provided by the user</param>
    /// <param name="rememberDevice">Flag indicating if the device should be remembered for future logins</param>
    /// <returns>IActionResult redirecting to appropriate page based on validation</returns>
    [HttpPost("VulnerableValidateTwoFactor")]
    public IActionResult VulnerableValidateTwoFactor(string code, string rememberDevice)
    {
        var userEmail = TempData["PendingTwoFaEmail"] as string;
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("VulnerableLogin");
        }
        if (code != ValidTwoFaCode)
        {
            TempData["Error"] = "Invalid 2FA code";
            TempData.Keep("PendingTwoFaEmail");
            return RedirectToAction("VulnerableTwoFactorAuth");
        }
        if (rememberDevice?.ToLower() == "on")
        {
            // Set cookie to skip 2FA for this device
            Response.Cookies.Append("SkipTwoFa", Base64Encode("true"), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(30),
                HttpOnly = true
            });
        }
        return RedirectToAction("VulnerableProfile");
    }
```

<br>

```csharp
/// <summary>
/// Authenticates a user and determines whether to bypass 2FA based on cookie presence.
/// WARNING: This is an example of insecure implementation for educational purposes
/// </summary>
/// <param name="username">The username provided by the user</param>
/// <param name="password">The password provided by the user</param>
/// <returns>IActionResult redirecting to either Profile page (if 2FA is skipped), 
/// TwoFactorAuth page, or Login page (if authentication fails)</returns>
[HttpPost("login")]
public IActionResult Login(string username, string password)
{
    var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
    if (user == null)
    {
        return RedirectToAction("Login");
    }
    // If the cookie is present, skip 2FA and redirect to profile
    if(Base64Decode(Request.Cookies["SkipTwoFa"]) == "true")
    {
        return RedirectToAction("Profile");
    }
    return RedirectToAction("TwoFactorAuth");
}
```
<br>

### Type 2: Skip 2FA with business logic

```csharp
/// <summary>
/// Authenticates a user and determines whether to bypass 2FA based on cookie presence.
/// WARNING: This is an example of insecure implementation for educational purposes
/// </summary>
/// <param name="username">The username provided by the user</param>
/// <param name="password">The password provided by the user</param>
/// <returns>IActionResult redirecting to either Profile page (if 2FA is skipped), 
/// TwoFactorAuth page, or Login page (if authentication fails)</returns>
[HttpPost("login")]
public IActionResult Login(string username, string password)
{
    var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
    if (user == null)
    {
        return RedirectToAction("Login");
    }

    var result = ValidateMFA(user.Email);
    if(result)
    {
        return RedirectToAction("TwoFactorAuth");
    }
    
    return RedirectToAction("Profile");
}
```
<br>

```csharp
/// <summary>
/// Validates the MFA code based on the email
/// </summary>
/// <param name="email">The email of the user</param>
/// <returns>A boolean indicating whether the MFA code is valid</returns>
public bool ValidateMFA(string email)
{
    try
    {
        // Check if need to skip MFA or not
        var result =  Base64Decode(Request.Cookies["SkipTwoFa"]) == "true";
        return result;
    }
    catch (Exception ex)
    {
        // exception logging here
        return false;
    }
}
```
