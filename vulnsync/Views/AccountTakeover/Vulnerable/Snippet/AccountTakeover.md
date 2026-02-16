### <u>Account Takeover using userId</u>

##### **Description**
Will see the vulernable account takeover application.


This application has a passkey feature, allowing users to register a passkey for their account.

```csharp
[HttpPost] // Ensure this action only accepts POST requests
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
        // Load user securely from a database or other storage
        var user = LoadUserById(model.UserId);
        
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
                userId = model.UserId
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

**Attack Technique:** During the registration process for a passkey, a tool is used to intercept the request. In this process, the user ID is being sent from the client side, making it easy for an attacker to modify the user ID to gain unauthorized access to another account.

