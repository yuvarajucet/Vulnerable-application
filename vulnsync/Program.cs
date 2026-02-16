var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddPolicy("InsecurePolicy", policy =>
    {
        policy.AllowAnyOrigin() //  Allows any website (Security Risk)
              .AllowAnyMethod() //  No method restrictions
              .AllowAnyHeader(); 
    });
});
builder.Services.AddControllers();
builder.Services.AddSession();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseCors("InsecurePolicy");
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // Default route
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Route for Product action in ResponseManipulation
    endpoints.MapControllerRoute(
        name: "ResponseManipulationProduct",
        pattern: "ResponseManipulation/{action=Product}",
        defaults: new { controller = "ResponseManipulation", action = "Product" });

});

app.Run();