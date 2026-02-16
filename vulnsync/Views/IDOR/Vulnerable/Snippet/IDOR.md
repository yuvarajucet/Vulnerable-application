## Vulnerable Example for IDOR

### Vulnerability Explanation:
- The vulnerable code allows any authenticated user to access the account details by changing the userId in the URL, for example: https://example.com/account/details?userId=1234.
- A malicious user can alter the userId parameter in the URL to another value (e.g., userId=1235), gaining access to other users' account details.

### Sample Code:
### Backend (Controller):
```csharp
public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // Vulnerable method that fetches user account by UserId directly from URL
    public IActionResult Details(int userId)
    {
        // Fetching account details without checking if the logged-in user owns this account
        var account = _context.Accounts.SingleOrDefault(a => a.UserId == userId);
        
        if (account == null)
        {
            return NotFound();
        }
        
        return View(account); // Passing account details to view
    }
}
```
### Client-side (View):
```html
@model Account

<h1>Account Details</h1>
<p>User ID: @Model.UserId</p>
<p>Account Name: @Model.Name</p>
<p>Balance: @Model.Balance</p>
```