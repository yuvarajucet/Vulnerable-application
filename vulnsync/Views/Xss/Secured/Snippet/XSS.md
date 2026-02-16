### <u>Cross Site Scripting(XSS)</u>

##### **Description**
Will see the Secured XSS application.

This is an eCommerce application with search and comment fields. These fields are not properly sanitized, allowing attackers to easily inject malicious XSS scripts. This leads to reflected XSS vulnerabilities in the search functionality and stored XSS vulnerabilities in the comment section. Reflected XSS occurs when malicious scripts are immediately executed and returned to the user via input, while stored XSS persists in the system and impacts multiple users.

##### **Reflected XSS**

```csharp
public IActionResult Reflect(string search)
{
    if (!string.IsNullOrEmpty(search))
    {
        // Safe: Input passed as-is; encoding is handled in the view
        ViewBag.SearchQuery = search;
        return View("Secure");
    }
    
    return View("Secure");
}
``````
<br/>

```html
<h1>Secure Search</h1>

<form method="get" action="/Xss/Reflect">
    <label for="search">Enter Search Query:</label>
    <input type="text" name="search" id="search" value="@Html.Encode(ViewBag.SearchQuery)" />
    <button type="submit">Search</button>
</form>

@if (!string.IsNullOrEmpty(ViewBag.SearchQuery))
{
<p>Search results for: <b>@Html.Encode(ViewBag.SearchQuery)</b></p>
}
```
##### **Stored XSS**

```csharp
[HttpPost]
[ValidateAntiForgeryToken] // CSRF Protection
public IActionResult AddComment(int productId, string userComment)
{
    if (string.IsNullOrEmpty(userComment))
    {
        ModelState.AddModelError("Comment", "Comment cannot be empty.");
        return View();
    }
    //The validation of user input ensures that it follows the required format and does not contain any sensitive or malicious code.
    string sanitizedComment = SanitizeInput(userComment);
    if (string.IsNullOrWhiteSpace(sanitizedComment))
    {
        ModelState.AddModelError("Comment", "Comment contains invalid characters.");
        return View();
    }
    // Encoding the comment to prevent XSS
    var encodedComment = System.Net.WebUtility.HtmlEncode(sanitizedComment);
    
    //Load products (simulating from DB)
    var products = LoadProductsFromDatabase(); // Replace with actual DB load logic
    var product = products.FirstOrDefault(p => p.Id == productId);
    if (product == null)
    {
        return NotFound();
    }
    
    product.Comments.Add(encodedComment);
    
    // Save updated products back to the "database"
    SaveProductsToDatabase(products); // Replace with actual DB save logic
    return RedirectToAction("ViewProductComments", new { id = productId });
}
``````
<br/>

```html
//Ensure that all user-generated content is properly encoded (e.g., HTML encoding) before rendering it to the view to prevent Cross-Site Scripting (XSS) attacks.
<div class="product-card">
    <img src="https://via.placeholder.com/300" alt="@Html.Encode(Model.Name)">
    <h2>@Html.Encode(Model.Name)</h2>
    <p>@Html.Encode(Model.Description)</p>
    <div class="price">$@Html.Encode(Model.Price)</div>
</div>

<h3>Comments</h3>
<div class="comments-section">
    @if (Model.Comments != null && Model.Comments.Any())
    {
        foreach (var comment in Model.Comments)
        {
        <!-- Securely render comment -->
        <div class="comment">
            @Html.Encode(comment)
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

This solution is secure because it ensures that all user input is encoded before being rendered in the HTML, preventing any potentially harmful scripts from executing. By using `@Html.Encode()`, it converts characters like `<` and `>` into their safe HTML equivalents (`&lt;` and `&gt;`), so they are displayed as plain text rather than executed as JavaScript. Razor’s default behavior also helps by automatically encoding any values rendered into the view, further mitigating XSS risks. This prevents attackers from injecting malicious scripts through user input. In essence, encoding ensures that only safe characters are interpreted, making it impossible for harmful scripts to run.

