## Vulnerable Example
```html
<!DOCTYPE html>
<html>
<head>
    <title>Clickjacking Vulnerable Page</title>
</head>
<body>
    <h1>Clickjacking Vulnerable Page</h1>
    <p>This page is vulnerable to clickjacking attacks.</p>
    <iframe src="https://example.com" width="500" height="500"></iframe>
</body>
</html>
```

### Explanation:
- The page contains an **iframe** element that loads content from an external site.
- An attacker can use this to trick users into interacting with the page without their knowledge.

