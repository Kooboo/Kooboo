# E-commerce - Product Types

## Page Purpose

- Admin page: `/commerce/product-types`
- Product types prepare a reusable template for new products, including product attributes, variant options, the digital-product flag, and default download limits.
- They are not content types or a continuing binding after a product is created.

## Template Contents

- Product attributes can use selection items or custom input to describe product information such as material, brand, and specifications.
- Variant options must use selection items to form sellable combinations such as colors and sizes. Actual price, SKU, inventory, and enabled status still belong to the product variant after creation.
- A digital product type can preset whether it is a digital product and its download-count and download-day limits; these values are copied to the product draft when the product is created.
- Admin settings can hide the attribute or variant area on the product-creation page. Hiding affects only the interface and does not mean data in the type or existing product data has been deleted.

## Relationship to Products

- When a type is selected to create a product, its attributes, variant options, and digital-product settings are copied to the new product draft; after saving, the product and variants own them.
- A product does not save a type relationship for later synchronization. Therefore, changing a type does not automatically update, rebuild, or migrate existing products, and deleting a type does not delete products created from it.
- Type names and template items can be changed, but those changes affect only products whose creation is started with that type afterward.

## Modification and Deletion Risks

- Before deleting a type, confirm whether the team still relies on it to create new products with a consistent structure. The current page has no “in use” protection based on existing products because products do not retain a type relationship.
- Deleting or renaming attributes and options does not repair products created in the past or merge existing variant combinations.
- A successful save proves only that the template was persisted; it does not mean the product catalog is uniform or that frontend pages will automatically display these attributes or options.
