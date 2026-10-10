# E-commerce - Taxes

## Page Purpose

- Admin pages: `/commerce/taxes`, `/commerce/taxes/create`, `/commerce/taxes/edit`
- Taxes define a base rate by country and can add regional and product-condition overrides.
- Each country has one tax configuration. It is a cart amount-calculation rule, not a tax-registration, invoicing, filing, or payment-provider tax service.

## Scope and Order

- The system looks up a country's tax configuration only when the cart provides an identifiable country; the tax is not applied when no configuration exists or the country cannot be identified.
- Tax is calculated per product line, based on the line amount after discounts. Current tax does not separately tax shipping fees, insurance fees, or points offsets.
- The region name is matched against the country's province/state or city resources; if no region matches, only the country's base rate is used.
- Regional overrides are applied first, followed by product overrides. Product overrides use only the first matching rule in configuration order.

## Three Override Modes

- Added: adds the specified rate based on the product-line amount to the current tax.
- Instead: replaces the previously calculated base and regional tax with the specified rate.
- Compounded: calculates the specified rate on the product-line amount plus current tax, then adds it.
- Product overrides can match current product and variant conditions. Configuration order affects the first matching result; verify it with a specific product, region, and discounted amount.

## Modification and Deletion Boundaries

- Creating or saving a tax rate proves only that the rule was persisted; it does not verify that the rate is lawful, the address is real, or a tax obligation exists.
- Changing or deleting a tax does not recalculate already-created orders; orders retain the product-line and order tax amounts calculated at creation.
- Deleting a country's configuration affects only later calculations. It does not issue refunds, rewrite historical orders, issue invoices retroactively, or undo external financial records.
