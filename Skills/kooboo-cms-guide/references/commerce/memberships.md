# E-commerce - Membership Levels

## Page Purpose

- Admin page: `/commerce/loyalty?name=membership`
- Membership levels define the name, description, level priority, validity period, purchase price, automatic-upgrade conditions, and custom data.
- They define membership-eligibility rules; they are not CMS roles, access permissions, or paid subscription products.

## Acquisition and Matching

- Allow Purchase means only that the level may be purchased by a business flow and has a price; the current configuration page does not automatically create a purchase page, payment order, or renewal charge.
- When Allow Auto Upgrade is enabled, the system matches levels using conditions such as the customer, valid points, and the count of paid, non-cancelled orders.
- Automatic matching checks from highest to lowest priority and considers only candidates no lower than the customer's current level. It is therefore for upgrades and does not automatically downgrade.
- Viewing the member list or customer details may also trigger automatic matching and write a new membership record; it is not a purely read-only display.

## Validity and Manual Operations

- Membership records use UTC start and end times to determine current validity. The highest-priority valid record is the current level; when no record remains valid, the page may still show the highest-priority expired record and mark it invalid.
- A manual level change adds a membership record and disables the customer's previous record when it takes effect immediately. Renewal disables the current record and adds a renewal record using the level duration.
- Changing a level's duration, priority, or conditions does not automatically migrate existing membership records or recalculate past start and end times.

## Deletion Boundaries

- Deleting a membership level does not delete customers, orders, or points, and does not automatically repair level references in existing membership history.
- The real money flows for purchase, payment, refunds, and renewal must be completed by site business logic and payment integration. Allow Purchase or a price field cannot be taken as proof that a subscription loop exists.
