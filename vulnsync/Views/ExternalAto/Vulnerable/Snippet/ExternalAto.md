## <u>External Login Account takeover</u>

##### **Description**
Will see the external login account takeover due to improper external user creation process.

#### Vulnerable external account creation handler
```csharp
[HttpPost("oauth/callback")]
public IActionResult VulnerableOAuthCallback(string username, string email)
{
    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
    {
        DatabaseResponseModel result = UpdateNewUser(username, email, Convert.ToBase64String(Encoding.UTF8.GetBytes("NaN")));
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
DatabaseResponseModel result = UpdateNewUser(username, email, Convert.ToBase64String(Encoding.UTF8.GetBytes("NaN")));
```

on this code block we can see that the `Convert.ToBase64String(Encoding.UTF8.GetBytes("NaN"))` is used as a password for the user.
This is a static password which is used for all the users. And also the password is encoded using encryption. which is used to process a login request.
This will lead to account takeover of the external user.

<br/>
here you can see the login process

```csharp
[HttpPost]
public IActionResult SecuredLogin(string username, string password)
{
    using (var connection = new SqliteConnection(_connectionString))
    {
        connection.Open();
        
        string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND Password = @Password";
        using (var command = new SqliteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Username", username);
            var base64EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
            command.Parameters.AddWithValue("@Password", base64EncodedPassword);
            
            var count = Convert.ToInt32(command.ExecuteScalar());
            
            if (count == 1)
            {
                // User authenticated successfully
                ViewBag.Username = username;
                return View("Secured/Profile");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password";
                return View("Secured/ExternalAto");
            }
        }
    }
}
```

<br/>

```csharp
var base64EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
```

in this code block we encrypt the password and compare with database so the exploit working fine.