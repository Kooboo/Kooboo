# E-commerce - Discounts

## Page Purpose

- Admin pages: `/commerce/discounts`, `/commerce/discounts/create`, `/commerce/discounts/edit`
- Discounts can apply to product amounts, order amounts, or free shipping, and can use automatic-discount or discount-code mode.
- Conditions, validity times, usage counts, priority, and exclusivity together determine which discounts are ultimately applied in one cart calculation.

## Modes, Types, and Conditions

- An automatic discount becomes a candidate automatically during calculation; a discount code becomes a candidate only when the cart submits the corresponding code.
- Product-amount discounts match order-line conditions; order-amount discounts and free shipping match order conditions. Switching the discount type clears the current conditions because the available condition scope differs.
- Conditions can require any item or all items to match. Amount discounts can use a fixed amount or percentage; free shipping does not use an amount value.
- A discount becomes a candidate only when the current UTC time is strictly later than the start time and strictly earlier than the end time. It is not effective exactly at either boundary.

## Priority, Stacking, and Exclusivity

- Candidate discounts are processed from highest to lowest priority. Ordinary discounts can continue to stack.
- If an exclusive discount matches successfully while no other discount allocation exists, processing of later discounts stops. If an allocation already exists, the exclusive discount is skipped.
- Therefore, “exclusive” does not mean it unconditionally overrides every other discount; the result depends on priority and earlier matches.

## Usage Counts

- Discount-code limits are counted from discount-usage records. A usage record is written when an order is created; the order does not need to be paid.
- Cancelling an order deletes its discount-usage record and releases the count. Directly deleting the order or discount also deletes the corresponding usage records.
- The list's usage count and related orders come from current usage records. Deleting a discount does not rewrite discount allocations saved in historical orders or recover external business results that have already occurred.

## Modification and Verification

- Changing a discount does not recalculate already-created orders. A cart reapplies the current rules during later calculation and checkout.
- A successful save proves only that the rule was persisted; it does not prove that a particular product, customer, or order satisfies the conditions. Verify with a recalculation of a specific cart.
- Deleting a discount prevents later calculations from using it and cleans up usage records; it does not undo paid orders, refunds, messages, or other external side effects.
