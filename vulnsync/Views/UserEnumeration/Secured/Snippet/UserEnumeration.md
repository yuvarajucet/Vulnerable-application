## Preventing User Enumeration

This example demonstrates how to display a **generic error message** to prevent **user enumeration attacks**.

When users try to log in or register, show a generic message like:

> "The username or password is incorrect."

![Secure1.png](Assets/Image/UserEnumeration/Secure1.png)

By displaying the same message for both valid and invalid usernames, attackers cannot distinguish between valid and invalid entries, thus preventing user enumeration.
