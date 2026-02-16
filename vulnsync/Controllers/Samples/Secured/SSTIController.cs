using Microsoft.AspNetCore.Mvc;
using RazorLight;

namespace VulnSync.Controllers.Samples.Secured;

[Route("secured/[controller]")]
public class SSTIController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View("Secured/Index");
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> SecuredSearch(string search)
    {
        try
        {
            string razorTpl = $"Hello @Model.search!";
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
}

public class SSTIProduct
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
}

public class SSTIProductRepository
{
    private static readonly List<SSTIProduct> Products = new List<SSTIProduct>
    {
        new SSTIProduct { Id = 1, Name = "Laptop" },
        new SSTIProduct { Id = 2, Name = "Smartphone" },
        new SSTIProduct { Id = 3, Name = "Headphones" },
        new SSTIProduct { Id = 4, Name = "Tablet" },
    };

    public IEnumerable<SSTIProduct> GetProducts(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Products;

        return Products.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }
}
