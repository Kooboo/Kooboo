# Site-Building Capability Catalog

Use this document to generate site-level milestones after S2 and to filter next-step recommendations after a stage is complete. It is not an enumeration of site types; AI must trim it according to the Site Brief, Page Plan, the user's recent intent, and completed capabilities.

## When to Use

- **After S2 stop conditions pass**: read this document and generate the first round of `update_site_milestones` for the current site. The first-round milestones contain only the site's main path and capabilities the user explicitly requested.
- **After a stage completes**: read or recall this document, choose 1–3 natural next-step recommendations, and call `show_next_recommended_action`.
- **After the user accepts a recommendation**: add or activate the accepted capability as a site-level milestone and immediately call `update_todos` for that milestone to generate current-stage tasks.

## Filtering Order

```text
当前阶段
  -> Site Brief
  -> Page Plan / 页面模块
  -> 已完成 milestones / 已完成能力
  -> 用户最近表达
  -> capability.appliesWhen / notWhen
  -> 建立当前站点的能力依赖关系
  -> 输出主干 milestones 或当前依赖前沿
```

### Output Method

1. **Select capabilities**: retain only capabilities required by the user's final goal, explicitly requested by the user, or supported by direct structural evidence in the Page Plan.
2. **Establish dependencies**: arrange prerequisites such as data, identity, transactions, and notifications into a directed main path; do not disguise one chain as a set of options that can start in parallel.
3. **Generate milestones**: normally output 2–4 user-perceivable results. Name them with business objects from the Site Brief and user paths from the Page Plan instead of copying the internal capability titles or implementation checkpoints below.
4. **Generate recommendations**: find incomplete milestones whose dependencies are satisfied first and normally recommend only the current frontier; show 2–3 items only when multiple capabilities are independent and each has direct evidence.
5. **Optional enhancements**: consider forms, SEO, email, membership, and similar enhancements only after the established main path is complete; do not recommend them merely to fill a quota when evidence is missing.
6. **Deduplicate**: when two candidates affect the same result or one is a prerequisite/substep of the other, retain only the currently executable item closest to the user.

Implementation checkpoints in capability entries are for judging scope and dependencies only; they are not milestones to copy directly. Tool calls, document reads, preview refreshes, the Design System, and Preflight belong to current-stage todos or internal execution and do not enter the site roadmap.

## Core Capabilities

### static_delivery

- Title: Static first-version delivery
- Applies to: all new sites and whole-site static redesigns.
- Does not apply to: fixing one object, backend API-only work, or answering an inquiry only.
- Implementation checkpoints:
  - Confirm first-version scope and page structure
  - Deliver a previewable static first version
  - Complete static first-version acceptance

### content_dynamic

- Title: Dynamic content
- Applies to:
  - The site contains maintainable content such as blogs, news, cases, events, menus, product catalogs, teams, or FAQs.
  - The user wants to maintain content in the back office later.
  - The Page Plan contains content structures such as lists/details/categories/filters/pagination.
- Does not apply to:
  - The user explicitly wants a one-off static page.
  - The page has very little content and does not need maintenance.
- Prerequisites:
  - The static first version is previewable, or the user explicitly requests direct dynamicization.
  - The S7 data-source decision is complete.
- Implementation checkpoints:
  - Decide the content data source
  - Confirm the content-structure draft
  - Connect SSR dynamic content
  - Complete dynamic-content acceptance

### lead_form

- Title: Form lead capture
- Applies to:
  - The site has conversion intent such as contact, booking, quotation, trial, registration, inquiry, or subscription.
  - The Page Plan contains Contact, CTA, Booking, Inquiry, Newsletter, or similar modules.
- Does not apply to:
  - The user explicitly does not want to collect information.
  - The site is purely presentational with no contact or conversion goal.
- Prerequisites:
  - Form fields, post-submit feedback, and notification method are clear.
- Implementation checkpoints:
  - Confirm form fields
  - Create a submission API or data store
  - Connect frontend form state
  - Complete form-flow acceptance

### commerce_catalog

- Title: Product catalog and inquiry
- Applies to:
  - The site displays products, service packages, menus, courses, room types, SKUs, or price ranges.
  - `commerceModel = catalog-contact`, or the user only needs display and purchase contact.
- Does not apply to:
  - The user explicitly requests online ordering and payment; consider `online_checkout`.
  - The site has no product/package/service catalog.
- Prerequisites:
  - Product/category/price/image strategy is clear.
  - Product, Category, and Variant use Commerce by default; if Content is used instead, confirm that it is an independent display catalog and does not participate in the Commerce transaction flow.
- Implementation checkpoints:
  - Confirm the product-catalog structure
  - Connect dynamic product/category data
  - Connect inquiry or contact entry
  - Complete product-catalog acceptance

### online_checkout

- Title: Online ordering and payment
- Applies to:
  - The user explicitly requests online purchase, ordering, cart, subscription, billing, or payment.
  - `commerceModel = online-checkout`.
- Does not apply to:
  - Product display or purchase contact only.
  - The user has not confirmed the payment method, currency, or order flow.
- Prerequisites:
  - Product or package data is clear.
  - Payment method, currency, and order-success/failure paths are confirmed.
  - Commerce- and payment-related references must be read.
  - `commerce-dynamicization.md` must be read, and the shared Header must be treated as a required consumer of customer and cart state.
- Minimum Page Plan path when entering the `online_checkout` transaction-page batch:
  - `products`: category/search/sorting, product results, pagination.
  - `product-detail`: product media, variants, quantity, purchasable state, add-to-cart action.
  - `cart`: product-line increase/decrease/delete, empty state, and amount summary.
  - `checkout`: contact, address, shipping, payment information, and order confirmation.
  - `order-result`: order and payment success/processing/failure/cancellation states and recovery actions.
  - Add `account` / `orders` / `order-detail` / `login` / `register` according to the user's membership and order-center scope.
- This path is the final page contract for `online_checkout`, not the list of first-batch static pages. The first batch is selected by the Page Plan's general page roles and budget strategy; transaction and account pages are generated in batches after the corresponding milestone/todo starts.
- Implementation checkpoints:
  - Confirm the ordering and payment flow
  - Connect the cart/order
  - Connect the payment API
  - Complete payment-flow acceptance
- Planning boundary: reuse Page Plan v3 and use `page-item` + `view-batch` per page; the Page Plan and per-page call results are the sole sources for page scope and prompts.

### member_auth

- Title: Membership and login
- Applies to:
  - The site needs login, a member center, order history, favorites, restricted content, or user profiles.
  - Commerce user flows involve orders, addresses, favorites, or member pricing.
- Does not apply to:
  - A purely public presentation site.
  - A one-off form submission without an account system.
- Prerequisites:
  - The authentication method is selected.
  - For a Commerce user flow, read the Commerce customer-login flow first.
- Implementation checkpoints:
  - Confirm the login method
  - Create login/registration/member pages
  - Connect member state and permissions
  - Complete membership-flow acceptance

### multilingual_seo

- Title: Multilingual and SEO
- Applies to:
  - The user targets multiple language markets.
  - The site needs search indexing, localized content, SEO titles/descriptions, or structured content.
- Does not apply to:
  - The user explicitly wants a single-language internal page.
  - Current page content is not stable enough for multilingual work.
- Prerequisites:
  - Content and target languages are clear.
  - The site's basic pages are stable.
- Implementation checkpoints:
  - Confirm language and SEO scope
  - Connect language switching and multilingual URLs
  - Complete SEO metadata
  - Complete multilingual/SEO acceptance

### email_marketing

- Title: Email marketing and notifications
- Applies to:
  - The site has subscriptions, event reminders, lead follow-up, order notifications, verification codes, or marketing email.
  - The user wants to build a customer contact channel.
- Does not apply to:
  - There is no email field or notification scenario.
  - The user explicitly does not need message notifications.
- Prerequisites:
  - Email purpose, trigger timing, and recipient rules are clear.
  - At least one trigger source exists among the form, membership, or order flow.
- Implementation checkpoints:
  - Confirm the email scenario
  - Connect email sending
  - Connect subscription or notification triggers
  - Complete email-flow acceptance
