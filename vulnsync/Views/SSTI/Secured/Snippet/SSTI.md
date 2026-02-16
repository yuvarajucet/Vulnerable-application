## Secured SSTI
Will see here the secured way to code against Server Side Template Injection

## Recommended
> Avoid using template engines like `engine.CompileRenderStringAsync` for rendering dynamic view pages.  
> Instead, use separate views for each page and pass the required values through the model

## Source code
```csharp
[HttpGet("search")]
public async Task<IActionResult> SecuredSearch(string search)
{
    try
    {
        ViewBag.SearchText = SanitizeUseInput(search);
        SearchItem item = new SSTIProductRepository().GetProducts(search).FirstOrDefault()?.Name;
        return View(item);
    }
    catch (Exception ex)
    {
        ViewBag.Error = $"Error: Something went wrong1";
        ViewBag.item = null;
    }
    return View();
}
```

<br/>
<br/>

## Using `engine.CompileRenderStringAsync` Template engine
<p>If it’s not possible to avoid using a template engine, then use it only as a last resort to render dynamic content.</p>

## Source code
```csharp
[HttpGet("search")]
public async Task<IActionResult> SecuredSearch(string search)
{
    try
    {
        string razorTpl = $"Hello @Model.search";
        string renderedTemplate = null;
        var engine = new RazorLightEngineBuilder()
            .UseMemoryCachingProvider()
            .Build();
        renderedTemplate = await engine.CompileRenderStringAsync<object>("templateKey", razorTpl, new {search = search});
        ViewBag.RenderedTemplate = renderedTemplate;
        ViewBag.SearchedItem = new SSTIProductRepository().GetProducts(search).FirstOrDefault()?.Name;
        ViewBag.Template = razorTpl;
    }
    catch (Exception ex)
    {
        ViewBag.RenderedTemplate = $"Error: {ex.Message}";
        ViewBag.Template = "Error rendering template.";
    }
    return View("Secured/Index");
}
```