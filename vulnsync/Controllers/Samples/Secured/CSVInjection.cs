using Microsoft.AspNetCore.Mvc;

namespace vulnsync.Controllers.Samples.Secured;

[Route("Secured/[controller]")]
public class CSVInjectionController : Controller
{
    private readonly UserService _userService;
    private string _idorConnectionString;

    public CSVInjectionController()
    {
        _idorConnectionString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database/IDOR/database.db")}";

        _userService = new UserService(_idorConnectionString) ?? throw new ArgumentNullException(nameof(_userService));
        _userService.SeedUsers();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("Secured/Login");
    }

    [HttpGet("SubmitFeedback")]
    public IActionResult SubmitFeedback()
    {
        return View("Secured/Feedbackform");
    }

    [HttpGet("AdminPanel")]
    public IActionResult AdminPanel()
    {
        return View("Secured/AdminPanel");
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _userService.AuthenticateUser(username, password);

        if (user != null)
        {
            return View("Secured/Feedbackform");
        }
        else
        {
            ViewBag.ErrorMessage = "Invalid username or password";
            return View("Secured/Login");
        }
    }

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

    [HttpGet("ExportFeedbackCSV")]
    public IActionResult ExportFeedbackCSV()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Feedbacks", "feedback.csv");

        if (!System.IO.File.Exists(filePath))
        {
            ViewBag.Message = "No feedback data available for export.";
            return RedirectToAction("AdminPanel");
        }

        var fileContent = System.IO.File.ReadAllBytes(filePath);
        return File(fileContent, "text/csv", "feedback.csv");
    }
}



