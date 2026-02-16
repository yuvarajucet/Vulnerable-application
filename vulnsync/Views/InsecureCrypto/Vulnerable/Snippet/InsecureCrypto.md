## Insecure / Weak encryption / encoding techniques

### Sample Code
```csharp
private string GenerateMDHash(string input)
{
    using (MD5 md5 = MD5.Create())
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        StringBuilder sb = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
}
```
<br/>

In this case the `md5` hashing algorithm create encrypted values but it easy to find the plain text by doing
bruteforce attack.