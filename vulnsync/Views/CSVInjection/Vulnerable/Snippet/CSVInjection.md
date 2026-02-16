## Vulnerable sample for CSV Injection

#### Vulnerability Explanation:
This vulnerability occurs when user input is not properly sanitized before being written to a CSV file. Malicious users can inject formulas or scripts that may execute when the CSV is opened in spreadsheet applications, potentially leading to data leakage or other security issues.

#### Sample code:
```csharp
    [HttpPost("SubmitFeedback")]
    public IActionResult SubmitFeedback(string name, string email, string feedback)
    {
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
        
        var csvLine = $"{name.Trim()},{email.Trim()},{feedback.Trim()}\n";
        
        System.IO.File.AppendAllText(filePath, csvLine);
        
        ViewBag.Message = "Feedback submitted successfully";
        return View("Vulnerable/Feedbackform");
    }
```