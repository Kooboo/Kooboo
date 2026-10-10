# Service Integrations - Payment

## Page Purpose

- The Payment group stores account keys and mode configuration for payment providers such as Stripe, PayPal, WeChat Pay, Alipay, 2Checkout, Square, Pay.nl, and MoneyBoxs.
- This page configures provider credentials, not payment-method switches on the e-commerce checkout page. Allowed checkout methods, order creation, and how the page calls payment must be completed separately in e-commerce settings and business code.

## Configuration and Provider Differences

- Different payment methods use different configuration items. For example, Stripe Checkout and Stripe Payment Intent are independent configurations, and PayPal Checkout and PayPal Form are not the same entry point.
- Sandbox or test mode affects only providers that support the option; do not assume every payment method has a unified test switch. Test and production credentials must not be mixed.
- WeChat Pay requires AppId, merchant number, API v3 key, certificate, and private-key files. Alipay uses fixed RSA2 and UTF-8 and selects the gateway from the sandbox switch. A successful field save does not prove certificates match, signatures are correct, or merchant permissions are enabled.
- Configuration files uploaded on the page have a small frontend size limit and suit certificate or key text; this is not a general attachment-storage entry.

## Payment Results and Callbacks

- Starting payment creates a payment-request record and calls the selected provider. Some methods return immediately; others require a redirect, QR code, client action, or asynchronous notification.
- A provider accepting a request does not mean the order is paid; a page returning to a success address cannot replace signature verification, status queries, or callback handling.
- The system enters the payment-callback flow only after confirming Paid status. Failure, timeout, duplicate notifications, and a user closing the page must be handled correctly by the business flow. Saving keys does not automatically configure the Webhook or callback URL required in the provider admin.
- Payment methods may also restrict supported currencies. Correct credentials can still fail when currency, amount, country, callback, or order status does not meet requirements.

## Risks and Verification

- Changing or clearing keys affects only later provider calls; it does not refund, cancel in-flight payments, or change existing order status.
- Before release, use the provider sandbox to complete request creation, next action, successful callback, failed callback, duplicate callback, and status-query tests, then verify with a small production transaction. A successful admin configuration list is not evidence of a complete payment loop.
