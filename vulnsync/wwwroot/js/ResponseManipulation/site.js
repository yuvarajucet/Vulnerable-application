// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function validateForm() {
    const cardNumber = document.getElementById('cardNumber').value;
    const expiryDate = document.getElementById('expiryDate').value;
    const cvv = document.getElementById('cvv').value;

    // Check for valid card number (16 digits)
    if (!/^\d{16}$/.test(cardNumber)) {
        alert("Please enter a valid card number (16 digits).");
        return false;
    }

    // Check for valid expiry date (MM/YY)
    const [month, year] = expiryDate.split('/');
    const now = new Date();
    const currentYear = now.getFullYear() % 100; // Get last two digits of current year
    const currentMonth = now.getMonth() + 1; // Months are 0-based

    if (!/^(0[1-9]|1[0-2])\/([0-9]{2})$/.test(expiryDate) ||
        (year < currentYear || (year == currentYear && month < currentMonth))) {
        alert("Please enter a valid expiry date that is not in the past.");
        return false;
    }

    // Check for valid CVV (3 or 4 digits)
    if (!/^\d{3,4}$/.test(cvv)) {
        alert("Please enter a valid CVV (3 or 4 digits).");
        return false;
    }

    return true; // All validations passed
}