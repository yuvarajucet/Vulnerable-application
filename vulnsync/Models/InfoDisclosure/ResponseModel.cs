namespace VulnSync.Models.Vulnerable.InfoDisclosure;

public class ResponseModel
{
    public bool Success { get; set; }
    public bool IsAdmin { get; set; }
    public string Username { get; set; }
    public string Message { get; set; }
    public string Email { get; set; }
    
}