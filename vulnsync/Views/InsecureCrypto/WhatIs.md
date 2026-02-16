## Insecure cryptography
**Insecure Cryptography** refers to the use of **weak, outdated, or improperly implemented cryptographic algorithms** that can be easily broken, leading to security risks like data breaches and unauthorized access.

### **Common Issues:**
1. **Weak Algorithms** → MD5, SHA-1, DES (easily cracked)
2. **Improper Key Management** → Hardcoded keys, weak key lengths
3. **Use of Non-Random IVs** → Predictable encryption patterns
4. **Lack of Salting** → Easy hash cracking (e.g., password hashes)
5. **ECB Mode Usage** → Leaks patterns in encrypted data

💡 **Best Practice:** Always use **strong encryption standards** like AES-256, RSA-2048, SHA-256, and secure key management techniques. 🚀

## Sample credentials
`username` : `john` <br/>
`password` : `John@123`