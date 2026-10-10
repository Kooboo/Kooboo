# KScript Return Objects and Template Binding

Values returned by `k.*` APIs can be CLR-backed Kooboo runtime wrappers or ordinary dictionaries/arrays, depending on the API. These differences matter during KScript and SSR template evaluation; the browser ultimately receives rendered HTML, not these server-side objects.

Code in `<script env="server">` looks like JavaScript, but **SSR template bindings** (native `href` interpolation, `k-for`, `k-content`, and `k-attribute`) resolve object members through Kooboo template read paths. Differences between script-side and template-side behavior for “mutating properties, spreading objects, and casing” are a common source of pitfalls.

> Boundary: This document explains field naming and object-copying issues in template bindings. It does not define how to discover or modify Site objects; follow the corresponding object or Function reference for those operations.

## Symptoms (using `currentURLs()` as an example)

`k.site.multilingual.currentURLs()` returns a list of language links. The module reference and generated `kooboo.d.ts` are the authoring contract for fields: `key`, `name`, `URL` (`[KeepCase]`), and `isActive`. Generated camelCase names do not guarantee the enumerable keys of every CLR-backed runtime object.

### Pitfall 1: Adding a new field to an item in `forEach`

```javascript
langLinks.forEach((item) => {
    item.linkClass = item.isActive ? 'active' : 'muted'
})
```

This is valid for a plain JavaScript object, but do not use mutation as a reliable way to add a template field to a **CLR-backed runtime object**:

- Script-side assignment and template-side member resolution use different readers. The template should not be expected to treat a newly attached `linkClass` as a declared/source field.

### Pitfall 2: Using `{ ...item, linkClass: '...' }` and then `map`

```javascript
langLinks = langLinks.map((item) => ({
    ...item,
    linkClass: '...'
}))
```

This produces a **plain JavaScript object**, so the template can read `linkClass`, but for a CLR-backed source item:

- The keys copied by the spread may reflect **CLR names** (`Key`, `Name`, `IsActive`), which differs from the generated camelCase names.
- For **plain JS objects**, templates use **exact key names**; casing must match the object's actual keys.
- Result: `item.key` stops working and only `item.Key` works, conflicting with the documentation and other page patterns.

Apply this warning only when the source may still be a CLR-backed or otherwise opaque runtime object. If the relevant API reference explicitly says it returns a plain dictionary/array, follow that reference's field contract instead.

## Mechanism summary

| Layer | Behavior |
|------|------|
| **KScript return objects** | Runtime shape varies by API. Read fields from the module reference, generated `kooboo.d.ts`, and confirmed source contract; do not infer enumerable keys by probing. |
| **SSR template `k-for`** | The loop variable can reference the original runtime value; template member reads use Kooboo's template reader and may differ from script-side object mutation. |
| **New object returned by `{...item}` / `map`** | Becomes a plain JavaScript object; templates use **exact keys**. For CLR-backed input, spread may copy CLR names rather than generated d.ts names. |
| **Some Commerce APIs** | `ToKscriptModel` recursively converts supported values into camelCase dictionaries, arrays, and primitive values. This is a **server-defined DTO contract**, not a global rule. |

`URL` on `SiteMultilingualInfo` has `[KeepCase]`; keep `URL` when generating definitions and do not use `url`.

## Rules AI must follow

### 1. Preferred: Compute UI state in the template (recommended)

Do not pre-attach `linkClass` to the item; use the `k-attribute` class-object syntax (consistent with [multilingual-site-guide](../Scenario/multilingual-site-guide.md)):

```html
<script env="server">
    var links = k.site.multilingual.currentURLs()
</script>
<ul>
    <li k-for="item in links">
        <a href="{item.URL}" k-attribute="class {
            'text-white bg-cta/30 border border-cta/50 font-semibold': item.isActive,
            'text-white/70 hover:text-white hover:bg-white/10 border border-white/20': !item.isActive
        }" k-content="item.name"></a>
    </li>
</ul>
```

Use the **camelCase** field names from **`kooboo.d.ts` / the module reference** (`item.name`, `item.isActive`); keep `URL` uppercase.

### 2. When a derived field is required in KScript: construct a plain object explicitly

**Do not** rely on the `...item` spread. **Copy only the documented fields needed**, one by one, and use camelCase consistently (**unused fields do not need to be copied**):

```javascript
var links = k.site.multilingual.currentURLs()
links = links.map(function (item) {
    return {
        key: item.key,
        name: item.name,
        URL: item.URL,
        isActive: item.isActive,
        linkClass: item.isActive
            ? 'text-white bg-cta/30 border border-cta/50 font-semibold'
            : 'text-white/70 hover:text-white hover:bg-white/10 border border-white/20'
    }
})
```

If reading a field fails, return to the corresponding module reference, `kooboo.d.ts`, or confirmed public source contract. Do not guess the object shape through runtime introspection or method enumeration. For CLR-backed input, do not expose copied `Key`/`Name` keys when the documented output contract is `key`/`name`; for a documented plain-dictionary API, use its actual documented keys.

### 3. Prohibited

- Do not rely on `item.newField = ...` in a `forEach` over a CLR-backed `k.*` return value to add a field for the template.
- Do not assume `{ ...item, ... }` or `Object.assign({}, item)` preserves the documented d.ts casing when the source is CLR-backed or otherwise opaque.
- Do not use `JSON.parse(JSON.stringify(...))` as a general “convert to plain object” solution (it may lose types, makes key names uncontrollable, and fails for unserializable members).
- Do not use runtime introspection or method enumeration to discover APIs not confirmed in a reference; use the corresponding object/Function reference for Site writes.

### 4. Self-check (include in reasoning)

- Is the source a CLR-backed or otherwise opaque runtime object, or does its reference explicitly promise a plain dictionary/array? → Apply the matching rule rather than assuming all `k.*` results behave alike.
- Is the requirement only presentation logic such as class / href? → Keep `href` native, bind dynamic class through `k-attribute`, and do not mutate the item in the script.
- Must `map` produce a new field? → Use an explicit field list; spread is prohibited.
- Are you trying to confirm a field or method through probing? → Stop; read the module reference, `kooboo.d.ts`, or a confirmed public source contract instead.

## Relationship to other documents

- Template binding capabilities: [template-binding-syntax.md](../Frontend/template-binding-syntax.md) (`k-attribute` class object)
- Multilingual switching example: [multilingual-site-guide.md](../Scenario/multilingual-site-guide.md)
- Script variable naming: [code-style.md](../Frontend/code-style.md). API field names come from the corresponding module reference, generated `kooboo.d.ts`, and confirmed source contract.
