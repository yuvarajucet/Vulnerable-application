## Secured 2FA Business Logic

### Type 1: Mitigate Skip 2FA with on this device
```csharp
/// <summary>
/// Validates the two-factor authentication code in a secure way
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

    string skipTwoFaCookie = Request.Cookies["SkipTwoFa"];
    bool skipTwoFafromClient = Base64Decode(skipTwoFaCookie);

    if(skipTwoFafromClient)
    {
        return RedirectToAction("VulnerableProfile");
    }
}
```

<br>

```csharp
/// <summary>
/// Decodes the base64 encoded data and checks if the user needs to skip 2FA
/// </summary>
/// <param name="base64EncodedData">The base64 encoded data</param>
/// <param name="email">The email of the user</param>
/// <returns>A boolean indicating whether the user needs to skip 2FA</returns>
private bool Base64Decode(string base64EncodedData, string email)
    {
        try {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            string needToSkip = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            string[] needToSkipArray = needToSkip.Split("_");
            string userEmail = GetEmailFromId(needToSkipArray[0]);

            // Check the skip cookie is for current user or not
            if (needToSkipArray[1] == "true" && userEmail == email)
            {
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

```

<br>

### Type 2: Mitigate Skip 2FA with business logic

```csharp
/// <summary>
/// Authenticates a user and determines whether to bypass 2FA based on cookie presence.
/// </summary>
/// <param name="username">The username provided by the user</param>
/// <param name="password">The password provided by the user</param>
/// <returns>IActionResult redirecting to either Profile page (if 2FA is skipped), 
/// TwoFactorAuth page, or Login page (if authentication fails)</returns>
[HttpPost("login")]
public IActionResult Login(string username, string password)
{
    var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
    if(user == null)
    {
        return RedirectToAction("Login");
    }

    if(Base64Decode(Request.Cookies["SkipTwoFa"], user.Email))
    {
        return RedirectToAction("Profile");
    }

    return RedirectToAction("TwoFactorAuth");
}

```