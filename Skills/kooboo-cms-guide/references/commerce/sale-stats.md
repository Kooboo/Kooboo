# E-commerce - Sales Statistics

## Page Purpose

- Admin page: `/commerce/sale-stats`
- Sales statistics generate product, order-detail, and daily views from order data, and support exporting the current filter range to Excel.
- They are operational statistics, not payment-provider reconciliation, tax reports, inventory ledgers, or a general ledger.

## Statistical Scope

- Only paid and non-cancelled orders are counted. Unpaid and cancelled orders do not enter amount, order-count, or product-sales statistics.
- Date filtering uses order creation time, not payment, shipment, or refund time; the end date includes the selected calendar day.
- Daily statistics are also grouped by order creation date. The page's date basis must not be equated directly with a payment provider's settlement date.

## Three Views

- Product statistics aggregate quantity, amount, and involved-order count by variant identifier, in descending amount order. If a product or variant has been deleted, the title and SKU saved on the order line are used when possible, but current variant options may not be recoverable for display.
- Order statistics contain one row per order line with the order number, product, SKU, quantity, amount, payment method, address, and notes. The total order count is deduplicated by order number.
- Daily statistics summarize each day's order count, product quantity, amount, and distinct-customer count. Customer count comes from customer identifiers in orders and is not the same as unique visitors or payer accounts.

## Multiple Currencies and Export

- Multi-currency orders are converted back to the site's default currency before counting. Conversion uses the current site currency settings or currently available rate, not a historical rate locked when the order was created.
- Changing a manual rate, enabling automatic rates, or changing the default currency can change statistics generated later for the same historical orders.
- Excel export uses the current date filter and exports the complete result, not only the current page. The exported file is a snapshot generated at that time and does not update automatically when order status or rates change.
- Statistics should be reconciled separately with the payment provider, refunds, chargebacks, and offline fund records; this page alone cannot establish that financial reconciliation is complete.
