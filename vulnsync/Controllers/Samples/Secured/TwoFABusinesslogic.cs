using Microsoft.AspNetCore.Mvc;

namespace VulnSync.Controllers.Samples.Secured;

[Route("Secured/[controller]")]
public class TwoFABusinesslogic : Controller
{
    // Static list of users (in real applications, this should be in a database)
    private static readonly List<UserModel> Users = new()
    {
        new UserModel { ID = "1",  Email = "admin@example.com", Password = "admin123", IsTwoFaEnabled = true },
        new UserModel { ID = "2", Email = "user@example.com", Password = "user123", IsTwoFaEnabled = true },
        new UserModel { ID = "3", Email = "user2@example.com", Password = "user2123", IsTwoFaEnabled = false },
        
    };
    
    // Static 2FA code for demo (in real applications, this should be generated dynamically)
    private const string ValidTwoFaCode = "654321";

    // Add these private helper methods
    private string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    private bool Base64Decode(string base64EncodedData, string email)
    {
        try {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            string needToSkip = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            string[] needToSkipArray = needToSkip.Split("_");
            string userEmail = GetEmailFromId(needToSkipArray[0]);
            if (needToSkipArray[1] == "true" && userEmail == email)
            {
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private string GetIdFromEmail(string email)
    {
        return Users.Where(x => x.Email == email).Select(x => x.ID).FirstOrDefault();
    }

    private string GetEmailFromId(string id)
    {
        return Users.Where(x => x.ID == id).Select(x => x.Email).FirstOrDefault();
    }
    
    [HttpGet]
    public IActionResult SecuredLogin()
    {
        return View("Secured/Login");
    }

    [HttpPost("Login")]
    public IActionResult SecuredPostLogin(string email, string password)
    {
        var user = Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        
        if (user == null)
        {
            TempData["Error"] = "Invalid credentials";
            return RedirectToAction("SecuredLogin");
        }

        // Check if user has 2FA enabled and if there's no remember device cookie
        if (user.IsTwoFaEnabled)
        {
            var skipTwoFaCookie = Request.Cookies["SkipTwoFa"];
            if (string.IsNullOrEmpty(skipTwoFaCookie))
            {
                TempData["PendingTwoFaEmail"] = email;
                return RedirectToAction("SecuredTwoFactorAuth");
            }
            
            bool skipTwoFafromClient = Base64Decode(skipTwoFaCookie, user.Email);
            if (skipTwoFafromClient)
            {
                // Store user email in TempData for the 2FA validation
                return RedirectToAction("SecuredProfile");
            }
            TempData["PendingTwoFaEmail"] = email;
            return RedirectToAction("SecuredTwoFactorAuth");
            
        }

        return RedirectToAction("SecuredProfile");
    }

    [HttpGet("TwoFactorAuth")]
    public IActionResult SecuredTwoFactorAuth()
    {
        if (TempData["PendingTwoFaEmail"] == null)
        {
            return RedirectToAction("SecuredLogin");
        }

        TempData["PendingTwoFaEmail"] = TempData["PendingTwoFaEmail"];
        return View("Secured/TwoFactorAuth");
    }

    [HttpPost("SecuredValidateTwoFactor")]
    public IActionResult SecuredValidateTwoFactor(string code, string rememberDevice)
    {
        var userEmail = TempData["PendingTwoFaEmail"] as string;
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("SecuredLogin");
        }

        if (code != ValidTwoFaCode)
        {
            TempData["Error"] = "Invalid 2FA code";
            TempData.Keep("PendingTwoFaEmail");
            return RedirectToAction("SecuredTwoFactorAuth");
        }

        if (rememberDevice?.ToLower() == "on")
        {
            // Set cookie to skip 2FA for this device
            Response.Cookies.Append("SkipTwoFa", Base64Encode(GetIdFromEmail(userEmail)+"_true"), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(30),
                HttpOnly = true
            });
        }

        return RedirectToAction("SecuredProfile");
    }

    [HttpGet("Profile")]
    public IActionResult SecuredProfile()
    {
        // Keep the email in TempData to display it in the profile
        TempData.Keep("PendingTwoFaEmail");
        return View("Secured/Profile");
    }
}


public class UserModel
{
    public string ID { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsTwoFaEnabled { get; set; }
}