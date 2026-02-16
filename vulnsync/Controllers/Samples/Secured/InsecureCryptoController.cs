using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace VulnSync.Controllers.Samples.Secured;

[Route("secured/[controller]")]
public class InsecureCryptoController : Controller
{
    // GET
    public IActionResult SecuredIndex()
    {
        return View("Secured/Index");
    }
    
    [HttpPost]
    [Route("securedlogin")]
    public IActionResult SecuredLogin()
    {
        var loginInfo = Request.ReadFormAsync().Result;
        string username = loginInfo["username"];
        string password = loginInfo["password"];
        if (username == "john" && password == "John@123")
        {
            TempData.Add("username", "john");
            return RedirectToAction("SecuredProfile");
        }
        else
        {
            TempData.Add("message", "Invalid username or password");
            return RedirectToAction("SecuredIndex");
        }
    }
    
    [HttpGet]
    [Route("securedprofile")]
    public IActionResult SecuredProfile()
    {
        return View("Secured/Profile");
    }
    
    [HttpGet]
    [Route("securedforgetpassword")]
    public IActionResult SecuredForgetPassword()
    {
        return View("Secured/ForgetPassword");
    }
    
    
    [HttpPost]
    [Route("securedforgetpassword")]
    public async Task<JsonResult> SecuredForget()
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
            string emailHash = EncrypteWithAES(email);
            string forgetLink = $"http://localhost:5187/forgetpassword?code={emailHash}";

            return Json(new { success = true, resetLink = forgetLink });
        }
    }
    
    [HttpGet]
    [Route("securedgenerate")]
    public JsonResult SecuredGenerate()
    {
        var hashvalue = EncrypteWithAES("john");
        return Json(new { hash = hashvalue });
    }
    
    
    private string EncrypteWithAES(string input)
    {
        string encryptionKey = "MyStaticEncryptionKey123!";
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32, ' '));
        byte[] ivBytes = Encoding.UTF8.GetBytes("1234567890123456");

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.IV = ivBytes;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            using (MemoryStream msEncrypt = new MemoryStream())
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                csEncrypt.Write(inputBytes, 0, inputBytes.Length);
                csEncrypt.FlushFinalBlock();
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
}