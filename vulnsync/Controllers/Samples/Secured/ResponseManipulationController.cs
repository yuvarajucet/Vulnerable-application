using Microsoft.AspNetCore.Mvc;
using VulnSync.Models.Vulnerable.ResponseManipulation;
using System.Text.Json;
using System.IO;

namespace VulnSync.Controllers.Samples.Secured;

[Route("secured/[controller]")]

public class ResponseManipulationController : Controller
{
    private static readonly List<Product> Products = new()
    {
        new Product { Id = 1, Name = "Product A", Description = "Description for Product A", Price = 10.99M },
        new Product { Id = 2, Name = "Product B", Description = "Description for Product B", Price = 20.99M },
        new Product { Id = 3, Name = "Product C", Description = "Description for Product C", Price = 15.99M }
    };

    private static readonly List<CartItem> Carts = new List<CartItem>();
    private const string StripeDbFilePath = "StripeDB.json"; // Simulate Stripe DB as a JSON file

    // Default card details for payment processing simulation
    private const string DefaultCardNumber = "4111111111111111"; // Example Visa card number
    private const string DefaultExpiryDate = "12/25";            // Example expiry date (MM/YY)
    private const string DefaultCVV = "123";

    [HttpGet]
    public IActionResult SecuredIndex()
    {
        return View("Secured/Index");  // Pass products to the view
    }
        
    [HttpGet("SecuredProduct")]
    public IActionResult SecuredProduct()
    {
        return View("Secured/Product", Products); 
    }

    [HttpPost("SecuredAddToCart")]
    public IActionResult SecuredAddToCart(int id)
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

        return RedirectToAction("SecuredCart");
    }

    [Route("/secured/ResponseManipulation/Cart")]
    [HttpGet("SecuredCart")]
    public IActionResult SecuredCart()
    {
        return View("Secured/Cart", Carts);  // Pass cart items to the view
    }

    [Route("/secured/ResponseManipulation/Checkout")]
    [HttpGet("SecuredCheckout")]
    public IActionResult SecuredCheckout()
    {
        int totalItems = Carts.Sum(item => item.Quantity);
        decimal totalPrice = Carts.Sum(item => item.Product.Price * item.Quantity);

        var orderSummary = new OrderSummaryViewModel
        {
            TotalItems = totalItems,
            TotalPrice = totalPrice
        };

        return View("Secured/Checkout", orderSummary);
    }

    [Route("/secured/ResponseManipulation/Payment")]
    [HttpPost("SecuredPayment")]
    public IActionResult SecuredPayment(OrderSummaryViewModel orderSummaryViewModel)
    {
        return View("Secured/Pay", orderSummaryViewModel);
    }

    // Stripe process
    [Route("/Stripe/SecuredPaymentProcess")]
    [HttpPost("PaymentVerification")]
    public IActionResult PaymentVerification(OrderSummaryViewModel orderSummary)
    {
        // Generate Order ID if not already provided
        if (string.IsNullOrEmpty(orderSummary.OrderId))
        {
            orderSummary.OrderId = Guid.NewGuid().ToString(); // Create a new Order ID
        }

        bool paymentSuccess = SecuredProcessPayment(orderSummary);

        // Create Stripe payment info
        var paymentInfo = new StripePaymentInfo
        {
            OrderId = orderSummary.OrderId,
            CardNumber = orderSummary.CardNumber,
            ExpiryDate = orderSummary.ExpiryDate,
            CVV = orderSummary.CVV,
            Success = paymentSuccess
        };

        // Use the StripePaymentInfoStorage to store payment info
        var paymentInfoStorage = new StripePaymentInfoStorage(StripeDbFilePath);
        paymentInfoStorage.StorePaymentInfo(paymentInfo);

        orderSummary.success = paymentSuccess;

        // Serialize the order summary into TempData for retrieval in CompleteOrder
        TempData["OrderSummary"] = JsonSerializer.Serialize(orderSummary);

        return RedirectToAction("SecuredCheckPaymentStatus", new { orderId = orderSummary.OrderId });
    }

    private bool SecuredProcessPayment(OrderSummaryViewModel orderSummary)
    {
        return orderSummary.CardNumber == DefaultCardNumber &&
               orderSummary.ExpiryDate == DefaultExpiryDate &&
               orderSummary.CVV == DefaultCVV;
    }

    // Internal call
    [Route("/product/CheckPaymentStatus")]
    [HttpPost("SecuredCheckPaymentStatus")]
    public IActionResult SecuredCheckPaymentStatus(string orderId)
    {
        var paymentStatus = GetPaymentStatus(orderId);

        if (paymentStatus)
        {
            var orderSummaryJson = TempData["OrderSummary"]?.ToString();
            if (orderSummaryJson != null)
            {
                var orderSummary = JsonSerializer.Deserialize<OrderSummaryViewModel>(orderSummaryJson);

                // Ensure OrderItems is initialized to avoid NullReferenceException
                orderSummary.OrderItems ??= new List<CartItem>();

                return View("Secured/CompleteOrder", orderSummary);
            }
        }

        return View("Secured/Failed");
    }

    private bool GetPaymentStatus(string orderId) // Getting payment details from strip DB
    {
        var records = LoadPaymentRecords();

        // Get data from stripe DB
        if (records.TryGetValue(orderId, out var record))
        {
            return record.Success;
        }

        return false;
    }

    private Dictionary<string, StripePaymentInfo> LoadPaymentRecords()
    {
        var storage = new StripePaymentInfoStorage(StripeDbFilePath);
        return storage.LoadPaymentRecords();
    }
    }

    public class StripePaymentInfoStorage
    {
        private readonly string _filePath;

        public StripePaymentInfoStorage(string filePath)
        {
            _filePath = filePath;
        }

        public void StorePaymentInfo(StripePaymentInfo paymentInfo)
        {
            if (string.IsNullOrEmpty(paymentInfo.OrderId))
            {
                throw new ArgumentNullException(nameof(paymentInfo.OrderId), "Order ID cannot be null or empty.");
            }

            var existingRecords = LoadPaymentRecords();
            existingRecords[paymentInfo.OrderId] = paymentInfo;

            try
            {
                System.IO.File.WriteAllText(_filePath, JsonSerializer.Serialize(existingRecords));
            }
            catch (IOException ex)
            {
                // Handle file write errors (log or throw exception)
                throw new Exception("Failed to write payment information.", ex);
            }
        }

        public Dictionary<string, StripePaymentInfo> LoadPaymentRecords()
        {
            if (!System.IO.File.Exists(_filePath))
                return new Dictionary<string, StripePaymentInfo>();

            try
            {
                var jsonContent = System.IO.File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<Dictionary<string, StripePaymentInfo>>(jsonContent) ?? new Dictionary<string, StripePaymentInfo>();
            }
            catch (IOException ex)
            {
                // Handle file read errors (log or throw exception)
                throw new Exception("Failed to read payment records.", ex);
            }
        }
  }
