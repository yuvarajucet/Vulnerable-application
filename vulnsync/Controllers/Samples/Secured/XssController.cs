using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VulnSync.Models.XSS;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities; // For encoding
using System.Net;

namespace VulnSync.Controllers.Samples.Secured
{
    [Route("secured/[controller]")]
    public class XssController : Controller
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "SecuredProducts.json");

        // Load products from JSON file
        private List<OnlineProduct> SecuredLoadProducts()
        {
            var userDetailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "XSS", "SecuredProducts.json");

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
        private void SecuredSaveProducts(List<OnlineProduct> products)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(products);
                var userDetailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "XSS", "SecuredProducts.json");
                System.IO.File.WriteAllText(userDetailsPath, jsonData);
            }
            catch (IOException ex)
            {
                System.Console.WriteLine($"Error saving data to file: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult SecuredIndex()
        {
            var products = SecuredLoadProducts();
            return View("Secured/Index", products);
        }

        [HttpPost("SecuredReflect")]
        public IActionResult SecuredReflect(string search)
        {
            var products = SecuredLoadProducts();

            if (!string.IsNullOrEmpty(search))
            {
                // Secure the search input by encoding it
                ViewBag.SearchQuery = WebUtility.HtmlEncode(search); // Prevent XSS by encoding input

                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                if (products.Count == 0)
                {
                    var product = SecuredLoadProducts();
                    return View("Secured/Index", product);
                }

                return View("Secured/Index", products);
            }

            return RedirectToAction("SecuredIndex", "Xss");
        }

        [HttpPost("SecuredDetails")]
        public ActionResult SecuredDetails(int id)
        {
            var products = SecuredLoadProducts();
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View("Secured/Product", product);
        }

        [HttpPost("SecuredAddComment")]
        public IActionResult SecuredAddComment(int id, string comment)
        {
            var products = SecuredLoadProducts();
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Secure the comment input by encoding it
            if (!string.IsNullOrEmpty(comment))
            {
                if (product.Comments == null)
                {
                    product.Comments = new List<string>();
                }

                // Encode the comment to prevent stored XSS
                var encodedComment = WebUtility.HtmlEncode(comment); // Prevent XSS by encoding input
                product.Comments.Add(encodedComment);

                SecuredSaveProducts(products); // Save updated products list to the JSON file
            }

            return RedirectToAction("SecuredUserComment", new { id = id });
        }

        [HttpGet("SecuredUserComment")]
        public ActionResult SecuredUserComment(int id)
        {
            var products = SecuredLoadProducts();
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View("Secured/Product", product);
        }
    }
}
