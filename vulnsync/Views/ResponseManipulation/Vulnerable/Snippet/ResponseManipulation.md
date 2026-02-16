### <u>Response Manipulation</u>

##### **Description**
Will see the response manipulation due to accept client side input without validation.

#### This site is vulnerable to response manipulation

After user provided the card details. we are checking that data and pass that response(bool) through post method. 

```csharp
// This method completes an order based on the provided success status.
[HttpPost]
public IActionResult CompleteOrder(bool isSuccess)
{
    if (isSuccess)
    {
        // Place the order using the OrderService and store the resulting status.
        bool orderPlacementStatus = OrderService.PlaceOrder(order);
        
        // Update the order status based on the result of the place order operation.
        order.Status = orderPlacementStatus;
        
        // Save updated order detail
        CartService.SaveOrderDetails(order);
        
        // Return the view with the order details.
        return View(order);
    }
    
    // If the operation was not successful, redirect to a "Failed" view.
    return View("Vulnerable/Failed");
}

```

In this case attacker easy using tool to change the response. After changing the response attacker send it for server. In this case above code just checking bool value so attacker easy change the value and bypass the payment steps using wrong details.


