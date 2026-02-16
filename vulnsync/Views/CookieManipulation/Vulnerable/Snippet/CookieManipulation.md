### <u>Cookie Manipulation</u>
##### **Description**
Will see the vulernable cookie manipulation application.


In this application, the dashboard page has an option to edit the email after OTP verification. After entering the OTP, a new cookie named "OTPVerification" is created to confirm that the user has entered a valid OTP to process this request. Upon providing the correct OTP, the email should be updated.

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]
public IActionResult SaveUser(User updatedUser)
{
    // Check if the user is authenticated and authorized
    var currentUserId = User.Identity?.Name; // Retrieve the authenticated user's identifier (e.g., username or ID)
    if (string.IsNullOrEmpty(currentUserId))
    {
        return Unauthorized("User is not authenticated.");
    }
    
    // Check if the current customer is valid
    if (!TempData.ContainsKey("CurrentCustomer"))
    {
        return BadRequest("Invalid user session or customer information.");
    }
    
    int userId = Convert.ToInt32(TempData["CurrentCustomer"]);
    
    // Validate that the user is allowed to receive an OTP
    if (!IsValidUserForOTP(userId)) // Custom method to validate the user
    {
        return Forbid("User is not authorized to receive an OTP.");
    }
    
    // Generate OTP for the user
    var otp = GenerateOTP();
    
    // Save OTP securely (e.g., database, encrypted storage)
    SaveOTPToDatabase(userId, otp);
    
    // Check if OTP verification has already been completed
    if (Request.Cookies.ContainsKey("OTPVerification") && Request.Cookies["OTPVerification"] == "Completed")
    {
        return RedirectToAction("SaveUserAfterOTP");
    }
    
    // Redirect to the OTP verification page
    return View("Vulnerable/VerifyOTP");
}
``````
<br/>

**Attack Technique:**  
In this flow, after the user enters the OTP, a new cookie named "OTPVerification" is generated to confirm the validity of the OTP. An attacker can analyze this process, identify the cookie creation mechanism, and manually craft or modify the "OTPVerification" cookie to bypass the OTP verification step. This allows them to exploit the system and proceed with unauthorized actions without providing the correct OTP.

