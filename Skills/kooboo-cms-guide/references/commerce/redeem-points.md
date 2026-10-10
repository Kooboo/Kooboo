# E-commerce - Redeeming Points Configuration

## Page Purpose

- Admin page: `/commerce/loyalty?name=redeemPointsConfig`
- This tab sets how many points redeem one unit of the default currency and configures the rules for points that can be redeemed in an order.
- It controls cart amount calculation; it is not a payment-refund, cash-withdrawal, or points-store fulfillment system.

## Rate and Rules

- The redemption rate is the number of points that offset one unit of the default currency; it must be greater than zero for an offset to be calculated.
- Rules can match order amount, customer tags, and membership level, and calculate the redemption ceiling using fixed points or a percentage of the order amount.
- Multiple matching rules accumulate in sequence, but every step is limited by the remaining order amount and the customer's remaining points. The offset cannot exceed the order amount, and available points are never calculated below zero.

## Carts and Orders

- Cart calculation returns both the maximum redeemable amount and the actual offset after the user chooses to redeem. Showing an available offset does not mean points have been deducted.
- A negative point record is written and deducted from the balance only when the order is created; payment is not a prerequisite for deducting redeemed points.
- Cancelling an order disables the point record associated with that order, restoring the balance basis. Directly deleting an order does not reliably perform the same restoration flow.
- Changing the redemption rate or rules does not recalculate already-created orders or compensate past point usage. Recalculate a new cart after saving to verify the result.
