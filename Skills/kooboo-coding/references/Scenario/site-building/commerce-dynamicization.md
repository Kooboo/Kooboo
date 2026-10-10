# Commerce Site Dynamicization Contract

This document governs public storefront dynamicization for `commerceModel = catalog-contact | online-checkout`. It supplements Site Building S7–S10, does not create a second Storefront Blueprint, and does not replace the Page Plan.

## 1. Data-Source Ownership

Commerce is the sole default source of truth for commerce business entities:

| Entity | Default data source |
| --- | --- |
| product / category / variant | `commerce` |
| cart / customer / order | `commerce` |
| discount / shipping / membership / loyalty | `commerce` |
| Blog / FAQ / Lookbook / brand story | `content` |
| Site-specific extension data without a Commerce model | `sqlite` or another confirmed data source |

Do not copy Product, Category, Variant, Cart, or Order into Content merely to make them maintainable in the back office. Only when the user explicitly requests a display catalog independent of Commerce may that catalog be built as Content, and it must be stated that it does not participate in the Commerce inventory, variant, cart, or order flow.

After the data source is confirmed:

1. Call `function_discover` with `tags` containing `Commerce`.
2. Before calling any Commerce function, run `use_function` to read its real InputSchema / ReturnSchema.
3. Prefer `<k-data>` + `<query source="commerce">` for SSR product/category reads.
4. When a built-in Commerce write function exists, do not reimplement the same write with `run_kscript`.

## 2. Storefront Data Quality

Seed data that AI creates for the storefront must meet displayable and routable standards.

Minimum Product fields:

- `title`
- `seoName`
- `featuredImage`
- `price`
- `inventory`
- `active`
- `categoryIds` (use category IDs returned by `create_product_category`; pass `[]` only when an uncategorized product is explicitly needed)

Minimum Category fields:

- `title`
- `seoName`
- `image`
- `active`

The image requirement here applies only to storefront data created by AI and does not change the general optional fields available to other products/categories in the Commerce runtime. Use real in-site media URLs or ImagePlaceholder; do not invent external links.

After creation, call a detail function to read back key fields and confirm that `product.categories` contains the expected category and that the category detail/backend count is no longer 0. `Save success`, a successful function call, or the existence of a write statement in source code cannot alone prove data completeness.

## 3. Product Prices and Variants

- `product.price` is the minimum price among the current product's variants and is suitable for list and “starting at” displays.
- `product.price` is not guaranteed to equal `product.variants[0].price`.
- PDP displays the selected `variant.price` after the user chooses a specific variant.
- Add-to-cart, wishlist, and exact inventory checks use the selected `variant.id`.
- Example code may use `variants[0].id` only after confirming that the product has exactly one default variant; multi-variant products must first match options to a variant.

## 4. Product Detail URL

An independently addressable Product Detail Page should use a path parameter for the product identity. Prefer `/product/{seoName}` and link to it with `/product/{item.seoName}`. The PDP reads `seoName` from the route context and passes it to the Commerce `item` query. Keep query strings for list state or other non-identity values; use a query-string product identity only when an existing public route must be preserved.

## 5. Products Category Filtering

The category selector must use the same `selectedCategory` fact value to drive both the query and the active UI.

- The category list shows only `active = true`.
- The product list shows only `active = true`.
- `All` means that there is no category condition; do not treat `all` as a category ID or seoName.
- Reset pagination to the first page when switching categories.
- Derive the active style by comparing the current queryString with the category `seoName`; do not hard-code it on the first button.
- Active state may be bound through a server-side dynamic class or synchronized by frontend code that reads `URLSearchParams` and updates `class` / `aria-current`; both approaches must use the current category parameter as the fact value.
- Support All, populated categories, empty categories, refresh, back/forward navigation, and no-result states.

Recommended k-data query structure:

```html
<k-data>
  <let as="selected_category" source="queryString" from="category" default="''" export></let>
  <let as="page_number" source="queryString" from="page" default="1"></let>
  <query as="categories" source="commerce" resource="category" action="list" export
    where="{active:{$eq:true}}"></query>
  <when test="{selected_category:{$ne:''}}">
    <query as="product_paged" source="commerce" resource="product" action="paged" export
      where="{category:selected_category,active:{$eq:true}}"
      page-number="page_number" page-size="12"></query>
    <else>
      <query as="product_paged" source="commerce" resource="product" action="paged" export
        where="{active:{$eq:true}}"
        page-number="page_number" page-size="12"></query>
    </else>
  </when>
</k-data>
```

The category selector and product grid may be in two Views, but they must be accepted as one Products-page capability:

- Keep only one category selector and one authoritative category query on the page;
- Both Views read the current queryString from the same-named `selected_category`; neither maintains a second independent frontend state;
- Category links omit the `page` parameter and therefore naturally reset to the first page;
- Preflight must receive the related Views referenced by the Page together; do not scan them separately and then require every View to contain both the category and product query.

### Products Runtime Acceptance

Execute in order; observing only the initial page is not allowed:

1. Open `/products`, confirm that All is active and only active products are shown;
2. Click an active category containing products and confirm that the URL, active style, and result set agree;
3. Click an empty category and confirm that a real empty state appears instead of falling back to all products;
4. Click All again and confirm that the URL does not contain `category=all`;
5. Refresh and use browser back/forward, confirming that the active state and results remain consistent;
6. Check that the DOM contains no duplicate category selector.

## 6. In-Place Dynamicization

Dynamicization normally modifies existing Views rather than generating a generic Commerce template again.

The following must be preserved:

- the selected layout archetype / desktop topology;
- the main DOM sections and information order;
- visual hierarchy, responsive behavior, and accessibility;
- existing design differences among task pages such as Cart, Checkout, and Account.

Necessary loading, empty, error, disabled, authenticated, and cart-count areas may be added for real states, but do not replace the entire original visual structure with an undifferentiated table or simple cards.

## 7. Cross-Page Capability Contracts

### catalog-contact

Complete Product/Category lists, details, and a contact/quote entry point. Unless explicitly requested by the user, do not add login, cart, orders, or payment.

### online-checkout

Complete the following in the same Commerce context:

| Surface | State it must consume |
| --- | --- |
| Products | Categories, search, sorting, pagination, product results |
| PDP | Media, variants, price, quantity, inventory, add-to-cart |
| Header | Customer state, login/register or account/orders/logout, cart quantity |
| Cart | Line-item increase/decrease/delete, empty state, amounts, discount/shipping state |
| Checkout | Contact, address, shipping, payment method, order confirmation |
| Order Result | Success, processing, failure, cancellation, and recovery actions |
| Account / Orders | Current customer, order list, and order details |

Login or add-to-cart cannot update only the current local component: Header must update immediately and restore from the server-side `kb_commerce` context after refresh/navigation.

Login implementation defaults to `k.commerce.customer.login/currentId/logout`. Do not replace `kb_commerce` with `commerce_token + localStorage`; the complete four-endpoint flow and acceptance path are in `../commerce-customer-login-flow.md`.

The Account address book, Checkout shipping address, and order address share Commerce `Address` field semantics. When any address form is involved, read [Commerce Customer and Shipping Address Contract](../commerce-customer-address-flow.md); different pages must not independently trim and override the same address object.

### Header Authentication State

At the current stage, Header / Account uses shared `/api/auth/me` (or an equivalent auth-status endpoint) to restore authentication state; do not introduce `env="server"` for Header.

- Both guest and authenticated sides must initially be `hidden`; after `fetch` returns, show exactly one side according to `authenticated`.
- A dynamically shown/hidden node must not use `hidden` together with responsive display classes such as `md:flex` / `lg:flex` / `sm:block`. Put breakpoint control on the outer layer and `flex/grid` layout on the inner layer; Desktop and Mobile auth nodes share one state-update function.
- Handle both sides on every switch; never show only the account entry while leaving login/register visible.
- Refresh the same auth-status after login/register/logout; do not replace the entire Header with `innerHTML` (especially do not replace the cart node along with it).

### PDP / Cart Client Boundary

- SSR/`k-data` reads the product and determines the default variant;
- Render client-needed values such as `productId` and the default `variantId` into `data-*` or safe JSON;
- Client scripts read only the DOM and do not directly reference server-template variables such as `product_detail.variants[0].id`;
- When there is no variant radio/select, use the default `data-variant-id` on the button; do not query a nonexistent control and submit an empty value;
- The add-to-cart button must restore its loading/disabled state in `try/finally`;
- A successful add-to-cart uniformly dispatches `kooboo:cart-changed`; Header listens for that event and reads authoritative cart detail again;
- Keep Header cart and auth nodes separate; refreshing auth must not replace the cart DOM with `innerHTML`.

### Minimum Product Contract for Account / Orders Pages

Account:

- current-customer identity summary, account navigation, logout entry, and recent orders;
- guest, loading, authenticated, empty, and error states;
- do not reduce the entire page to a generic welcome card.

Orders:

- date, order number, product summary, total amount, payment/fulfillment status, and a view-details action;
- loading, empty, error, pending, paid, and canceled states;
- use scannable order blocks on mobile instead of forcing a wide desktop table into compression.

Order Detail:

- product lines, quantity, unit price, subtotal, discount, shipping, tax, and total;
- shipping address, payment status, delivery status, and valid recovery actions;
- entry points for reading details, continuing payment, canceling, and similar actions must validate `order.customer.id === k.commerce.customer.currentId`;
- Order Result expresses only the result of this checkout and does not replace the long-lived Order Detail.

The three pages must reuse the current site's fonts, colors, spacing, buttons, and image proportions; do not cover different sites with the same undifferentiated card template.

### Checkout / Payment / Order Result

For the complete steps, cart-ownership validation, server-rendered payment form, read-only return status, and Account order entry, see [Commerce Checkout and Payment Golden Path](../commerce-checkout-payment-flow.md).

Hard-constraint summary:

- Before placing an order, both `customer.currentId` and `cart.currentId` exist, and `cart.customerId === currentId`;
- After creation, confirm `order.customer.id === currentId`, then redirect to `/api/payment/alipay?orderId=...`;
- The payment endpoint calls `charge({ order })` server-side and writes directly with `k.response.write(renderHtml)`; `<script env="server">` in a View and client-side innerHTML injection are forbidden;
- `returnUrl` is not evidence of successful payment; Account must be able to list and open the customer's order.

## 8. Configuration Entry Points

- Payment-channel secrets: service integration `Payment`.
- Commerce payment switches/field model: Commerce Settings.
- Currency: Commerce Currencies.

When the user must open the back office to finish configuration, do not guess a back-office path or hand-write a SiteId. If the current toolchain has no dedicated and verified payment-configuration entry point, clearly tell the user to complete payment-integration configuration in the back office and keep the item as incomplete evidence; do not use the generic `show_action_button` to disguise an untrusted configuration entry.

## 9. Completion and Evidence

State completion separately:

- `modified`: the object or data has been written.
- `structurally verified`: Preflight passed after the final write.
- `runtime verified`: the original-path interaction was verified in Preview/Browser.
- `business verified`: real business results such as orders, payment callbacks, or back-office results were confirmed.

Products All/category filtering, the post-login Header, the post-add-to-cart Header count, Cart increase/decrease/delete, and the Checkout configuration entry all require same-path runtime evidence. When Preview is unavailable, explicitly mark them unverified; do not treat a Preflight PASS as runtime completion.

The `completionGate` returned by Preflight is the machine-readable upper bound:

- When `claimAllowed=false`, the final response must not say “completed” or “running normally”;
- `verifiedArtifacts[].contentHash` proves structural checks only for the corresponding version;
- Any later write invalidates the old hash evidence;
- When Commerce `missingEvidence` is incomplete, the highest possible statement is `structurally verified`.

Run Commerce Preflight by delivery stage:

- Static first version: `deliveryStage: "static-first"`; dynamic rules remain `deferred`.
- Product/customer/cart/checkout dynamicization: `deliveryStage: "commerce-runtime"`; complete same-path acceptance for Products, PDP, login/Header, Cart, Checkout, and order ownership.
- Payment integration: `deliveryStage: "payment-runtime"`; complete evidence for PaymentRequest, provider callback/checkStatus, and paid-order state in the frontend and back office.

Do not create BLOCKERs from future dynamicization rules during the static stage, and do not treat a payment return redirect as completed payment acceptance during `commerce-runtime`.
