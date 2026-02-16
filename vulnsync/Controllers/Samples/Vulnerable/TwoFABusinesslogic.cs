using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VulnSync.Models;
using System.Text.Json.Serialization;

namespace VulnSync.Controllers.Samples.Vulnerable;

[Route("Vulnerable/[controller]")]
public class TwoFABusinesslogicController : Controller
{
    // Static list of users (in real applications, this should be in a database)
    private static readonly List<UserModel> Users = new()
    {
        new UserModel { Email = "admin@example.com", Password = "admin123", IsTwoFaEnabled = true },
        new UserModel { Email = "user@example.com", Password = "user123", IsTwoFaEnabled = true },
        new UserModel { Email = "user2@example.com", Password = "user2123", IsTwoFaEnabled = false },
        
    };

    // Static 2FA code for demo (in real applications, this should be generated dynamically)
    private const string ValidTwoFaCode = "654321";

    // Add these private helper methods
    private string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    private bool Base64Decode(string base64EncodedData)
    {
        try {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            string needToSkip = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            if (needToSkip == "true")
            {
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return true;
        }
    }

    [HttpGet]
    public IActionResult VulnerableLogin()
    {
        return View("Vulnerable/Login");
    }

    [HttpPost("Login")]
    public IActionResult VulnerablePostLogin(string email, string password)
    {
        var user = Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        
        if (user == null)
        {
            TempData["Error"] = "Invalid credentials";
            return RedirectToAction("VulnerableLogin");
        }

        // Check if user has 2FA enabled and if there's no remember device cookie
        if (user.IsTwoFaEnabled)
        {
            var skipTwoFaCookie = Request.Cookies["SkipTwoFa"];
            bool skipTwoFafromClient = Base64Decode(skipTwoFaCookie);
            
            if (string.IsNullOrEmpty(skipTwoFaCookie))
            {
                TempData["PendingTwoFaEmail"] = email;
                return RedirectToAction("VulnerableTwoFactorAuth");
            }
            if (skipTwoFafromClient)
            {
                // Store user email in TempData for the 2FA validation
                return RedirectToAction("VulnerableProfile");
            }
            TempData["PendingTwoFaEmail"] = email;
            return RedirectToAction("VulnerableTwoFactorAuth");
            
        }

        return RedirectToAction("VulnerableProfile");
    }

    [HttpGet("TwoFactorAuth")]
    public IActionResult VulnerableTwoFactorAuth()
    {
        if (TempData["PendingTwoFaEmail"] == null)
        {
            return RedirectToAction("VulnerableLogin");
        }

        TempData["PendingTwoFaEmail"] = TempData["PendingTwoFaEmail"];
        return View("Vulnerable/TwoFactorAuth");
    }

    [HttpPost("VulnerableValidateTwoFactor")]
    public IActionResult VulnerableValidateTwoFactor(string code, string rememberDevice)
    {
        var userEmail = TempData["PendingTwoFaEmail"] as string;
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("VulnerableLogin");
        }

        if (code != ValidTwoFaCode)
        {
            TempData["Error"] = "Invalid 2FA code";
            TempData.Keep("PendingTwoFaEmail");
            return RedirectToAction("VulnerableTwoFactorAuth");
        }

        if (rememberDevice?.ToLower() == "on")
        {
            // Set cookie to skip 2FA for this device
            Response.Cookies.Append("SkipTwoFa", Base64Encode("true"), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(30),
                HttpOnly = true
            });
        }

        return RedirectToAction("VulnerableProfile");
    }

    [HttpGet("Profile")]
    public IActionResult VulnerableProfile()
    {
        // Keep the email in TempData to display it in the profile
        TempData.Keep("PendingTwoFaEmail");
        return View("Vulnerable/Profile");
    }
}

public class UserModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsTwoFaEnabled { get; set; }
}