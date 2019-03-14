To implement a new payment method. 

1. Implement the interface IPaymentMethod. 

2. If there is a need for call back url. 
- Create a method that return PaymentCallBack. 
- Use Manager.GetCallbackUrl to generate the url, assign and send them to payment provider. 