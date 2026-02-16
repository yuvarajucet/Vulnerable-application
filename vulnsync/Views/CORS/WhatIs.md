## What is the CORS Policy Vulnerability?
A `CORS policy vulnerability arises` when a web server is misconfigured to allow unintended cross-origin access, potentially exposing sensitive data or enabling malicious actions. This happens when a server is too permissive in defining CORS rules, such as:

Using `Access-Control-Allow-Origin: *` – Allows any website to request and access sensitive data.
Allowing credentials (`Access-Control-Allow-Credentials: true`) with wildcard origins – Attackers can steal session cookies or authentication tokens.
Improperly implementing the allow-list – Accidentally permitting unauthorized domains to access restricted resources.

#### Login credentials:
- username: `praveen`
- password: `123`