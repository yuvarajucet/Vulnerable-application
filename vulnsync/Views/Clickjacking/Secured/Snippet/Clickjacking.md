## Secured Example
This version adds protection against clickjacking by using **X-Frame-Options** and **Content Security Policy (CSP)** headers.

### Code for ASP.NET Core Middleware:
```csharp
public void Configure(IApplicationBuilder app)
{
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors 'none'");
        await next();
    });
}
```

### Explanation:
- **X-Frame-Options:**
  - The **X-Frame-Options** header prevents the page from being rendered in a frame or iframe.
  - The value **DENY** prevents the page from being rendered in a frame on any site.
- **Content Security Policy (CSP):**
  - The **Content-Security-Policy** header restricts the sources from which the browser can load content.
  - The directive **frame-ancestors 'none'** prevents the page from being embedded in any frame.