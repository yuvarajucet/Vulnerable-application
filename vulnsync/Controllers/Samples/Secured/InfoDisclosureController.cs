using System.Text;
using Microsoft.AspNetCore.Mvc;
using VulnSync.Models.Vulnerable.InfoDisclosure;

namespace VulnSync.Controllers.Samples.Secured;

[Route("secured/[controller]/")]
public class InfoDisclosureController : Controller
{
    private List<LoginViewModel> _loginViewModel = new List<LoginViewModel>();

    public InfoDisclosureController()
    {
        _loginViewModel = this.UpdateUsersOnList();
    }
    
    [HttpGet]
    public IActionResult SecuredLogin()
    {
        return View("Secured/Login");
    }
    
    [HttpPost("secureduserlogin")]
    public async Task<IActionResult> SecuredLoginUser()
    {
        var loginInfo = Request.ReadFormAsync().Result;
        string username = loginInfo["username"];
        string password = loginInfo["password"];
        
        if(this.UserLogin(username, password))
        {
            string userEmail = GetEmailBasedOnUsername(username);
            string EncryptedEmail = Convert.ToBase64String(Encoding.UTF8.GetBytes(userEmail));
            Response.Cookies.Append("AuthCredentials", EncryptedEmail);
            
            return Ok(new ResponseModel()
            {
                IsAdmin = false,
                Username = username,
                Success = true,
                Email = EncryptedEmail
            });
        }

        return Ok(new ResponseModel()
        {
            IsAdmin = false,
            Username = username,
            Success = false,
            Message = "Invalid username or password"
        });
    }
    
    [HttpGet("user")]
    public IActionResult SecuredUserProfile()
    {
        // get the email from the cookie
        string encryptedEmail = Request.Cookies["AuthCredentials"];
        string email = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedEmail));
        string username = GetUsernameBasedOnEmail(email);
        ViewBag.Username = username;
        return View("Secured/User");
    }

    [HttpGet("accountinfo")]
    public IActionResult SecuredAccountInfo()
    {
        return View("Secured/AccountInfo");
    }

    [HttpPost("submitform")]
    public IActionResult SecuredAccountInfoResult(CardDetails model)
    {
        return View("Secured/AccountInfoResult",model);
    }

    [HttpPost]
    public IActionResult SecuredGetAccountInformation()
    {
        var loginInfo = Request.ReadFormAsync().Result;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "DlgcmqKTGUiwPspQirEwcbMlps");
        var response = client.GetAsync($"http://{Request.Host.ToString()}/secured/infodisclosure/api/GetAccountInfo?username={loginInfo["username"]}").Result;
        if (response.IsSuccessStatusCode)
        {
            var accountInfo = response.Content.ReadAsStringAsync().Result;
            return Ok(accountInfo);
        }
        else
        {
            return Unauthorized();
        }
    }
    
    /*API application process*/
    [HttpGet("api/GetAccountInfo")]
    public IActionResult GetAccountInfo(string username)
    {
        string token = Request.Headers["Authorization"];
        if (token == "DlgcmqKTGUiwPspQirEwcbMlps")
        {
            var user = _loginViewModel.FirstOrDefault(x => x.Username == username);
            return Ok(user.AccountInfo);
        }
        else
        {
            return Unauthorized();
        }
    }
    
    
    
    /*-------- Helper Methods ------*/
    
    private List<LoginViewModel> UpdateUsersOnList()
    {
        List<LoginViewModel> users = new List<LoginViewModel>();
        users.Add(new LoginViewModel(){
            Username = "john",
            Password = "John@123",
            RememberMe = false,
            Email = "john@gmail.com",
            AccountInfo = new AccountInfo()
            {
                AccountNumber = "123456789",
                Amount = 1000,
                PinNumber = 1234,
                AccountType = "Savings"
            }
        });
        
        users.Add( new LoginViewModel() {
            Username = "jane",
            Password = "Jane@123",
            RememberMe = false,
            Email = "jane@gmail.com",
            AccountInfo = new AccountInfo()
            {
                AccountNumber = "987654321",
                Amount = 2000,
                PinNumber = 4321,
                AccountType = "Current"
            }
           
        });

        users.Add(new LoginViewModel()
        {
            Username = "admin",
            Password = "P@ssw0rd",
            RememberMe = false,
            Email = "admin@gmail.com",
            AccountInfo = new AccountInfo()
            {
                AccountNumber = "123456789",
                Amount = 10000,
                PinNumber = 8879,
                AccountType = "Savings"
            }
            
        });
        
        return users;
    }
    
    private bool UserLogin(string username, string password)
    {
        foreach (var user in _loginViewModel)
        {
            if(user.Username == username && user.Password == password)
            {
                return true;
            }
        }
        return false;
    }
    
    private string GetEmailBasedOnUsername(string username)
    {
        foreach (var user in _loginViewModel)
        {
            if(user.Username == username)
            {
                return user.Email;
            }
        }
        return string.Empty;
    }

    private string GetUsernameBasedOnEmail(string email)
    {
        foreach (var user in _loginViewModel)
        {
            if(user.Email == email)
            {
                return user.Username;
            }
        }
        return string.Empty;
    }
}