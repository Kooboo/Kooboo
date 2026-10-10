# Dynamic Delivery (S6–S10)

Read this document only when the user actively requests dynamicization or explicitly accepts a dynamicization recommendation.

## S6 Trigger

Until dynamicization is confirmed, remain in the static stage. A recommendation card is not default consent and cannot justify creating data structures, APIs, login, orders, or payment capabilities.

## S7 Data-Source Decision

First output the mapping from entities to data sources, and declare cross-source relation keys and the SSR path:

- Commerce Product, Category, Variant, Cart, Customer, and Order default to `commerce` as the source of truth;
- Ordinary content uses `content`, custom relational data uses `sqlite`, and external data uses `fetch`;
- SSR defaults to `<k-data>`; use `env="server"` only for imports or dedicated `k.*` capabilities that k-data cannot express;
- Commerce must check shared state and real consumers across Products, PDP, Header, Cart, Checkout, Order Result, and Account/Orders, and read [Commerce Dynamicization Contract](commerce-dynamicization.md).

Do not enter implementation until the data-source mapping is complete.

## S8 Data Structures

When the mapping includes `sqlite` or `content`, first confirm fields, constraints, and relations, then create the structure. For Content modeling, read `references/K-Script/content/function_guide.md`; for actual CRUD, read the relevant module reference. When only `commerce` / `fetch` is used, do not create a parallel data model.

## S9 Dynamic Implementation

Keep the static version's major DOM, topology, visual hierarchy, and responsive structure, and bind real data and behavior in place.

1. Track A: prefer `<k-data>` for first-screen queries, filtering, pagination, field calculations, and conditional branches;
2. Track B: use API + frontend JS for submission, persistence, authentication, cart, orders, and payment;
3. Read [Backend Creation Functions](../../Functions/backend-create.md) and the Backend reference as needed for APIs / CodeBlocks;
4. Dynamic images still use [ImagePlaceholder](../../Functions/image-placeholder.md) or real business resources;
5. Validate Commerce delivery with `deliveryStage: "commerce-runtime"` and payment with `payment-runtime`; a Preflight PASS proves structure only. Do not complete a milestone when `completionGate.missingEvidence` is non-empty.

Static menus, tabs, galleries, hover, and reveal interactions do not belong to this stage and must not be added to the first version under the pretext of dynamicization.

## S10 Dynamic Iteration

Make small changes from user feedback, marking only the actually changed objects dirty in each round. When the AI Chat Completion Ledger is connected, `mode: "focused"` may be used explicitly; the adapter fills in dirty objects and the model supplies existing direct consumers. Each round must obtain a persistence receipt and the necessary same-path runtime evidence. After all steps for one runtime requirement are complete, attach its `verificationRequirementId` only to the final successful browser assertion. When the entire dynamic capability stage is complete (or a commerce-runtime / payment-runtime milestone is reached), run one scan with `mode: "capability"`; the adapter authoritatively fills all successfully written objects in the session, and the model adds existing consumers that belong to the capability but were not modified. Payment Provider, callback, permissions, order status, and similar concerns require business-result evidence and cannot be replaced by structural scanning; Checkout form visibility, API subroutes, order ownership, and payment state after the last write are all same-path evidence. If only structural checks are complete, remain `structurally verified`; if same-path runtime has passed but provider/admin business results are still missing, the maximum status is `runtime verified`.
