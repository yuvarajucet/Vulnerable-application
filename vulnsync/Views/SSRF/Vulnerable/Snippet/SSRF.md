### <u>SSRF</u>

##### **Description**
Will see the vulernable SSRF application.


In this application, the user dashboard displays profile details and license information. When the user clicks the license button, the application fetches data from an internal server and dynamically displays the related license details on the dashboard.

```csharp
[HttpPost("GetLicenseDetails")]
public async Task<IActionResult> GetLicenseDetails(string licenseDetail)
{
    if (string.IsNullOrWhiteSpace(licenseDetail))
    {
        return BadRequest("❌ Bad Request: User API URL is required.");
    }
    try
    {
        string getDetail = "http://localhost:5290" + licenseDetail;
        var request = new HttpRequestMessage(HttpMethod.Get, getDetail);
        
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            string userData = await response.Content.ReadAsStringAsync();
            ViewBag.LicenseData = userData;
            return View("Vulnerable/License");
        }
        
        return StatusCode((int)response.StatusCode, $"⚠️ Error: {response.StatusCode}");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"❌ Exception: {ex.Message}");
        return BadRequest("❌ Bad Request: Unable to reach the user server.");
    }
}
``````
<br/>

**Attack Technique:** In this flow, when a user clicks the **license button**, the application fetches data from an internal server and displays it on the dashboard. An attacker can intercept this request and manipulate the **unverified parameter** to target another internal server. This allows them to exploit **SSRF (Server-Side Request Forgery)** to access sensitive user details from the internal network.

