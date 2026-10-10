# k.site.labels

> Manage multilingual tags for your site

## Overview

`k.site.labels` is the manager of labels, which are used to store multilingual text content. With tags you can:

- Centrally manage your site’s multilingual text
- Reference tags in templates instead of hard-coded text
- Internationalize easily

## Prerequisites

::: tip Multi-language configuration
Before using the multi-language function, you need to complete the following configuration in the Kooboo background
:::
### 1. Enable multiple languages

**Configuration Path**: Site Settings → Basic Settings → Basics → Multilingual

1. Enter the Kooboo CMS backend
2. Select target site
3. Enter Site Settings → Basic Settings → Basics tab
4. Add the required language in the multi-language option (such as `zh` Chinese, `en` English)
5. Save settings

### 2. Configure label content

**Configuration Path**: Content → Tags

On this page, you can create and manage labels, and configure the translated text of each label in different languages. See [Admin: Labels](../../cms/content/labels.md) for operating instructions.

For example, create a `Cancel` tag:
- `zh`: Cancel
- `en`: Cancel

## k.label() / k.t()

`k.label()` and `k.t()` are global functions with exactly the same functions, used to obtain the text content of the specified tag. They return corresponding translations based on the current site's language settings.

### parameter

| Parameter | Type | Required | Description |
|------|------|------|------|
| NameOrId | string | yes | The name or ID of the label |
| params? | object | no | Replace parameter object |

### return value

`string` - the text content of the label

### code example

```ts
// Get the current-language text by label name
const text = k.label("Cancel");
// Returns: "Cancel" in the current language

// Use k.t() (same behavior)
const text2 = k.t("Cancel");
// Returns: "Cancel"

// Replace parameters
const text3 = k.t("Items: {count}", { count: 123 });
// Returns: "Items: 123"

const text4 = k.label("Welcome {name}", { name: "Alice" });
// Returns: "Welcome Alice"
```

## labels method

`k.site.labels` provides the following methods:

### all()

Get a list of all tags.

**Parameters**: none.

**Returns**: `object[]`. Each object contains key-value pairs for every language version.

```ts
const labels = k.site.labels.all();
// Returns: [{ zh: "Cancel", en: "Cancel" }, ...]
```

Each returned label object contains key-value pairs for each language version.

### get(nameOrId)

Get a single tag based on its name.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `nameOrId` | `string` | Yes | Label name or label ID. |

**Returns**: `object | null`. Matching label object, or an empty value when it does not exist.

```ts
const label = k.site.labels.get("Cancel");
// Returns: { zh: "Cancel", en: "Cancel" }
```

## Related Docs

- [k.site](../site/index.md) - Site Overview
