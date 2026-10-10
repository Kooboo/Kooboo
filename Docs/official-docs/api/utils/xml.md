# k.utils.xml

> Parse and build XML documents

## Overview

`k.utils.xml` is based on the `XDocument` package and provides `parse`, `create` and document/element/attribute operations.

## parse()

Parses the XML string as `Document`.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `xml` | `string` | Yes | XML string |

**Returns:** `Document`.

```ts
const doc = k.utils.xml.parse("<root><item id=\"1\">a</item></root>")
const xml = doc.stringify()
```

## create()

Creates a new document starting with the specified root element name.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `rootName` | `string` | Yes | Root element name |

**Returns:** `Document`.

```ts
const doc = k.utils.xml.create("root")
doc.root.addChild("item")
```

## Document

| Member | Description |
|------|------|
| `root` | Root `Element` |
| `declaration` | XML declaration |
| `stringify()` | serialize to string |

## Element

| members/methods | Description |
|-------------|------|
| `name` / `value` | Element name and text |
| `children` / `attributes` | Child elements and attribute arrays |
| `addChild(name)` | Add child element |
| `addAttribute(name, value)` | Add properties |
| `addCData(content)` | Add CDATA |
| `removeChild(name)` / `removeAttribute(name)` | delete |

| Method | Parameter | Return |
|------|------|------|
| `addChild(name)` | `string` | `Element` |
| `addAttribute(name, value)` | `string, string` | `Attribute` |
| `addCData(content)` | `string` | `void` |
| `removeChild(name)` | `string` | `void` |
| `removeAttribute(name)` | `string` | `void` |

```ts
const doc = k.utils.xml.create("items")
const item = doc.root.addChild("item")
item.addAttribute("id", "1001")
item.value = "Kooboo"
doc.root.addCData("raw <xml> content")
doc.root.removeAttribute("draft")
const xml = doc.stringify()
```

## Related Docs

- [k.utils Overview](./index.md)
