# E-commerce - Earning Points Configuration

## Page Purpose

- Admin page: `/commerce/loyalty?name=earnPointsConfig`
- This tab sets point expiration, login-earning rules, and order-earning rules.
- Saving rules does not issue points that were not earned in the past or recalculate historical orders that have already been paid.

## Point Expiration

- The current balance aggregates only point records within the validity period that have not been disabled. Changing the validity period affects later available-balance calculations but does not physically delete old records.
- Point expiration is determined in UTC. Old records may still appear in details, but are no longer included in the balance.

## Earning Points on Login

- A login reward is attempted only when the system identifies the login as the customer's first login that day.
- Rules can combine customer tags and the current valid membership level. The same rule generates a deduplication identifier by day, week, month, or year and is not issued repeatedly within that period.
- Configuring login rules does not create a login page or prove that every OAuth or custom login flow passes through the reward entry point.

## Earning Points on Orders

- Order rules can match order amount, customer tags, and membership level, and accumulate estimated points using fixed points or a percentage of the order amount.
- Estimated points are calculated in the cart and when the order is created, but the customer point record is actually written after the order is paid.
- Cancelling an order disables the point record associated with it; deleting an order does not reliably perform the same point-rollback flow.
- Multiple matching rules accumulate; only the first is not used. Verify the final points with a specific customer and order amount before saving.
