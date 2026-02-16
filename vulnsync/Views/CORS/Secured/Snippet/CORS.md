## Secure sample for CORS policy
To prevent this attack, update the CORS policy as follows:

**This configuration fixes the security flaws by:**
- Restricting origins to specific trusted domains.
- Limiting allowed HTTP methods.
- Enabling credentials only for trusted sources.
- Using proper authentication & authorization mechanisms.
#### Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("SecurePolicy", policy =>
    {
        policy.WithOrigins("https://trustedsite.com") // ✅ Restrict to trusted sites
              .WithMethods("GET", "POST") // ✅ Limit HTTP methods
              .WithHeaders("Authorization", "Content-Type") // ✅ Allow specific headers
              .AllowCredentials(); // ✅ Only for trusted origins
    });
});

var app = builder.Build();
app.UseCors("SecurePolicy"); // ✅ Apply the secure policy
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

