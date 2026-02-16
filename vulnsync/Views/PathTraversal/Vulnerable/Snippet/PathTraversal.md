## Vulnerable sample for Path Traversal
The following code demonstrates a vulnerable implementation of file download functionality that is susceptible to path traversal attacks:

```csharp
[HttpGet("downloadFile")]
public IActionResult DownloadFile(string fileName)
{
if (string.IsNullOrEmpty(fileName))
return BadRequest("Filename cannot be empty");
// Vulnerable: Direct concatenation of user input into file path
var filePath = Path.Combine(uploadDirectory, fileName);
if (!System.IO.File.Exists(filePath))
return NotFound("File not found");
// Read and return the file
var fileBytes = System.IO.File.ReadAllBytes(filePath);
return File(fileBytes, "application/octet-stream", fileName);
}
```

### Vulnerability Explanation
This code is vulnerable to path traversal attacks because:

1. It accepts a filename parameter directly from user input without sanitization
2. The user-provided filename is directly combined with the upload directory
3. No validation is performed to prevent directory traversal sequences (../../../)

### Attack Scenario
An attacker could:
1. Request: `GET /api/download?fileName=../../../etc/passwd`
2. Access files outside the intended directory
3. Potentially read sensitive system files or configuration files

### Impact
- Unauthorized access to files outside the intended directory
- Information disclosure
- Potential system compromise

### How to Fix
To secure this code:
1. Use `Path.GetFileName()` to strip path information
2. Validate file extensions
3. Ensure files are within the intended directory
4. Implement proper access controls

See the secured implementation in the [Secured Sample](../Secured/PathTraversal) section.




