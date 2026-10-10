# Site Multilingual Label Development Guide

This guide covers multilingual Label copy in Kooboo site frontends. `k-label`, `k.label()`, and `k.t()` all use the same Label store; their shared goal is to “read the value for the current language”.

## Key conclusions

- Prefer `k-label` for plain-text nodes.
- Prefer `k.label(...)` for attribute values.
- Use `k.label(...)` or `k.t(...)` to read copy in server-side scripts.
- Labels support `{param}` placeholder arguments.
- `k.t(...)` has one behavior beyond `k.label(...)`: when a label does not exist, it automatically adds a label with the same key.

## Relationship between the three forms

### `k-label`

Use it to hand an element's inner text to the Label system.

```html
<h1 k-label="hero_title">Build faster with Kooboo</h1>
```

Rendering reads the value of `hero_title` in the current language.

Suitable for:

- Headings
- Button copy
- Paragraph copy
- Link text

Not suitable for:

- Elements such as `<input>` and `<textarea>` that require attribute values
- Complex content that must be assembled and output in a script

### `k.label(...)`

Use it to actively read a label in KScript / template bindings.

```html
<div k-content="k.label('hero_title')"></div>
```

Suitable for:

- `k-content`
- `k-attribute`
- `<script>` with `env="server"`
- Returning current-language text from API / Service / redirect logic

### `k.t(...)`

`k.t(...)` also reads a label in the current language, but is better for quickly bringing existing static copy into the multilingual system.

```html
<p k-content="k.t('No data available')"></p>
```

In the current implementation, if the `"No data available"` label does not yet exist, `k.t(...)` first adds the label automatically and then returns the current-language value.

Recommended mental model:

- `k.label("agreed_key")`: read an already agreed label key.
- `k.t("literal text")`: connect existing copy directly to the multilingual system.

## Common forms

### Text nodes

```html
<h2 k-label="pricing_title">Simple pricing</h2>
<p k-label="pricing_desc">Choose a plan that fits your team.</p>
```

### Attribute values

Do not use `k-label` directly on `<input>` because it only renders an element's inner content. Use `k.label(...)` for attribute values.

```html
<input k-attribute="placeholder k.label('search_placeholder')">
```

### Combining text and attributes

```html
<a href="/contact" k-label="contact_us">Contact us</a>
<input k-attribute="aria-label k.label('search_aria_label')">
```

## Passing Label parameters

Label values support `{param}` placeholders.

For example, a label value saved in the backend may be:

```text
Hello {name}
```

Pass parameters when reading it on the frontend or server:

```html
<div k-content="k.label('welcome_user', { name: 'Kooboo' })"></div>
```

You can also use it in KScript:

```js
var message = k.label("order_created", {
    orderNo: "KB20260514001"
})
```

If the label content is:

```text
Order created successfully: {orderNo}
```

the output is:

```text
Order created successfully: KB20260514001
```

## Selection guidance

### Prefer `k-label` when

- The page contains fixed text.
- You want AI to mark which copy should be multilingual directly in the HTML structure.
- The goal is for the backend content team to maintain this copy centrally in the Label list.

### Prefer `k.label(...)` when

- Copy must be written into attributes such as `placeholder`, `title`, `aria-label`, or `alt`.
- Copy must first be assembled in a server-side script.
- An API / Service must return a current-language message.

### Prefer `k.t(...)` when

- You already have static copy and want to make it multilingual quickly.
- Automatically adding a label on the first run is acceptable.

## Coordination with the backend

- The global multilingual switch is on the Basic page at `/_Admin/system/settings?SiteId=xxx`.
- The Label list is at `/_Admin/content/labels?SiteId=xxx`.
- `k-label`, `k.label()`, and `k.t()` ultimately depend on multilingual values in the Label list.

## Related document:

- [Multilingual site scenario guide](../Scenario/multilingual-site-guide.md)
