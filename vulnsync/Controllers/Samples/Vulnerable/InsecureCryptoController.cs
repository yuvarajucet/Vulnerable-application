using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace VulnSync.Controllers.Samples.Vulnerable;

[Route("vulnerable/[controller]")]
public class InsecureCryptoController : Controller
{
    // GET
    public IActionResult VulnerableIndex()
    {
        return View("Vulnerable/Index");
    }

    [HttpPost]
    [Route("vulnerbalelogin")]
    public IActionResult VulnerableLogin()
    {
        var loginInfo = Request.ReadFormAsync().Result;
        string username = loginInfo["username"];
        string password = loginInfo["password"];
        if (username == "john" && password == "John@123")
        {
            TempData.Add("username", "john");
            return RedirectToAction("VulnerableProfile");
        }
        else
        {
            TempData.Add("message", "Invalid username or password");
            return RedirectToAction("VulnerableIndex");
        }
    }

    [HttpGet]
    [Route("vulnerableprofile")]
    public IActionResult VulnerableProfile()
    {
        return View("Vulnerable/Profile");
    }

    [HttpGet]
    [Route("vulnerableforgetpassword")]
    public IActionResult VulnerableForgetPassword()
    {
        return View("Vulnerable/ForgetPassword");
    }

    [HttpPost]
    [Route("vulnerableforgetpassword")]
    public async Task<JsonResult> VulnerableForget()
    {
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            var body = await reader.ReadToEndAsync();
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
        
            if (jsonData == null || !jsonData.ContainsKey("email"))
            {
                return Json(new { success = false, message = "Email is required." });
            }

            string email = jsonData["email"];
            string emailHash = GenerateMDHash(email); // Assuming this is your hash function
            string forgetLink = $"http://localhost:5187/forgetpassword?code={emailHash}";

            return Json(new { success = true, resetLink = forgetLink });
        }
    }
    
    
    [HttpGet]
    [Route("generate")]
    public JsonResult Generate()
    {
        var hashvalue = GenerateMDHash("john");
        return Json(new { hash = hashvalue });
    }


    private string GenerateMDHash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}