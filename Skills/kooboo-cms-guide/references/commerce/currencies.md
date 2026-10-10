# E-commerce - Currencies

## Page Purpose

- Admin page: `/commerce/currencies`
- This page sets the site's default currency and adds additional currencies and exchange rates that carts and orders can select.
- It is not the settlement-currency configuration for a payment provider and does not open collection accounts, perform foreign-exchange conversion, or reconcile finances for you.

## Default and Additional Currencies

- The default currency is the base for site amounts. Additional currencies store a manual exchange rate relative to the default currency or enable automatic updates.
- Changing the default currency clears all additional currencies because the exchange-rate basis has changed; add and verify each currency again.
- Deleting an additional currency only prevents later pages from selecting it normally. It does not rewrite saved order amounts, currency codes, or payment records.

## Manual and Automatic Exchange Rates

- Manual mode uses the rate saved on the page. Saving a rate does not verify that a payment provider accepts the currency or lock historical statistics beyond future orders.
- Automatic mode obtains data from a remote exchange-rate service. Rates use a local cache; after the cache exceeds three days, the existing value is returned first and refreshed in the background. The latest market price is not fetched synchronously every time the page opens.
- If the remote service fails, the rate is invalid, or an unenabled additional currency is requested, business calculations may fall back to the default currency and a 1x rate. Currencies listed on the page do not prove that the external exchange-rate service is always available.

## Effects on Orders and Statistics

- When an order is created, its amount is saved in the selected currency using the rate obtained at that time, together with the currency code. The order itself does not save a separate exchange-rate record for sales statistics to reuse.
- When sales statistics convert historical orders back to the default currency, they use the current currency settings or the currently available rate. Later rate changes can therefore change the normalized amounts in historical statistics.
- Before changing the default currency, additional currencies, or rates, separately verify cart display, order creation, payment-provider support, refunds, and reports. Do not treat a successful admin save alone as a release conclusion.
