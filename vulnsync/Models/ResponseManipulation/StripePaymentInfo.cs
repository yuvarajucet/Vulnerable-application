namespace VulnSync.Models.Vulnerable.ResponseManipulation;

public class StripePaymentInfo
{
    public string OrderId { get; set; }              // Unique identifier for the order
    public string CardNumber { get; set; }           // Masked or partial card number
    public string ExpiryDate { get; set; }           // Expiry date of the card
    public string CVV { get; set; }                  // CVV code of the card
    public bool Success { get; set; }                // Payment success status
}