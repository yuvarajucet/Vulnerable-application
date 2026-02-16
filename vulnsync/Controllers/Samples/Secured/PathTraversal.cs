using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.RegularExpressions;

namespace VulnSync.Controllers.Samples.Secured;

[Route("Secured/[controller]")]
public class PathTraversalController : Controller
{
    private readonly string _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    private readonly string[] _allowedExtensions = { ".csv" };

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
        return View("Secured/FileUpload");
    }

    [HttpPost]
    public IActionResult FileUpload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ViewBag.ErrorMessage = "Please select a file to upload.";
            return View("Secured/FileUpload");
        }

        try
        {
            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                ViewBag.ErrorMessage = "Only CSV files are allowed.";
                return View("Secured/FileUpload");
            }

            // Simplified filename sanitization - Path.GetFileName already removes path traversal attempts
            var fileName = Path.GetFileName(file.FileName);
            
            // Add timestamp to ensure uniqueness
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var finalFileName = $"{timestamp}_{fileName}";
            
            var filePath = Path.Combine(_uploadDirectory, finalFileName);
            
            // Additional path traversal check
            if (!Path.GetFullPath(filePath).StartsWith(Path.GetFullPath(_uploadDirectory)))
            {
                ViewBag.ErrorMessage = "Invalid file path detected.";
                return View("Secured/FileUpload");
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            ViewBag.SuccessMessage = "File uploaded successfully!";
            return View("Secured/FileUpload");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred while uploading the file.";
            return View("Secured/FileUpload");
        }
    }

    [HttpGet("SecuredDownloadFile")]
    public IActionResult SecuredDownloadFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return BadRequest("Invalid file name");
        }

        // Simplified filename sanitization
        //var sanitizedFileName = Regex.Replace(fileName, @"[^a-zA-Z0-9\._]", "");
        var sanitizedFileName = Path.GetFileName(fileName);
        var filePath = Path.Combine(_uploadDirectory, sanitizedFileName);

        // Verify the final path is within the upload directory
        if (!Path.GetFullPath(filePath).StartsWith(Path.GetFullPath(_uploadDirectory)))
        {
            return BadRequest("Invalid file path");
        }

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File not found");
        }

        // Verify file extension
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            return BadRequest("Invalid file type");
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "text/csv", sanitizedFileName);
    }
}
