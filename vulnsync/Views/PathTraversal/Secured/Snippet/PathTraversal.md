## Secured sample for Path Traversal

The following code demonstrates a secured implementation of file download functionality that is resistant to path traversal attacks:

```csharp
[HttpGet("downloadFile")]
public IActionResult DownloadFile(string fileName)
{
    if (string.IsNullOrEmpty(fileName))
        return BadRequest("Filename cannot be empty");
        
    // Secure: Use Path.GetFileName to remove any path traversal attempts
    //var sanitizedFileName = Regex.Replace(fileName, @"[^a-zA-Z0-9\._]", "");   --or--
    var sanitizedFileName = Path.GetFileName(fileName);
    
    // Secure: Combine paths safely using Path.Combine
    var filePath = Path.Combine(_uploadDirectory, sanitizedFileName);
    
    // Verify the final path is within the intended directory
    var fullPath = Path.GetFullPath(filePath);
    if (!fullPath.StartsWith(_uploadDirectory))
        return BadRequest("Invalid file path");
        
    if (!System.IO.File.Exists(filePath))
        var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        _logger.LogWarning("Attempted access to non-existent file: {FilePath} from IP: {UserIp}",filePath, userIp);
        return BadRequest("An error occurred while processing your request");
        
    // Read and return the file
    var fileBytes = System.IO.File.ReadAllBytes(filePath);
    return File(fileBytes, "application/octet-stream", sanitizedFileName);
}
```

### Security Measures Implemented

1. **Path Sanitization**
   - Uses `Path.GetFileName()` to strip any directory traversal attempts
   - Removes path information from user input, keeping only the filename

2. **Safe Path Construction**
   - Uses `Path.Combine()` to safely join paths
   - Handles platform-specific directory separators correctly

3. **Path Validation**
   - Verifies the final path is within the intended directory
   - Prevents directory traversal even if other protections fail

4. **Input Validation**
   - Checks for null or empty filenames
   - Returns appropriate error messages

### Additional Recommendations

1. **File Extension Validation**
   ```csharp
   var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
   var extension = Path.GetExtension(sanitizedFileName).ToLowerInvariant();
   if (!allowedExtensions.Contains(extension))
       return BadRequest("Invalid file extension");
   ```

2. **Access Control**
   ```csharp
   // Implement user authentication
   [Authorize]
   public class FileController : Controller
   
   // Check user permissions
   if (!UserHasAccessToFile(sanitizedFileName))
       return Forbid();
   ```

3. **Logging**
   ```csharp
   _logger.LogInformation($"File download requested: {sanitizedFileName}");
   ```

4. **Attack vectors mitigation**
   ```csharp
   public class PathTraversalMitigation
   {
        public static bool IsSafeFilePath(string filePath)
        {
            // 1. Reject null byte injection (%00)
            if (filePath.Contains("%00") || filePath.Contains("\0"))
            {
                return false;
            }
            // 2. Decode the file path to prevent double encoding attacks
            string decodedPath = HttpUtility.UrlDecode(filePath);
            decodedPath = HttpUtility.UrlDecode(decodedPath);
            // 3. Check for directory traversal attempts ('../' or '\' or '/')
            if (decodedPath.Contains("..") || decodedPath.Contains("\\") || decodedPath.Contains("/"))
            {
                return false;
            } 
            // 4. Validate user-defined file names to exclude dangerous characters
            string fileNamePattern = @"^[^<>:""/\\|?*\x00-\x1F]*$";
            if (!Regex.IsMatch(decodedPath, fileNamePattern))
            {
                return false;
            }
            // 5. Prevent symbolic links (symlinks)
            if (IsSymlink(decodedPath))
            {
                return false;
            }
            // 6. Ensure the file is within a designated safe directory (e.g., C:\Uploads\)
            string safeDirectory = @"C:\Uploads\";
            string fullPath = Path.GetFullPath(decodedPath);
            if (!fullPath.StartsWith(safeDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }
        // Helper method to check if a file is a symbolic link
        private static bool IsSymlink(string path)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                return fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
            }
            catch
            {
                return false;
            }
        }
    }
   ```

### Benefits
- Prevents directory traversal attacks
- Maintains secure file access boundaries
- Platform-independent solution
- Follows security best practices

See the vulnerable implementation in the [Vulnerable Sample](../Vulnerable/PathTraversal) section to understand the risks of improper implementation.

