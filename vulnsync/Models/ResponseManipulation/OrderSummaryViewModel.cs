namespace VulnSync.Models.Vulnerable.ResponseManipulation;
using System.ComponentModel.DataAnnotations;

public class OrderSummaryViewModel
{
    public int TotalItems { get; set; }
    public decimal TotalPrice { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    
    [Required]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Please enter a valid card number (16 digits).")]
    public string CardNumber { get; set; }

    [Required]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Please enter a valid expiry date (MM/YY).")]
    public string ExpiryDate { get; set; }

    [Required]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Please enter a valid CVV (3 or 4 digits).")]
    public string CVV { get; set; }
    
    public bool success { get; set; }
    
    public string OrderId { get; set; }
    public List<CartItem> OrderItems { get; set; } // Assuming CartItem contains Product and Quantity
}