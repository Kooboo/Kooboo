# E-commerce - Notifications

## Page Purpose

- Admin page: `/commerce/notification`
- This page separately configures email and Webhook notifications for e-commerce events and shows their sending logs.
- Email and Webhooks are asynchronous external notifications after an event is triggered; they are not authoritative sources for order status, payment status, or downstream business success.

## Email Notifications

- The email master switch must be enabled and the event must have a corresponding template before email is sent. Current email events include order payment, order cancellation, and order shipment.
- Each template can send to the customer's address in the event and to additional addresses; each recipient address produces an independent send and log result.
- You can use a Kooboo email address or custom SMTP. Saving the server and template does not test a real connection, quota, account permissions, or recipient reachability.
- Preview renders the subject and HTML with simulated event data. It does not send email or prove that real event data always satisfies the template code.

## Webhooks

- The Webhook master switch must be enabled and the event must have a corresponding URL configuration before it is sent. Besides order payment, cancellation, and shipment, Webhook events also include customer- and cart-related events.
- Requests use POST and JSON content. The signature is the lowercase hexadecimal SHA-256 value of the request body concatenated with the site secret, placed in the `X-Kooboo-Hmac-SHA256` request header.
- Webhook requests time out after 15 seconds. Only a success status code from the remote side is recorded as a successful send; this still does not prove that the remote side completed or persisted its business processing.
- The secret is for signature verification, not URL access control. After changing the secret, the receiver must update its verification logic as well.

## Logs and Retries

- Email and Webhook logs are paginated separately and save the address, event, content, time, send status, error count, and error message. They are not a unified audit record for every order event.
- After the first send fails, it enters an in-memory retry queue after a short delay. If a queued retry fails again, the next retry is delayed in hours based on the current error count: error count N means a delay of N hours, not seconds or minutes.
- Retries stop after one day or when the error-count limit is reached. A service restart, disabled task service, or site release may lose in-memory retries that have not yet run.
- Insufficient email quota does not enter the ordinary retry flow. Deleting or disabling a notification configuration does not retract requests already sent or automatically delete historical logs.
