# E-commerce - Membership and Point Details

## Page Purpose

- Admin page: `/commerce/loyalty?name=member`
- This tab shows current membership status and available points from e-commerce customers, and provides entries for membership history, point records, manual level changes, renewals, point additions, and point deductions.
- It is not an independent customer directory; member counts come from e-commerce customer data.

## Current Status

- Opening the list or details recalculates points and may update membership status under automatic-upgrade rules, so reading the page is not necessarily a purely read-only operation.
- The current membership is selected by validity period and level priority. Manual changes or renewals add history records rather than directly overwriting one unique row.
- Current points are the sum of valid point records; expired or disabled records are excluded, and a result below zero is displayed as zero.

## Manual Points

- A manual point addition must be positive. A manual deduction writes a negative point record and rejects the operation if the balance would be insufficient afterward.
- Manual changes do not rewrite order amounts or refunds, and do not automatically revoke products or external benefits that have already been redeemed.
- Point details show historical records, including records later invalidated by expiration or order cancellation. The invalid flag in the list and the current balance are different concepts.

## Search and Deletion Boundaries

- Member search uses the customer email, phone, last-name, and first-name matching rules; a failed search does not mean the order or admin user does not exist.
- Deleting a customer deletes its membership and point records. Deleting a membership level does not automatically delete customers or fully clean up historical references to it.
