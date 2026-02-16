namespace VulnSync.Models.RoleManipulation;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } // "admin" or "user"
    public string PersonalDetails { get; set; } // Editable by the user
    public string Address { get; set; }
 
}