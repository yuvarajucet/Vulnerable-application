## Vulnerable to Open Redirect
#### Description
Will see the open redirect leads to Credentials theft vulnerability.


**BackEnd**
```csharp
[HttpPost("login")]
public IActionResult VulnerableLogin([FromBody] LoginModel userInfo)
{
    if (userInfo.Username == "john" && userInfo.Password == "John@123")
    {
        return Ok(new {
            redirectUrl = userInfo.ReturnUrl,
            success = true,
            errorMessage = string.Empty
        });
    }
    return Ok(new {
        success = false,
        errorMessage = "Username or password is incorrect"
    });
}
```
<br/>

**Front End**
```javascript
$.ajax({
    url: '/vulnerable/openredirect/login',
    type: 'POST',
    data: JSON.stringify({ Username: username, Password: password, ReturnUrl: redirectUrl }),
    contentType: 'application/json',
    success: function (response) {
        if (response.success) {
            window.location.href = response.redirectUrl;
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

above code block the `userInfo.ReturnUrl` got from user and return the same URL to frontend without any kind of validation. and in frontend they just use it directly for redirection like this ` window.location.href = response.redirectUrl;`, which can lead to an open redirect vulnerability.

