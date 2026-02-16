## Vulnerable sample for Improper Error Handling

#### Vulnerability in This Code
- **Exposes Detailed Errors** – If an exception occurs, it returns exact database error details, which an attacker can exploit.

#### Vulnerable Code (Exposes Detailed Errors)
```csharp
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        try
        {
            using (SqliteConnection conn = new SqliteConnection(_idorConnectionString))
            {
                var base64EncodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
                conn.Open();
                string query = $"SELECT * FROM Users WHERE Username = '{username}' AND Password = '{base64EncodedPassword}'";
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return Content("Login successful");
                    }
                    else
                    {
                        return Content("Invalid credentials.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return Content($"Error: {ex.Message}, Stacktrace: {ex.StackTrace}"); // ❌ Vulnerability: Exposes database and system details
        }
    }
```