---
name: kooboo-jsonld
description: "Use when adding, inspecting, or debugging JSON-LD structured data in Kooboo pages, views, or layouts: k-json-ld, schema.org, Product, Article, Organization, BreadcrumbList, rich results, field mappings, or missing application/ld+json output. Covers template syntax and current rendering limitations."
---

# Kooboo JSON-LD

## Workflow

1. Before changing site code, load `kooboo-coding` and follow its read, edit, and verification workflow. For admin configuration, load `kooboo-cms-guide` and read `references/operations/ai-service.md`.
2. Inspect the target page, its views/layouts, existing structured-data scripts, and the data available to its render context. Choose a schema.org type matching the visible content; do not invent ratings, prices, availability, or business details.
3. Prefer the explicit template element below for page-specific structured data. Do not assume saving field mappings creates page output.
4. Inspect the rendered HTML, parse every `application/ld+json` script as JSON, and compare values with the visible page. Check duplicate or conflicting entities across the page and layout.
5. Validate schema semantics separately with Schema.org Validator and, for eligible Google features, Rich Results Test. Valid JSON does not establish rich-result eligibility or guarantee indexing.

## Explicit Template Element

Use `<k-json-ld>`. Put declarations directly inside it, without ordinary HTML wrappers or raw JSON.

```html
<k-json-ld>
  <type name="Product">
    <property name="name" value="'Laptop'"></property>
    <property name="description" value="'Lightweight laptop'"></property>
    <map as="offers">
      <type name="Offer">
        <property name="price" value="99"></property>
        <property name="priceCurrency" value="'USD'"></property>
      </type>
    </map>
  </type>
</k-json-ld>
```

These are illustrative values, not facts to copy into a real site. The rendered JSON is equivalent to:

```json
{
  "@context": "https://schema.org",
  "@type": "Product",
  "name": "Laptop",
  "description": "Lightweight laptop",
  "offers": { "@type": "Offer", "price": 99, "priceCurrency": "USD" }
}
```

The template element is replaced by `<script type="application/ld+json">`. Each root `type` produces a separate script, not an automatic `@graph`.

## Values and Nesting

- `type name` sets `@type`; root types also receive `@context`.
- `property name` is the output key. `value` is read from the attribute, not the element's text.
- `value="product.name"` resolves through the render data context. First establish that `product` exists; this is not a database query or a general JavaScript expression.
- If context lookup fails, values are interpreted as null, boolean, integer, floating-point number, or a string with paired quotes removed. An unresolved `product.name` can become the literal string `"product.name"`. Inspect real values rather than assuming missing fields are omitted.
- Use inner quotes for literal strings, for example `value="'USD'"`. Follow normal HTML attribute escaping for quotes and special characters.
- Use `map as="offers"` without `from` to construct a nested object. `as` names the output property; it is not a variable alias.
- Do not directly nest `type` to create a named property. Child type keys are merged into the parent and can overwrite its `@type`; `type as` is parsed but not used by rendering.
- `map from` looks up an existing context value. Missing values yield null. Although it accepts JSON objects/arrays and non-string enumerables, the current renderer does not bind the current item into the child context. Do not use it as an item loop: it can repeat values from the same outer context. For item-dependent arrays, use a verified alternative supported by the site's coding workflow and serialize structured data safely; do not manually interpolate unescaped JSON into a script.
- Only `type`, `property`, and `map` declarations are recognized. Unknown wrappers are not traversed. Root-level properties/maps alone do not emit documents.

## Field Mapping Configuration

The admin JSON-LD configuration supports source/table or content-type selection and source-field mappings to schema.org types/properties. Configured sources include content, IndexedDB, SQLite, MySQL, SQL Server, and MongoDB. External connection failures can leave lists empty. Mapping does not modify the original data or validate schema requirements. Verify page output using the explicit template element above.

## Troubleshooting

1. No script: verify the actual rendered route, template inclusion, `<k-json-ld>` element, direct root `type`, and whether data evaluators are disabled. Ignoring `EnumEvaluator.Data` skips this element.
2. Literal paths in JSON: check context variable names and data availability at render time.
3. Wrong nested type or repeated collection objects: check direct nested `type` usage and the `map from` current-item limitation above.
4. Invalid or unexpected output: inspect server-rendered HTML and errors, not just template source. Rendering exceptions may appear as text in the page. The renderer does not validate schema.org names or required fields.
5. JSON parses but validation fails: verify the selected type, required properties, URLs, actual values, and consistency with visible content. Report remaining warnings without promising search-engine results.