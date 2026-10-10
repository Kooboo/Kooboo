# E-commerce - Orders

## Page Purpose

- Admin pages: `/commerce/orders`, `/commerce/order-detail`
- Orders save the amount, currency, customer contact information, address, product lines, discounts, points, and shipping data at order time, and separately record payment, shipment, and cancellation status.
- Creation, payment, shipment, cancellation, and deletion are different actions. Success in one state cannot replace the others or external business results.

## List and Details

- The list can query by payment status, cancellation status, shipping status, creation date, order number, phone, email, and filterable extension fields.
- The paid-order count and amount at the top include only paid, non-cancelled orders. Pending payment, pending shipment, and shipped orders are counted by their respective statuses.
- Order lines save price, quantity, SKU, and delivery data at order time. While the product and variant still exist, the list or details may supplement the display with the current title, options, and images, so these displayed values may not all be the versions at order time.
- Changing customer information, a product, or a variant does not recalculate the order amount or automatically modify contact information, address, price, or inventory results already saved in the order.

## Admin Payment

- The current admin payment dialog offers Cash; the credit-card option is not available. This action marks the order as paid in the admin; it is not a credit-card acquiring or payment-provider flow.
- Marking payment writes the payment time and method, increases product sales, grants the points earned for the order, and attempts delivery for digital order lines configured for automatic delivery.
- A successful payment-button action does not mean real funds have arrived or prove that digital email, Webhooks, or other external notifications succeeded. Real online payment should be confirmed through the payment provider and a complete callback flow.

## Shipment and Digital Delivery

- The current details page shows the shipment entry only for paid, non-cancelled orders; all unshipped lines or a subset can be selected.
- For physical products, a carrier and tracking number can be entered. Digital products use the digital-delivery items saved on the order line and are handled by the selected digital shipping method.
- A partial shipment marks order lines separately and puts the order into a partially shipped state; it becomes fully shipped only when all lines are complete.
- Digital-delivery failures are recorded as errors. Clicking shipment or receiving a successful page request must not be interpreted directly as proof that the recipient received the content.

## Cancellation and Deletion

- Cancelling an order records the cancellation reason and time, restores order-line inventory, releases discount-usage records, and disables point records associated with the order; a paid order also has its sales count reversed.
- Cancellation does not automatically initiate a refund with the payment provider and cannot retract sent email, Webhooks, physical packages, or downloaded digital content.
- Deleting an order is different from cancelling it. Deletion directly removes the order and its discount-usage records and does not perform the inventory, sales, points, or discount-eligibility restoration in the cancellation flow, nor undo external side effects.
- Therefore, normally cancel or refund through the business flow first, then decide whether the record should be deleted. Do not treat deletion as financial or fulfillment rollback.
