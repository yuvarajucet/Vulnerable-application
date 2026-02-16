using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VulnSync.Models.XSS;
using System.Linq;

namespace VulnSync.Controllers.Samples.Vulnerable
{
    [Route("vulnerable/[controller]")]
    public class XssController : Controller
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "OnlineProducts.json");

        // Load products from JSON file
        private List<OnlineProduct> LoadProducts()
        {
            var userDetailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "XSS", "OnlineProducts.json");

            if (!System.IO.File.Exists(userDetailsPath))
            {
                return new List<OnlineProduct>();
            }

            try
            {
                var jsonData = System.IO.File.ReadAllText(userDetailsPath);
                if (string.IsNullOrEmpty(jsonData))
                {
                    return new List<OnlineProduct>();
                }

                var products = JsonSerializer.Deserialize<List<OnlineProduct>>(jsonData);
                return products ?? new List<OnlineProduct>();
            }
            catch (JsonException)
            {
                return new List<OnlineProduct>();
            }
            catch (IOException)
            {
                return new List<OnlineProduct>();
            }
        }

        // Save products to JSON file
        private void SaveProducts(List<OnlineProduct> products)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(products);
                var userDetailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "XSS", "OnlineProducts.json");
                System.IO.File.WriteAllText(userDetailsPath, jsonData);
            }
            catch (IOException ex)
            {
                System.Console.WriteLine($"Error saving data to file: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            var products = LoadProducts();
            return View("Vulnerable/Index", products);
        }

        [HttpPost("Reflect")]
        public IActionResult Reflect(string search)
        {
            var products = LoadProducts();

            if (!string.IsNullOrEmpty(search))
            {
                // Reflected XSS Vulnerability
                ViewBag.SearchQuery = search;
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                if (products.Count == 0)
                {
                    var product = LoadProducts();
                    return View("Vulnerable/Index", product);
                }
                
                return View("Vulnerable/Index", products);
            }

            return RedirectToAction("Index", "Xss");
        }
        
        [HttpPost("Details")]
        public ActionResult Details(int id)
        {
            var products = LoadProducts();
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View("Vulnerable/Product", product);
        }

        [HttpPost("AddComment")]
        public IActionResult AddComment(int id, string comment)
        {
            var products = LoadProducts();
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Stored XSS Vulnerability
            if (!string.IsNullOrEmpty(comment))
            {
                if (product.Comments == null)
                {
                    product.Comments = new List<string>();
                }
                product.Comments.Add(comment);
                SaveProducts(products); // Save updated products list to the JSON file
            }

            return RedirectToAction("UserComment", new { id = id });
        }
        
        [HttpGet("UserComment")]
        public ActionResult UserComment(int id)
        {
            var products = LoadProducts();
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View("Vulnerable/Product", product);
        }

    }
}
