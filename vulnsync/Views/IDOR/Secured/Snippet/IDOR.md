## Secure Implementation (IDOR Prevention)
### Security Enhancement Explanation:
- The secure code retrieves the account details for the logged-in user using UserManager to get the authenticated user's ID (currentUser.Id), ensuring they can only access their own account.
- The userId is not passed through the URL, so users cannot manipulate it to access other accounts.

### Sample code:
### Backend (Controller):
```csharp
public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuthorizationService _authorizationService;
    
    public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, AuthorizationService authorizationService)
    {
        _context = context;
        _userManager = userManager;
        _authorizationService = authorizationService;
    }
    
    public async Task<IActionResult> Details(string userId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }
        
        bool isAuthorized = await _authorizationService.IsAuthorizedAsync(currentUser, userId);
        if (!isAuthorized)
        {
            return Forbid("You are not authorized to access this account.");
        }
        
        var account = _context.Accounts.SingleOrDefault(a => a.UserId == userId);
        
        if (account == null)
        {
            return NotFound();
        }
        
        return View(account);
    }
}
```
### Authorization Service:
```csharp
public class AuthorizationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public AuthorizationService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<bool> IsAuthorizedAsync(ApplicationUser currentUser, string userId)
    {
        var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
        
        return isAdmin || currentUser.Id == userId;
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
