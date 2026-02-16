### <u>Response Manipulation Prevention</u>

##### **Description**
Will see the response manipulation mitigation steps.


In this mitigation process we are getting order id from stripe instead of bool response and using that order id to make internal call to contact stripe database and verify that details from strip then confirm the order.

```csharp
// This method checks the payment status for a secured transaction.
public IActionResult SecuredCheckPaymentStatus(string orderId)
{
    // Fetch payment status from Strip DB
    bool paymentStatus = GetPaymentStatus(orderId);
    
    // Retrieve order details from the own DB using the orderId.
    var order = CartService.GetOrderDetails(orderId);
    
    // Check if payment status is successful.
    if (paymentStatus)
    {
        // Place the order using the OrderService and store the resulting status.
        bool status = OrderService.PlaceOrder(order);
        
        // Update the order status based on the result of the place order operation.
        order.Status = status;
        
        // Save updated order detail
        CartService.SaveOrderDetails(order);
        
        // Return the view with the order details.
        return View(order);
    }
    
    // If payment status is not successful, return a "Failed" view.
    return View("Secured/Failed");
}

``````
<br/>

Below code is act as stripe database. 

````csharp
// This method retrieves and checks the payment status from the Stripe database.
private bool GetPaymentStatus(string orderId)
{
    // Verify that orderId is not null or in an incorrect format.
    if (string.IsNullOrEmpty(orderId))
    {
        return false;
    }
    // Retrieve payment records from the Stripe database using the given orderId.
    var paymentRecords = RetrieveStripeOrderDetails(orderId);
    // Check if the retrieval was successful.
    if (paymentRecords != null && paymentRecords.Success)
    {
        return true;
    }
    
    return false;
}

`````````
In this implementation attacker not able to modify the response.

