## Vulnerable sample for HHInjection

#### Vulnerability Explanation:
Host Header Injection is a web security vulnerability that occurs when a server improperly processes user-supplied Host headers. Attackers can manipulate the Host header to bypass security controls, conduct phishing attacks, or exploit other vulnerabilities such as cache poisoning and password reset poisoning.

#### How It Works:
1. The attacker modifies the `Host` header in an HTTP request to a malicious domain.
2. If the server blindly trusts the `Host` header, it may generate links or perform redirections using the attacker's domain.
3. This can lead to session hijacking, password reset token theft, or phishing attacks.

#### Example Attack Scenario:
An application generates password reset links using the `Host` header:
```HTML
POST /GenerateResetLink HTTP/1.1
Host: attacker.com
Content-Type: application/json

{
    "email": "victim@example.com"
}
```
If the server constructs a password reset link as:
```HTML
https://attacker.com/reset-password?token=12345
```
An attacker can trick the victim into clicking the link and stealing the reset token.
#### Vulnerable Code:
```csharp
    [HttpPost]
    [Route("GenerateResetLinkVuln")]
    public JsonResult GenerateResetLinkVuln([FromBody] ResetPasswordRequest request)
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
        // Construct the reset link
        string resetLink = $"{Request.Scheme}://{Request.Host}/Vulnerable/HHInjection/ResetPassword?token={token}&email={Uri.EscapeDataString(request.Email)}";
        
        return Json(new { success = true, resetLink = resetLink });
    }
```