## Vulnerable SSTI
Vulnerable Code Sample for Server Side template Injection.

## Source code
```csharp
[HttpGet("search")]
public async Task<IActionResult> VulnerableSearch(string search)
{
    try
    {
        string razorTpl = $"Hello {search}";
        string renderedTemplate = null;
        var engine = new RazorLightEngineBuilder()
            .UseMemoryCachingProvider()
            .Build();
        renderedTemplate = await engine.CompileRenderStringAsync<object>("templateKey", razorTpl, null);
        ViewBag.RenderedTemplate = renderedTemplate;
        ViewBag.SearchedItem = new SSTIProductRepository().GetProducts(search).FirstOrDefault()?.Name;
        ViewBag.Template = razorTpl;
    }
    catch (Exception ex)
    {
        ViewBag.RenderedTemplate = $"Error: Something went wrong!";
        ViewBag.Template = "Error rendering template.";
    }
    return View("Vulnerable/Index");
}
```