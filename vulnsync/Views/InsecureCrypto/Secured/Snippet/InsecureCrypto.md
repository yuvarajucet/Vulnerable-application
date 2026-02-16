## Secure / Strong encryption / encoding techniques

### Sample Code
```csharp
private string EncrypteWithAES(string input)
    {
        string encryptionKey = "<TopSecretEncryptionKey>!";
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32, ' '));
        byte[] ivBytes = Encoding.UTF8.GetBytes("<TopSecretIV>");
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.IV = ivBytes;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            using (MemoryStream msEncrypt = new MemoryStream())
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                csEncrypt.Write(inputBytes, 0, inputBytes.Length);
                csEncrypt.FlushFinalBlock();
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
```
<br/>

Here we used `AES` algorithm to encrypt the give text based on `encryption key` and `IV`. even though
if attacker able to get the encrypted password they can't be able to decrypt the values without encryption key and IV bytes.