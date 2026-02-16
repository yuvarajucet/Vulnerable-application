const defaultUsername = "user123";
const defaultPassword = "password123";

// Function to handle form submission
document.getElementById("loginForm").addEventListener("submit", function(event) {
    event.preventDefault(); // Prevents the form from submitting the default way

    const enteredUsername = document.getElementById("username").value;
    const enteredPassword = document.getElementById("password").value;
    const message = document.getElementById("message");

    // Clear any previous messages
    message.textContent = "";

    // Step 1: Check if the username is correct
    if (enteredUsername !== defaultUsername) {
        message.textContent = "Username or password is incorrect!";
        message.style.color = "red";
    }
    // Step 2: If username is correct, check the password
    else if (enteredPassword !== defaultPassword) {
        message.textContent = "Username or password is incorrect!";
        message.style.color = "red";
    }
    // Step 3: If both are correct, show success message
    else {
        message.textContent = "Login successful!";
        message.style.color = "green";
    }
});