# Site Implementation

## Design System

Use the Page Plan's `resolvedBrief`:

```json
{"mode":"design-system","brief":{}}
```

The output keeps only the `classification`, `pattern`, `style`, `colors`, `typography`, `keyEffects`, and `antiPatterns` required for implementation. `colors` is authoritative for the default palette; later changes explicitly confirmed by the user must be synchronized to the Layout, CSS variables, Tailwind, and View.

## Page Item

```json
{"mode":"page-item","brief":{},"pageName":"home"}
```

Return `version`, `pageName`, and ordered `blockRefs[]`. Return a non-empty `compositionGroups[]` only when the current page has a primary/aside composition across Views; Page owns the composition wrapper, and ordinary pages omit this field. Derive placeholders from `uupm-${blockRef}`. Handle shared Views separately from the Page Plan's `sharedBlocks[]`.

`compositionGroups[]` describes cross-View relationships that ordinary Page HTML/CSS must own:

```json
{
  "groupRef":"product-detail-product-decision",
  "blockRefs":["product-detail-product-gallery","product-detail-product-summary"],
  "primaryBlockRef":"product-detail-product-gallery",
  "asideBlockRef":"product-detail-product-summary",
  "desktopTopology":"primary-aside",
  "responsiveStrategy":"stack-primary-first",
  "layoutIntent":"Place product media and the purchase summary side by side on desktop..."
}
```

Page is not limited to placing `<view>` elements consecutively. It may use `main`, `section`, `div`, `aside`, grid/flex wrappers, headings, and breadcrumbs, then place independent Views in the corresponding regions. Composition fixes only the task information architecture; it does not fix column widths, gaps, sticky behavior, corner radii, or visual style:

- `product-gallery + product-summary`: product media primary area and purchase summary are adjacent on desktop.
- `cart-items + checkout-summary`: cart item primary area and amount/checkout aside are adjacent on desktop.
- `checkout-form + checkout-summary`: checkout form primary area and order-review aside are adjacent on desktop.
- `order-status + checkout-summary`: order-status primary area and amount-review aside are adjacent on desktop.

On mobile, stack using `stack-primary-first`. Adjust this only when the user explicitly requests another structure or content/device constraints provide a strong reason. Do not degrade to two full-width sections stacked vertically on desktop merely because the View was split, generation is easier, or the creative direction differs.

## View Batch

```json
{"mode":"view-batch","brief":{},"blockRefs":["home-hero","home-features"]}
```

The top-level `baseContract` appears once. Each item contains `blockRef`, `recommendedViewName`, and a differentiated `prompt`. This result remains the authoritative source for View boundaries, block order, required content, composition roles, image/data/interaction policy, locale, palette, and real capability constraints.

Generate each View from `baseContract + current item.prompt`, retaining UUPM component building-block suggestions while composing a coherent page around the user request. Coordinate hierarchy, media, section handoff, and mobile layout across Views. Read [Page Composition](page-composition.md) when whole-page design guidance helps. The model designs and generates directly, without submitting a separate design document or obtaining a validation receipt. Execute every View independently: generate → create → read back → Prompt Conformance Gate → update when necessary → Preflight.

When a composition group is matched, the item prompt states the current View's primary/aside role. The View should remain fluid within its assigned column, must not create a Page wrapper across siblings, and must not duplicate a sibling's responsibility; Page owns their side-by-side relationship. When a product summary has an independent Gallery sibling, do not create a second dominant product hero image.

The Gate checks component roles, required content, avoid patterns, image/data authenticity, interaction authenticity, and how the View fits the whole-page composition. Do not generate a generic View from only the Page Plan summary or industry convention. Final HTML contains page content, not design-process explanations.
