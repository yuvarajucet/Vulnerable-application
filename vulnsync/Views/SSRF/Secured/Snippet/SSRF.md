### <u>SSRF</u>

##### **Description**
Will see the Secure SSRF application.


In this application, the user dashboard displays profile details and license information. When the user clicks the license button, the application fetches data from an internal server and dynamically displays the related license details on the dashboard.

```csharp
[HttpPost("SecuredGetLicenseDetails")]
public async Task<IActionResult> SecuredGetLicenseDetails(string licenseDetail)
{
    try
    {
        if (string.IsNullOrWhiteSpace(licenseDetail))
        {
            return BadRequest("❌ Bad Request: User API URL is required.");
        }
        // **STEP 1: Remove '@' tricks (attackers use this to obfuscate URLs)**
        licenseDetail = Regex.Replace(licenseDetail, @"^.*@", "");
        
        // **STEP 2: Define allowed base URLs**
        var allowedBaseUrl = "http://localhost:5290";
        string getDetail = allowedBaseUrl + licenseDetail;
        
        // **STEP 3: Normalize and validate the URL**
        if (!Uri.TryCreate(getDetail, UriKind.Absolute, out Uri? uri))
        {
            return BadRequest("❌ Invalid URL format.");
        }
        
        // **STEP 4: Decode encoded URLs**
        string decodedUrl = WebUtility.UrlDecode(uri.AbsoluteUri);
        
        // **STEP 5: Validate URL against allowed base domain**
        if (!decodedUrl.StartsWith(allowedBaseUrl, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("❌ Forbidden: Untrusted URL.");
        }
        
        // **STEP 6: Perform DNS resolution to detect DNS Rebinding**
        string host = uri.Host;
        IPAddress[] resolvedIPs = await Dns.GetHostAddressesAsync(host);
        
        // **Prevent private IP access (protects against SSRF & internal network leaks)**
        if (resolvedIPs.Any(ip => 
            IPAddress.IsLoopback(ip) ||  // Blocks 127.0.0.1, localhost
            ip.ToString().StartsWith("192.168.") || // Blocks private network (LAN)
            ip.ToString().StartsWith("10.") || // Blocks private network
            ip.ToString().StartsWith("172.16.") // Blocks private network
        ))
        {
            return BadRequest("❌ Forbidden: Internal network access is blocked.");
        }
        
        // **STEP 7: Process the secure API request**
        using (var request = new HttpRequestMessage(HttpMethod.Get, decodedUrl))
        {
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                string userData = await response.Content.ReadAsStringAsync();
                ViewBag.LicenseData = userData;
                return View("Secured/License");
            }
            else
            {
                return StatusCode((int)response.StatusCode, $"⚠️ Error: {response.StatusCode}");
            }
        }
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"❌ Exception: {ex.Message}");
        return StatusCode(500, "❌ Internal Server Error: Unable to reach the license server.");
    }
}
``````
<br/>

**Mitigation :** In this flow, when a user clicks the license button, the application securely processes data from the backend and enforces whitelisted domain validation before proceeding. This ensures that only authorized requests are handled, preventing attackers from targeting unauthorized internal services.

