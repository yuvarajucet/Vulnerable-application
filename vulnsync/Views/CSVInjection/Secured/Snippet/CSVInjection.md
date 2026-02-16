## Secured sample for CSV Injection

#### Security Enhancement Explanation:

**Input Sanitization:**
- A helper method SanitizeForCsv ensures inputs are safe for inclusion in a CSV file.
- This method prevents malicious input such as formulas from being executed in spreadsheet software.

#### Sample code:
```csharp
[HttpPost("SubmitFeedback")]
    public IActionResult SubmitFeedback(string name, string email, string feedback)
    {
        // Sanitize inputs to prevent CSV injection
        string SanitizeForCsv(string input)
        {
            // Replace leading =, +, -, or @ characters to prevent formulas
            if (!string.IsNullOrEmpty(input) && 
                (input.StartsWith("=") || input.StartsWith("+") || input.StartsWith("-") || input.StartsWith("@")))
            {
                input = "'" + input; // Prefix with a single quote
            }
            return input.Replace("\"", "\"\""); // Escape double quotes for CSV
        }
        
        // Sanitize each input
        string sanitizedName = SanitizeForCsv(name);
        string sanitizedEmail = SanitizeForCsv(email);
        string sanitizedFeedback = SanitizeForCsv(feedback);
        
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Feedbacks", "feedback.csv");
        
        var directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        if (!System.IO.File.Exists(filePath))
        {
            System.IO.File.AppendAllText(filePath, "Name, Email, Feedback\n");
        }
        
        var csvLine = $"{sanitizedName.Trim()},{sanitizedEmail.Trim()},{sanitizedFeedback.Trim()}\n";
        
        System.IO.File.AppendAllText(filePath, csvLine);
        
        ViewBag.Message = "Feedback submitted successfully";
        return View("Secured/Feedbackform");
    }
```
