### <u>External Login Account takeover Prevention</u>

##### **Description**
Will see the external login account takeover mitigation via registring new user with proper credentials.

#### Secured external account creation handler
```csharp
[HttpPost("oauth/callback")]
public IActionResult VulnerableOAuthCallback(string username, string email)
{
    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
    {
        DatabaseResponseModel result = UpdateNewUser(username, email,"NaN");
        if (result.IsUserCreated || result.IsUserExisit)
        {
            ViewBag.Username = username;
            return View("Vulnerable/Profile");
        }
        else
        {
            ViewBag.ErrorMessage = "Failed to create new user";
        }
    }
    return View("Vulnerable/ExternalAto");
}
```
<br/>

```csharp
DatabaseResponseModel result = UpdateNewUser(username, email, "NaN");
```

on this code block we can see that the `"NaN"` is used as a password for the user.
This is a static password which is used for all the users. But in this case we directly storing the `NaN` string as a password field. So if attacker try to login with this password they need to enter in login filed and the login flow will encrypt the value and compare with database. So on that case 
attacker will get response as `username or password is incorrect`.