## Unauthorized API Access
Will see how to secure a unauthorized API endpoints.

### Secured Sample Code
```csharp
[Authorize]
[AdminOnlyFilter]
[HttpPost("create-tenant")]
public IActionResult CreateTenant([FromBody] TenantCreateDto tenantDto)
{
    try
    {
        // Tenant create logic
    }
    catch (Exception ex)
    {
        // Log error
    }
}
```
<br/>

Here you can see the `create-tenant` endpoint is secured with `[Authorize]` and `[AdminOnlyFilter]` attributes.

 - `[Authorize]` attribute ensures that the user is authenticated.
 - `[AdminOnlyFilter]` - This attribute is custom attribute to check if the user has admin permissions based on the claims/policy/JWT tokens.

So now only authenticated users with admin permissions can create a tenant. If any other previlleged user try to create a tenant it will lead to unauthorized access page.
