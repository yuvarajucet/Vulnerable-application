## Unauthorized API Access
Will see the unauthorized API access due to improper authentication check.

### Vulnerable Sample Code
```csharp
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

Here you can see the `create-tenant` endpoint create a tenant without any authentication check.
So anyone can create a tenant without any authentication by directly accessing this endpoint.
