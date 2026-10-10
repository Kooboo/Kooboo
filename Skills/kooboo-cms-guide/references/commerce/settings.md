# E-commerce - Settings

## Page Purpose

- Admin page: `/commerce/settings`
- This page manages the weight unit, available payment-method list, product and category fields, order extension fields, and display and validation switches on the product edit page.
- It saves e-commerce structure and admin-behavior configuration, does not automatically migrate existing data, and cannot replace the separate configuration for payment providers, shipping, taxes, or notifications.

## Weight and Payment Methods

- The current weight unit can be kg or g, used to interpret product-variant weight and the total-weight condition for physical shipping. Switching units does not automatically convert saved variant weights or shipping-rule values.
- Available payment methods save a list of payment-method identifiers. Selecting a method does not create provider credentials, callback URLs, merchant accounts, or payment pages, and does not prove that frontend checkout consumes this list correctly.
- Whether a payment method can actually collect funds still depends on the corresponding provider configuration, currency, frontend call, and complete callback flow.

## Product and Category Fields

- Products and categories both contain system fields and can add custom fields. System fields are filled in automatically when settings are read, and the current UI does not allow them to be deleted directly.
- Fields can configure control type, display name, summary column, multiple values, validation, multilingual behavior, content folder, dynamic options, and other display settings; not every control supports every option.
- Dynamic options execute saved code when options are read. Saving the code does not prove it runs successfully, and expensive or side-effectful logic must not be treated as an ordinary option query.
- Modifying, reordering, or deleting field definitions does not migrate, rename, or clean up data already saved in products and categories, and does not automatically repair templates or API consumers.

## Order Extension Fields

- Current order extension fields support string and dateTime, and can control admin editing, list summaries, filtering, and Excel export.
- string filters can use contains matching or exact option matching; dateTime filters use a start and end range. A field has a value only after the order-creation flow actually writes it.
- Export fields can run custom code to transform output. Saving or previewing code does not prove that a batch export succeeds; erroneous code can make the export fail.
- Adding, changing, or deleting a definition does not backfill or rewrite extension-field values in historical orders.

## Display and Digital-Product Validation

- Hide Attributes and Hide Variants hide only the corresponding areas for the current admin product type and product-edit interface; they do not delete existing attributes, variants, or inventory data.
- Product Digital Item Required requires at least one digital-delivery item for each digital variant only in the current admin product-edit flow. Other APIs, scripts, or import paths must still ensure data integrity themselves.
- Saving settings persists the current configuration as a whole. Before release, separately verify admin editing, frontend templates, carts, order filtering, and export; a successful save alone cannot establish that every consumer is compatible.
