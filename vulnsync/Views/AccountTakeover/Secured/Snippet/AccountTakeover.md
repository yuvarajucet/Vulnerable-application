### <u>Account Takeover using userId</u>

##### **Description**
Will see the secure application.


This application has a passkey feature, allowing users to register a passkey for their account.

```csharp
[HttpPost]
[ValidateAntiForgeryToken] // Ensure CSRF protection is applied
[Authorize] // Ensure the user is authenticated
public IActionResult RegisterPasskey(PasskeyRegistrationModel model)
{
    // Input validation
    if (model == null || string.IsNullOrWhiteSpace(model.Id) || string.IsNullOrWhiteSpace(model.RawId) ||
        string.IsNullOrWhiteSpace(model.AttestationObject) || string.IsNullOrWhiteSpace(model.ClientDataJSON) ||
        model.UserId <= 0)
    {
        TempData["Error"] = "Invalid request parameters.";
        return RedirectToAction("Dashboard");
    }
    try
    {
        // Get the UserId from the current authorized user
        int userId = int.Parse(HttpContext.User.FindFirst("UserId").Value);
        
        // Load user securely from a database or other storage
        var user = LoadUserById(userId);
        
        // Handle enabling/disabling passkey
        if (user.PasskeyEnabled)
        {
            return RedirectToAction("Dashboard");
        }
        else
        {
            // Enable passkey
            var passkeyDetails = new PasskeyDetails
            {
                userId = userId.UserId
                Id = model.Id,
                RawId = model.RawId,
                AttestationObject = model.AttestationObject,
                ClientDataJSON = model.ClientDataJSON,
                UserAgent = model.UserAgent,
                CreatedAt = DateTime.UtcNow
            };
            
            user.PasskeyEnabled = true;
            user.PasskeyDetails = EncryptPasskeyDetails(passkeyDetails);
        }
        
        // Save updated user details securely
        SaveUser(user);
        
        TempData["Success"] = "Passkey updated successfully.";
        return RedirectToAction("Dashboard");
    }
    catch (Exception ex)
    {
        // Log the exception (do not expose sensitive details to the user)
        LogError(ex);
        TempData["Error"] = "An error occurred while processing your request.";
        return RedirectToAction("Dashboard");
    }
}
``````
<br/>

In a secured file, since the user is already authenticated, the user ID should be retrieved directly from the server-side session or context rather than relying on client-side input

