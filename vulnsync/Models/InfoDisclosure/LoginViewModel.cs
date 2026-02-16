namespace VulnSync.Models.Vulnerable.InfoDisclosure;

public class LoginViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
    public string Email { get; set; }
    
    public AccountInfo AccountInfo { get; set; }
}

public class AccountInfo
{
    public string AccountNumber { get; set; }
    public int Amount { get; set; }
    public int PinNumber { get; set; }
    public string AccountType { get; set; }
}