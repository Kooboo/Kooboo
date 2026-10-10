# k.paramConfig

> Read the structured JSON data saved in the background **Content → Parameter Configuration**

## Overview

`k.paramConfig` reads the data saved in the background **Content → Parameter Configuration**: each configuration has **name** (`name`), **display name**, **Schema** (field structure) and **data** (JSON value). Access according to configuration name and field name in Page, Layout, View or KScript. There is no need to write phone numbers, switches, link objects, etc. in the template.

See [Param Config](../../cms/content/user-options.md) for background maintenance methods.

::: The difference between tip and other APIs
| API | Purpose |
|-----|------|
| **`k.paramConfig`** | This page: Site-level JSON parameters defined by Schema |
| **`k.label` / `k.t`** | Multilingual short copy, see [k.label](../label/index.md) |
| **`k.content`** | Content folder entry fields, see [k.content](../content/index.md) |
| **Element Properties (KConfig)** | HTML attributes bound by template `k-config`, see [Tag Attributes](../../cms/content/tag-attributes.md) |
:::

## Access method

### Recommended Property Access

The **name** configured in the background is used as the first-level attribute of `k.paramConfig`; the field name in the Schema is used as the attribute below it (objects can continue to be nested).

```javascript
// Config name: siteContact; Schema contains phone and showBanner
var phone = k.paramConfig.siteContact.phone;
var show = k.paramConfig.siteContact.showBanner;
```

The engine will generate **KDefine** type definitions for each configuration of the current site. Under `k.paramConfig`, you can get intelligent prompts by configuration name and field name.


## data shape

- **Schema** is defined in the background and supports `string`, `number`, `boolean`, `object`, `array` (and nesting).
- **data** must be consistent with Schema; maintained in the background **Edit Data** page or JSON editor.
- Arrays and nested objects are accessed in the same way as ordinary JavaScript objects, such as `k.paramConfig.footer.links[0].url`.

## When it does not exist or the value is not filled in

- Configuration name does not exist, or the configuration does not yet have `data`: attribute access will result in **`undefined`**.
- The script should be judged before use to avoid errors when directly accessing deep properties.

## Related Docs

- [Param Config (CMS)](../../cms/content/user-options.md)
- [k.site](../site/index.md) — Site Resources
- [k.label](../label/index.md) — multilingual tags
