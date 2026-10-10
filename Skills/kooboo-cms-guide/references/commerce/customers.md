# E-commerce - Customers

## Page Purpose

- Admin page: `/commerce/customers`
- Customers store e-commerce contact information, tags, and addresses, and are associated with e-commerce data such as carts, orders, memberships, points, OAuth logins, and wishlists.
- E-commerce customers and site admin users are not the same identity system. Creating a customer does not grant CMS admin permission, and an admin user does not automatically become an e-commerce customer.

## Basic Information and Search

- The current admin creates and edits the email, phone, last name, first name, tags, and addresses; customer passwords are not set on this page.
- A non-empty email and a non-empty phone must each be unique among customers on the current site. An email or phone conflict prevents saving another customer with that value.
- The list is sorted by last-order time descending. Search uses contains matching for email, last name, and first name, and exact matching for phone. A failed search does not mean the same contact information is absent from orders or site users.

## Addresses

- Addresses are stored as part of the customer record and may include country, province, city, ZIP code, street address, recipient, and phone.
- The current admin interface tries to maintain one default address when adding an address, setting a default, or deleting the default. This is not an independent uniqueness constraint for every external write.
- Changing a customer's address does not write back to historical orders. The shipping address and customer contact information are copied when an order is created, and later customer-profile changes do not automatically modify that order snapshot.

## Memberships and Points

- The membership status and available points in customer details are calculated dynamically from current membership rules, point records, and order status; they are not ordinary directly editable fields in the current customer form.
- Changing tags or profile information may affect later membership, discount, or automation matching, but does not undo already-created orders, used discounts, or external operations.

## Deletion Boundaries

- Deleting a customer also deletes their carts, point records, membership relationships, OAuth bindings, wishlists, and customer record.
- Deleting a customer does not delete historical orders; orders retain the email, name, phone, address, and product-line snapshots copied at creation time.
- Do not interpret customer deletion as complete data erasure or business rollback. Payments, messages, Webhooks, shipments, and other external side effects that have already occurred are not automatically undone; other extension data should also be checked separately.
