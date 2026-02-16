using Microsoft.AspNetCore.Mvc;
using VulnSync.Models.Vulnerable.ResponseManipulation;
using System.Text.Json;

namespace VulnSync.Controllers.Samples.Vulnerable;
    
    [Route("vulnerable/[controller]")]
    public class ResponseManipulationController : Controller
    {
        // Static list of products for demonstration purposes
        private static readonly List<Product> Products = new()
        {
            new Product { Id = 1, Name = "Product A", Description = "Description for Product A", Price = 10.99M },
            new Product { Id = 2, Name = "Product B", Description = "Description for Product B", Price = 20.99M },
            new Product { Id = 3, Name = "Product C", Description = "Description for Product C", Price = 15.99M }
        };

        // Only one declaration of Cart
        private static readonly List<CartItem> Carts = new List<CartItem>();

        [HttpGet]
        public IActionResult Index()
        {
            return View("Vulnerable/Index");  // Pass products to the view
        }
        
        [HttpGet("Product")]
        public IActionResult Product()
        {
            return View("Vulnerable/Product", Products); 
        }
        
        [HttpPost("AddToCart")]
        public IActionResult AddToCart(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            var cartItem = Carts.FirstOrDefault(c => c.Product.Id == id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                Carts.Add(new CartItem { Product = product, Quantity = 1 });
            }

            return RedirectToAction("Cart");
        }

        [Route("/vulnerable/ResponseManipulation/Cart")]
        [HttpGet("Cart")]
        public IActionResult Cart()
        {
            return View("Vulnerable/Cart", Carts);  // Pass cart items to the view
        }
        
        [Route("/vulnerable/ResponseManipulation/Checkout")]
        [HttpGet("Checkout")]
        public IActionResult Checkout()
        {
            try
            {
                // Calculate total items and total price
                int totalItems = Carts.Sum(item => item.Quantity);
                decimal totalPrice = Carts.Sum(item => item.Product.Price * item.Quantity);

                // Create OrderSummaryViewModel
                var orderSummary = new OrderSummaryViewModel
                {
                    TotalItems = totalItems,
                    TotalPrice = totalPrice
                };
                

                // Clear the cart after checkout
                Carts.Clear();

                return View("Vulnerable/Checkout", orderSummary);

            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine(ex);  // Replace with proper logging
                return StatusCode(500, "Internal server error");
            }
        }
        
        [Route("/vulnerable/ResponseManipulation/Payment")]
        [HttpPost("Payment")]
        public IActionResult Payment(OrderSummaryViewModel orderSummaryViewModel)
        {

            return View("Vulnerable/Pay", orderSummaryViewModel);
        }
        
        [Route("/Stripe/Paymentstatus")]
        [HttpPost]
        public IActionResult CompleteOrder(bool success) // getting reponse from user.
        {
            if (TempData.ContainsKey("OrderSummary") && success)
            {
                var orderSummaryJson = TempData["OrderSummary"].ToString();
                var orderSummary = JsonSerializer.Deserialize<OrderSummaryViewModel>(orderSummaryJson);

                // Copy cart items to order items
                orderSummary.OrderItems = new List<CartItem>(Carts);

                // Clear the cart after processing the order
                Carts.Clear();

                // Return the CompleteOrder view with the order summary
                return View("Vulnerable/CompleteOrder", orderSummary);
            }

            // If we reach this point, something went wrong; redirect back to cart
            return View("Vulnerable/Failed");
        }
        
        
        // payment 
        
        // Default card details
        private const string DefaultCardNumber = "4111111111111111"; // Example Visa card number
        private const string DefaultExpiryDate = "12/25"; // Example expiry date (MM/YY)
        private const string DefaultCVV = "123"; // Example CVV

        
        [Route("/Stripe/PaymentProcess")]
        [HttpPost]
        public IActionResult Stripe(OrderSummaryViewModel orderSummary)
        {
            // Simulate payment processing
            bool paymentSuccess = ProcessPayment(orderSummary);

            // Serialize the order summary into TempData for retrieval in CompleteOrder
            TempData["OrderSummary"] = JsonSerializer.Serialize(orderSummary);

            // Use ViewData to pass success status to the view
            ViewData["PaymentSuccess"] = paymentSuccess;

            // Return the redirect view
            return View("Vulnerable/StripeRedirect");
        }

        private bool ProcessPayment(OrderSummaryViewModel orderSummary)
        {
            // Check if the provided card details match the default values
            if (orderSummary.CardNumber == DefaultCardNumber &&
                orderSummary.ExpiryDate == DefaultExpiryDate &&
                orderSummary.CVV == DefaultCVV)
            {
                return true;
            }

            return false;
        }
}
