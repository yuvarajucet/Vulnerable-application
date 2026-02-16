### <u>Cross Site Scripting(XSS)</u>

##### **Description**
Will see the vulernable XSS application.

This is an eCommerce application with search and comment fields. These fields are not properly sanitized, allowing attackers to easily inject malicious XSS scripts. This leads to reflected XSS vulnerabilities in the search functionality and stored XSS vulnerabilities in the comment section. Reflected XSS occurs when malicious scripts are immediately executed and returned to the user via input, while stored XSS persists in the system and impacts multiple users.

##### **Reflected XSS**

```csharp
public IActionResult Search(string search)
{
    if (!string.IsNullOrEmpty(search))
    {
        // Reflected XSS Vulnerability: Directly rendering unescaped user input in ViewBag
        ViewBag.SearchQuery = search;
        return View("Vulnerable");
    }
    return View("Vulnerable");
}
``````
<br/>

```html
<form method="get" action="/Xss/Reflect">
    <label for="search">Enter Search Query:</label>
    <input type="text" name="search" id="search" value="@ViewBag.SearchQuery" />
    <button type="submit">Search</button>
</form>

<!-- Dangerous: Directly injecting user input into the page -->
@if (!string.IsNullOrEmpty(ViewBag.SearchQuery))
{
    <p>Search results for: <b>@ViewBag.SearchQuery</b></p>
}
```
<br/>

##### **Stored XSS**

```csharp
[HttpPost]
public IActionResult AddComment(int productId, string userComment)
{
    // Simulate loading products from the "database"
    var products = LoadProductsFromDatabase();
    var product = products.FirstOrDefault(p => p.Id == productId);
    
    if (product == null)
    {
        return NotFound();
    }
    
    // Vulnerable Stored XSS: No sanitization or validation on user input (comment)
    if (!string.IsNullOrEmpty(userComment))
    {
        if (product.Comments == null)
        {
            product.Comments = new List<string>();
        }
        
        // Store the comment (including any malicious script)
        product.Comments.Add(userComment);  // Example: <script>alert('XSS')</script>
        
        // Save updated commants in "database"
        SaveProductsToDatabase(products);
    }
    
    return RedirectToAction("ViewProductComments", new { id = productId });
}

``````
<br/>

```html
<div class="container">
    <div class="product-card">
        <img src="https://via.placeholder.com/300" alt="@Model.Name">
        <h2>@Model.Name</h2>
        <p>@Model.Description</p>
        <div class="price">$@Model.Price</div>
    </div>
    <h3>Comments</h3>
    <div class="comments-section">
        @if (Model.Comments != null && Model.Comments.Any())
        {
            foreach (var comment in Model.Comments)
            {
            <!-- Stored XSS: Rendering comments as raw HTML -->
            <div class="comment">
                @Html.Raw(comment)
            </div>
            }
        }
        else
        {
            <p>No comments yet.</p>
        }
    </div>
```
<br/>

Attackers exploit XSS vulnerabilities by injecting malicious scripts into vulnerable fields, such as search bars or comment sections. For reflected XSS, the script is embedded in a URL and executed when the victim opens the link. In stored XSS, the malicious payload is saved on the server (e.g., in comments) and executed whenever a user views the infected page. Attackers use these techniques to steal cookies, session tokens, or sensitive data, or to impersonate victims. Proper input validation and output encoding can help prevent such attacks.

