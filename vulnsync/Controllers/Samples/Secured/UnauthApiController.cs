using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VulnSync.Models;
using System.Text.Json.Serialization;

namespace VulnSync.Controllers.Samples.Secured;

[Route("secured/[controller]")]
public class UnauthApiController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _jsonPath;

    public UnauthApiController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
        _jsonPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Database", "UnAuthApi", "tenants.json");
    }

    [HttpGet]
    public IActionResult SecuredIndex()
    {
        ViewBag.AdminLink = "unauthapi/previllege";
        ViewBag.UserLink = "unauthapi/user";
        return View("Index");
    }

    
    [HttpGet("previllege")]
    public IActionResult SecuredUser()
    {
        try
        {
            string adminPermission = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("admin"));
            
            Response.Cookies.Append("userPermission", adminPermission, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.Now.AddHours(1)
            });

            return RedirectToAction("SecuredAdmin");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in user endpoint: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("admin")]
    public IActionResult SecuredAdmin()
    {
        try
        {
            // Check for cookie and validate admin permission
            if (!Request.Cookies.TryGetValue("userPermission", out string? cookieValue))
            {
                
                ViewBag.HomeUrl = "/secured/unauthapi";
                ViewBag.RetryUrl = "/secured/unauthapi/logout";
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("Unauthorized", 
                    "No permission cookie found. Please login first.");
            }

            string decodedValue = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue));
            if (decodedValue != "admin")
            {
                return UnauthorizedView("You need admin permissions to access this page.");
            }

            var jsonPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Database", "UnAuthApi", "tenants.json");
            
            if (!System.IO.File.Exists(jsonPath))
            {
                Console.WriteLine($"File not found at path: {jsonPath}");
                return View("Secured/Admin", new List<TenantInfo>());
            }

            var jsonContent = System.IO.File.ReadAllText(jsonPath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            var tenantsData = JsonSerializer.Deserialize<TenantDataRoot>(jsonContent, options);

            if (tenantsData?.Tenants == null)
            {
                return View("Secured/Admin", new List<TenantInfo>());
            }

            // Add super admins list to ViewBag
            ViewBag.ExistingSuperAdmins = GetExistingSuperAdmins();

            return View("Secured/Admin", tenantsData.Tenants);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading tenant data: {ex.Message}");
            return UnauthorizedView("An error occurred while validating your permissions.");
        }
    }

    [HttpPost("create-tenant")]
    public IActionResult CreateTenant([FromBody] TenantCreateDto tenantDto)
    {
        try
        {
             if (!Request.Cookies.TryGetValue("userPermission", out string? cookieValue))
            {
                
                ViewBag.HomeUrl = "/secured/unauthapi";
                ViewBag.RetryUrl = "/secured/unauthapi/logout";
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return View("Unauthorized", 
                    "No permission cookie found. Please login first.");
            }

            if (string.IsNullOrEmpty(tenantDto.TenantName) || string.IsNullOrEmpty(tenantDto.SuperAdminName))
            {
                return BadRequest("Tenant name and Super Admin name are required.");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            var jsonContent = System.IO.File.ReadAllText(_jsonPath);
            var tenantsData = JsonSerializer.Deserialize<TenantDataRoot>(jsonContent, options) 
                             ?? new TenantDataRoot { Tenants = new List<TenantInfo>() };

            var existingTenant = tenantsData.Tenants.FirstOrDefault(t => 
                t.TenantName.Equals(tenantDto.TenantName, StringComparison.OrdinalIgnoreCase));

            if (existingTenant != null)
            {
                existingTenant.SuperAdminName = tenantDto.SuperAdminName;
                existingTenant.DatabaseName = tenantDto.DatabaseName;
                existingTenant.ServerUrl = tenantDto.ServerUrl;
                existingTenant.ConnectionString = GenerateConnectionString(tenantDto.ServerUrl, tenantDto.DatabaseName);
            }
            else
            {
                var newTenant = new TenantInfo
                {
                    TenantName = tenantDto.TenantName,
                    SuperAdminName = tenantDto.SuperAdminName,
                    TenantId = $"tenant-{Guid.NewGuid():N}",
                    CreatedDate = DateTime.UtcNow,
                    DatabaseName = tenantDto.DatabaseName,
                    ServerUrl = tenantDto.ServerUrl,
                    ConnectionString = GenerateConnectionString(tenantDto.ServerUrl, tenantDto.DatabaseName)
                };

                tenantsData.Tenants.Add(newTenant);
            }

            var updatedJson = JsonSerializer.Serialize(tenantsData, options);
            System.IO.File.WriteAllText(_jsonPath, updatedJson);

            return Ok(new { message = existingTenant != null ? "Tenant updated successfully" : "Tenant created successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating/updating tenant: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("user")]
    public IActionResult SecuredUserDashboard()
    {
        try
        {
            // Read the first tenant from JSON file
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonContent = System.IO.File.ReadAllText(_jsonPath);
            var tenantsData = JsonSerializer.Deserialize<TenantDataRoot>(jsonContent, options);
            
            // Get the first tenant for demo
            var firstTenant = tenantsData?.Tenants.FirstOrDefault();
            
            // Create view model for user dashboard
            var dashboardItems = new List<DashboardItem>
            {
                new DashboardItem { Name = "Security Alerts", Count = 5 },
                new DashboardItem { Name = "Active Sessions", Count = 3 },
                new DashboardItem { Name = "Pending Tasks", Count = 8 },
                new DashboardItem { Name = "Recent Activities", Count = 12 }
            };

            ViewBag.TenantName = firstTenant?.TenantName ?? "Default Tenant";
            return View("Secured/User", dashboardItems);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in user dashboard: {ex.Message}");
            return UnauthorizedView("An error occurred while validating your permissions.");
        }
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("userPermission");
        
        return RedirectToAction("SecuredIndex");
    }

    private string GenerateConnectionString(string serverUrl, string databaseName)
    {
        return $"Server={serverUrl};Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true";
    }

    private List<string> GetExistingSuperAdmins()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonContent = System.IO.File.ReadAllText(_jsonPath);
            var tenantsData = JsonSerializer.Deserialize<TenantDataRoot>(jsonContent, options);
            
            // Get unique super admin names
            return tenantsData?.Tenants
                .Select(t => t.SuperAdminName)
                .Distinct()
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList() ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private IActionResult UnauthorizedView(string message = null)
    {
        ViewBag.ErrorMessage = message;
        ViewBag.HomeUrl = "/secured/unauthapi";
        ViewBag.RetryUrl = "/secured/unauthapi/logout";
        return View("Unauthorized");
    }
}

public class TenantDataRoot
{
    [JsonPropertyName("tenants")]
    public List<TenantInfo> Tenants { get; set; } = new List<TenantInfo>();
}

public class DashboardItem
{
    public string Name { get; set; }
    public int Count { get; set; }
}