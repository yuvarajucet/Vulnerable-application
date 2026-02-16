## What is Host Header Injection?
`Host Header Injection (HHI)` is a type of security vulnerability that occurs when an attacker is able to manipulate the Host header in an HTTP request. The Host header is used to specify the domain name of the server (virtual host) that the client is requesting resources from. If an application does not properly validate or sanitize this header, an attacker can inject malicious input, potentially leading to various security issues such as:
1. **Cross-Site Scripting (XSS)**
2. **Server-Side Request Forgery (SSRF)**
3. **Cache Poisoning**
4. **Denial of Service (DoS)**
5. **Session Hijacking**

#### Login credentials:
- username: `praveen`
- password: `1234`