### <u>Cookie Manipulation</u>
##### **Description**
Will see the secured cookie manipulation application.


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
    
    // Check if OTP verification has already been completed from DB
    if (IsUserOTPVerified(userId))
    {
        return RedirectToAction("SaveUserAfterVerification");
    }
    
    // Redirect to the OTP verification page
    return View();
}
``````
<br/>

**Mitigation:**  
In this flow, after the user enters the OTP, we store that information in the database instead of using a cookie. We then retrieve the data from the database to process the request.

