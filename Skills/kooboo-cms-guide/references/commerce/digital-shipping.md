# E-commerce - Digital Shipping

## Page Purpose

- Admin pages: `/commerce/shippings?name=digital`, `/commerce/digital-shippings/create`, `/commerce/digital-shippings/edit`
- A digital shipping method configures the sending service, sender address, and email template containing digital-delivery content.
- It is separate from physical shipping-rate configuration, does not calculate logistics costs, and does not mean that a digital product has been paid for or delivered.

## Email and Preview

- You can use a Kooboo email address or custom SMTP. The Kooboo option requires a valid sender address and available sending quota; the custom option requires the server, port, SSL, account, and password to connect successfully.
- The template subject and HTML body support runtime data rendering. Preview generates a preview from a simulated order; it does not send email and cannot verify SMTP, recipient addresses, or real order data.
- After a default digital shipping method is set, an order with no specified digital shipping method can fall back to the default. Digital delivery fails when no usable configuration exists.

## Delivery and Downloads

- Digital delivery generates an order-specific download entry for files and sends a delivery email according to the configuration. Files, external links, and text content appear in the download entry as a file download, a 302 redirect, and a text response, respectively.
- The download entry requires the order to be paid and not cancelled. Files can also be limited by the maximum download count per product and the number of valid days after shipping; a file is not returned after the limit, after expiration, or when it no longer exists.
- The total file-download limit is calculated per order line: maximum downloads per unit × quantity purchased for that order line. For example, with a per-unit limit of 3 and a quantity of 4, each digital file in that order line can be downloaded at most 12 times; it is not limited to 3 downloads for the whole line.
- A successful email request is not the same as the recipient actually receiving, opening, or completing a download. Kooboo also does not guarantee the availability of an external link.

## Modification and Deletion Boundaries

- A successful save or preview does not prove that real email can be sent. Before release, verify the complete delivery flow with a real sending service, a test order, and a download entry.
- Changing the template, SMTP, or default item does not resend historical emails or change digital-delivery content and limits already saved to order lines.
- The current list does not allow the default digital shipping method to be selected directly for bulk deletion. Deleting another configuration does not repair references in products, carts, or historical orders, and does not retract sent emails or downloaded content.
