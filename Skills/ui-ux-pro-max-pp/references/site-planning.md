# Site Planning

## Invocation

```json
{
  "skillName":"ui-ux-pro-max-pp",
  "scriptName":"scripts/search.js",
  "arguments":{"mode":"page-plan","brief":{}}
}
```

For the first Page Plan in AI Chat, the model must omit both `brief.designVariantSeed` and top-level session fields; AI Chat injects the authoritative `sessionId`. Outside AI Chat, pass a real top-level `sessionId` or an explicit UUID as top-level `designVariantSeed`. A seed is an opaque identity value, not design prose: put descriptive direction in `brief.creativeDirection`. After this call, reuse `resolvedBrief` unchanged so its script-returned `dvs-*` seed remains authoritative.

The Site Brief must be generated from the current request. When `pages` is not supplied, the script selects the first batch from internal candidates by page role:

`brief.contentLocale` must be `zh` or `en`; the script writes it back to `resolvedBrief` in lowercase.

1. Primary entry point;
2. Browsing or core proof path;
3. Evaluation or action loop;
4. Usually no more than 3 pages and about 9 page-specific blocks;
5. Task pages that depend on real business state are deferred by default;
6. Trust, contact, and detail pages enter only when needed by the current loop.

Page roles, scores, and unselected candidates are internal planning metadata. They do not enter the Page Plan and the model does not submit them manually. Pass `pages` only when the user explicitly specifies the current batch; the script generates strictly from that array.

## Page Plan v3

```json
{
  "version":3,
  "resolvedBrief":{},
  "decisionReceipts":[],
  "classification":{},
  "sharedBlocks":[{"blockRef":"site-header","label":"Header 导航","promptId":"..."}],
  "pages":[{
    "name":"home",
    "title":"首页",
    "purpose":"...",
    "blocks":[{"blockRef":"home-hero","label":"Hero 主视觉","promptId":"..."}]
  }]
}
```

Shared Header/Footer blocks appear only in `sharedBlocks[]`. The Page Plan does not carry complete prompts, assembly placeholders, selection diagnostics, or execution strategy.

Later batches reuse `resolvedBrief`; replace only `pages` and call `page-plan` again while keeping the project name, seed, and design direction. `commerceModel` expresses the site's final transaction capability and does not automatically expand the current batch. During the first planning call, the script performs deterministic routing: ambiguous selling, mall, or store intent maps to `catalog-contact`; only explicit cart, checkout, online-ordering, or payment intent enters `online-checkout`.

## Home composition defaults

For ecommerce, content-publication, and local-service homes, the Page Plan prefers photography-led or search/editorial first viewports over marketing landing stacks:

- Ecommerce home blocks default to `hero` + `featured-products` without a first-batch `trust-bar` / mid-page `cta` brick wall.
- Hero sampling biases against `split-left` / `split-reverse` marketing copy|media heroes and toward full-bleed overlay, editorial, lookbook, category, or search-led topologies.
- Anti-slop treats left-copy/right-image as the default AI landing tell; silent full-bleed photo walls remain forbidden.

## Brief reuse and classification evidence

Pass the returned `resolvedBrief` as `brief` on subsequent calls. Prefer its compact `pages` strings such as `["home","products","product-detail"]`. A list of page reference objects carrying `name` is also accepted; only the names select the batch, and other metadata does not override the plan. Use one representation per list with unique non-empty names. `resolvedBrief.pages` always returns normalized strings. Preserve the returned seed.

Sparse keyword evidence falls back to `classification.category="General"` instead of applying an unrelated industry design. `topCandidates` remains diagnostic; it is not a confirmed choice. Use the original request to decide whether clarification would change business content or capabilities. When the task is already clear, continue with its page structure and design requirements rather than asking the user to choose an internal taxonomy label.

Industry keywords do not establish luxury positioning. Express visual intent through `tone`, `creativeDirection`, and `palettePreference`.
