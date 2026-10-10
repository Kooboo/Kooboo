# E-commerce - Product Management

## Page Purpose

- Admin pages: `/commerce/product-management`, `/commerce/product-management/create`, `/commerce/product-management/edit`
- Products store titles, SEO names, descriptions, images, attributes, tags, categories, and multilingual custom data. Prices, SKUs, barcodes, weights, inventory, enabled status, and digital-delivery items are stored on individual variants.
- Products and variants are two levels. An enabled product does not mean every variant is enabled, and the inventory shown in the product list is the sum of all its variant inventory.

## Creation and Product Types

- When creating a product, you can select a product type to prefill attributes, variant options, the digital-product flag, and download limits.
- A product type is only a creation template. After creation, the product is not continuously bound to that type; later changes or deletion of the type do not automatically migrate products that already exist.
- Titles, SEO names, and descriptions can be maintained per language. An SEO name must be unique among products and does not allow spaces or various special characters; it is one frontend query identifier but does not automatically create a page or public route.

## Variants, Inventory, and Availability

- Each variant can separately maintain its option combination, price, SKU, barcode, weight, images, inventory, tags, and enabled status. Changing options may add or remove the corresponding variant combinations; confirm before saving that inventory and digital-delivery items still belong to the correct combinations.
- When editing existing variant inventory, the system calculates it as “current actual inventory + new inventory entered on the page - original inventory when the page opened,” preventing order-related inventory changes during editing from being overwritten directly.
- The default frontend product list filters out disabled products and disabled variants; they are returned only when the caller explicitly asks to include unavailable data. The admin list remains available for managing those records.
- Inventory displayed for a product or variant is not an inventory reservation at checkout. The cart calculation and current order-creation flow do not automatically reject a line solely because Active is false or inventory is insufficient; business processes must still control concurrency and overselling risk.

## Categories

- The product edit page directly saves manual category relationships. Automatic categories match their conditions when read and do not generate the same manual relationships.
- Therefore, manual categories can be added or removed on the product page, but an automatic category that still matches its conditions cannot be cancelled manually; adjust the product data or automatic-category conditions instead.
- A product's category display may include both manual categories and automatic matching results, which come from different sources.

## Digital Products

- Digital-delivery items can be files, links, or text and are maintained per variant; a variant can also control whether it is delivered automatically after payment.
- Uploading a file with the same name under the same variant overwrites the actual file. Whether deleting only the file item on the page also deletes the remote file depends on the current editing scenario; do not treat a list change as the sole proof that a file has been permanently cleaned up.
- Download-count and download-day limits can be set when creating a product. Current product-edit data does not reliably round-trip and save these two limits, so do not assume a change made on the edit page has taken effect.

## Copying and Deletion

- Copying creates new product and variant identifiers, appends `--copy` to the title and SEO name, copies inventory, manual categories, and other product data, and resets sales to zero. After copying, still check whether the SEO name, SKU, digital files, and inventory fit the new product's purpose.
- Deleting a variant cleans up digital files for that variant that were not retained as order archives. Deleting a product deletes the product, variants, manual category relationships, search index, vector index, and related digital files.
- Deleting a product does not delete historical orders. Order lines save snapshots of the product, variant, price, and delivery data at order time; before deletion, still check pages, code, carts, and other dynamic references.
