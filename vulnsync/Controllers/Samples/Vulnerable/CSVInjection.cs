using Microsoft.AspNetCore.Mvc;

namespace vulnsync.Controllers.Samples.Vulnerable;

[Route("Vulnerable/[controller]")]
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
        return View("Vulnerable/Login");
    }

    [HttpGet("SubmitFeedback")]
    public IActionResult SubmitFeedback()
    {
        return View("Vulnerable/Feedbackform");
    }

    [HttpGet("AdminPanel")]
    public IActionResult AdminPanel()
    {
        return View("Vulnerable/AdminPanel");
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _userService.AuthenticateUser(username, password);

        if (user != null)
        {
            return View("Vulnerable/Feedbackform");
        }
        else
        {
            ViewBag.ErrorMessage = "Invalid username or password";
            return View("Vulnerable/Login");
        }
    }

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



