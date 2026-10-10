# E-commerce - Carts and Admin Checkout

## Page Purpose

- Admin pages: `/commerce/carts`, `/commerce/cart/create`, `/commerce/cart/edit`, `/commerce/cart/checkout`
- A cart is a transaction draft that has not yet become an order. It stores the customer, product-variant lines, quantities, shipping method, digital shipping method, discount code, notes, and whether points should be redeemed.
- Creating or editing a cart in the admin requires selecting an existing customer. A cart-line quantity below 1 is saved as 1.

## Listing and Saving

- The list shows only carts with contact information or a customer identifier; it is not a complete list of all anonymous sessions and temporary frontend state.
- Saving a cart only persists the draft. It does not create an order, reduce inventory, consume a discount code, deduct points, make a payment, or ship goods.
- Deleting a cart deletes only the draft. It does not delete an already-created order or undo payments, notifications, or other external operations that have already occurred.

## Calculation Preview

- The page calculates amounts in real time from the current product variant, quantity, customer, membership status, discounts, shipping, tax, points, and currency exchange rate. This is a preview; it does not persist an order or reduce inventory.
- Lines whose product or variant cannot be found are skipped from the calculation, so the original cart-line count may not equal the number of valid calculated lines.
- The current calculation does not automatically reject a line solely because its product or variant is disabled or inventory is insufficient. Inventory shown on the page also does not mean inventory has been reserved for the cart.
- Discount validity periods, usage counts, priorities, and exclusivity rules affect the result. After the cart or its configuration changes, an amount previously shown must not be treated as the final order amount.

## Admin Checkout

- Checkout does not save the cart again; it creates an order from the current cart. Prices, discounts, shipping, tax, points, and exchange rates are recalculated during creation rather than trusting the amount previously shown on the page.
- On success, an unpaid order is saved, and the product, variant, price, options, digital-delivery items, and other data are copied into order-line snapshots.
- When the order is created, variant inventory is immediately reduced, discount usage is recorded, the points redeemed for this transaction are deducted, and the customer's last-order time is updated. The cart's product lines, discount code, notes, shipping method, digital shipping method, and redeem-points flag are then cleared, while the cart record is retained.
- The current flow does not use sufficient inventory as a universal blocking condition, and the entire creation process must not be assumed to be one automatically rollbackable atomic transaction. Concurrent, inventory-sensitive business needs additional validation and compensation strategies.

## Checkout Does Not Mean Payment Completed

- A successful checkout button action means only that order creation completed; it does not mean payment, shipping, or digital delivery completed.
- Payment is a separate action. Product sales counts, earned points, and digital products configured for automatic delivery are processed after the order is paid, not when the order is created.
- Successful admin checkout alone cannot prove that order-creation notifications, Webhooks, or other external systems completed successfully. Check the order status and the corresponding integration results separately.
