## Secured from Open Redirect
### Description
Will see how to mitigate open redirect vulnerability.

**Backend**
```csharp
[HttpPost("login")]
public IActionResult SecuredLogin([FromBody] LoginModel userInfo)
{
    if (userInfo.Username == "john" && userInfo.Password == "John@123")
    {
        return Ok(new {
            redirectUrl = userInfo.ReturnUrl,
            success = true,
            errorMessage = string.Empty,
            isValidurl = IsValidRedirectUrl(userInfo.ReturnUrl)
        });
    }
    return Ok(new {
        success = false,
        errorMessage = "Username or password is incorrect"
    });
}
```

<br/>

If you look this snippet you can see `IsValidRedirectUrl` method call.
<br/>

```csharp
private bool IsValidRedirectUrl(string url)
{
    if (string.IsNullOrWhiteSpace(url)) return false;
    // Check the url is Relative or not
    Uri redirectUri;
    if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out redirectUri))
    {
        return false;
    }
    // Check if the url is absolute or not
    if (!redirectUri.IsAbsoluteUri)
    {
        return true;
    }
    // If url is absolute compare with current host
    var requestHost = HttpContext.Request.Host.ToString();
    return string.Equals(redirectUri.Host, requestHost, StringComparison.OrdinalIgnoreCase);
}
```
<br/>

Here I validated the redirect URl before passing into front end, ensuring that it is either a relative URL or matches the host of the current request. This prevents attackers from redirecting users to malicious sites by validating that the redirect URL is safe and intended.

```javascript
$.ajax({
    url: '/secured/openredirect/login',
    type: 'POST',
    data: JSON.stringify({ Username: username, Password: password, ReturnUrl: redirectUrl }),
    contentType: 'application/json',
    success: function (response) {
        debugger;
        if (response.success) {
            if (response.isValidurl) {
                window.location.href = response.redirectUrl;
            } else {
                window.location.href = "warning"
            }
        } else {
            $('#username').val("");
            $('#password').val("");
            $('#error-message').text(response.errorMessage || 'Login failed');
        }
    },
    error: function () {
        $('#error-message').text('An error occurred. Please try again.');
    }
});
```
<br/>

on this front end handling checked the status of the url `response.isValidurl` if it's valid directly redirect user to targetting page. else it redirects the user to "warning" page to to inform them about the potential risk.

```javascript
    let status = confirm('This site redirecting you to\n' + response.redirectUrl + '\nAre you sure want to continue?');
    if (status) {
            window.location.href = response.redirectUrl;
    } else {
        window.location.href = 'login?redirectUrl=profile';
    }
```
<br/>

You can implement similar validation in backend and instead of asking user for confirmation just add valid redirection URL from backend else redirect from backend.