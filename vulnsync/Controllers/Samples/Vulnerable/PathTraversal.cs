using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace VulnSync.Controllers.Samples.Vulnerable;

[Route("Vulnerable/[controller]")]
public class PathTraversalController : Controller
{
    private readonly string _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

    public PathTraversalController()
    {
        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }

    [HttpGet]
    public IActionResult FileUpload()
    {
        return View("Vulnerable/FileUpload");
    }

    [HttpPost]
    public IActionResult FileUpload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ViewBag.ErrorMessage = "Please select a file to upload.";
            return View("Vulnerable/FileUpload");
        }

        try
        {
            // Vulnerable: Directly using the filename without sanitization
            var filePath = Path.Combine(_uploadDirectory, file.FileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            ViewBag.SuccessMessage = "File uploaded successfully!";
            return View("Vulnerable/FileUpload");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"Error uploading file: {ex.Message}";
            return View("Vulnerable/FileUpload");
        }
    }

    [HttpGet("downloadFile")]
    public IActionResult DownloadFile(string fileName)
    {
        // Vulnerable: Directly using the filename without validation
        var filePath = Path.Combine(_uploadDirectory, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File not found");
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/octet-stream", fileName);
    }
}
