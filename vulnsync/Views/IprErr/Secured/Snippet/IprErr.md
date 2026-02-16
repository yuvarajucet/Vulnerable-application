## Secured sample for Improper Error Handling

#### 🛡️ Security Improvements
- ✅ **Parameterized Queries** – Prevents SQL Injection.
- ✅ **Internal Logging – Stores** error details securely in logs instead of exposing them to users.
- ✅ **Generic Error Messages** – Prevents attackers from gaining system insights.

#### Secure Code (Proper Error Handling & Secure Querying)
```csharp
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password); // ❗ Ideally, store hashed passwords!
                    
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        return Content("Login successful");
                    }
                    else
                    {
                        return Content("Invalid username or password.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during login"); // ✅ Logs error internally
            return Content("An unexpected error occurred. Please try again later."); // ✅ Generic message for users
        }
    }
```