namespace VulnSync.Models.AccountTakeover
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasskeyStatus { get; set; }
        public string PersonalDetails { get; set; }
        public string Passkeyname { get; set; }
        public string Passkeypassword { get; set; }
        
        // New passkey-related fields
        public string id { get; set; } // This will be a new unique identifier
        public string rawId { get; set; } // Same as id
        public string type { get; set; } // Should be "public-key"
        public string attestationObject { get; set; } // Randomly generated
        public string clientDataJSON { get; set; } // Randomly generated
        public string UserAgent { get; set; } // Fixed (browser info)
    }
}