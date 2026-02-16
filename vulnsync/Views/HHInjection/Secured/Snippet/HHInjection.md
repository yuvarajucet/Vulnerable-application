## Secured sample for HHInjection

#### Security Enhancement Explanation:
**Use a Trusted Base URL:**
- Instead of relying on `Request.Host`, the code retrieves the domain from a trusted configuration source (`_configuration["AppSettings:Domain"]`).
- This prevents attackers from injecting malicious host headers to manipulate password reset links.

#### Sample code:
```csharp
[HttpPost]
    [Route("GenerateResetLinkSec")]
    public JsonResult GenerateResetLinkSec([FromBody] ResetPasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            return Json(new { success = false, message = "Email is required!" });
        }
        
        string token = GenerateToken(); // Declare token outside
        
        using (var connection = new SqliteConnection(_idorConnectionString))
        {
            connection.Open();
            
            // Check if the user exists
            var command = new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", connection);
            command.Parameters.AddWithValue("@Email", request.Email);
            
            int userExists = Convert.ToInt32(command.ExecuteScalar());
            
            if (userExists == 0)
            {
                return Json(new { success = false, message = "User not found!" });
            }
            
            // Check if a token already exists for the email
            var checkTokenCommand = new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", connection);
            checkTokenCommand.Parameters.AddWithValue("@Email", request.Email);
            int tokenExists = Convert.ToInt32(checkTokenCommand.ExecuteScalar());
            
            if (tokenExists > 0)
            {
                // Update existing token
                var updateCommand = new SqliteCommand("UPDATE Users SET Token = @Token WHERE Email = @Email", connection);
                updateCommand.Parameters.AddWithValue("@Token", token);
                updateCommand.Parameters.AddWithValue("@Email", request.Email);
                updateCommand.ExecuteNonQuery();
            }
            else
            {
                // Insert new token if none exists
                var insertCommand = new SqliteCommand("INSERT INTO Users (Email, Token) VALUES (@Email, @Token)", connection);
                insertCommand.Parameters.AddWithValue("@Email", request.Email);
                insertCommand.Parameters.AddWithValue("@Token", token);
                insertCommand.ExecuteNonQuery();
            }
        }
        string trustedBaseUrl = _configuration["AppSettings:Domain"];
        // Construct the reset link
        string resetLink = $"{trustedBaseUrl}/Vulnerable/HHInjection/ResetPassword?token={token}&email={Uri.EscapeDataString(request.Email)}";
        
        return Json(new { success = true, resetLink = resetLink });
    }
```
#### Mitigation Strategies:

- **Validate the Host Header:** Accept only trusted hostnames from a predefined allowlist.
- **Use Absolute URLs:** Instead of relying on Request.Host, define the legitimate domain in the configuration.
- **Implement Forwarded Headers Middleware:** In ASP.NET Core, use app.UseForwardedHeaders() to handle proxies securely.
- **Enforce HTTPS and HSTS:** Prevent downgrade attacks and ensure secure communication.
