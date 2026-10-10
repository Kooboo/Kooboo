# k.site.styles

> Addition, deletion, modification and query of site CSS style resources, routing URL and version log

## Overview

`k.site.styles` Manages **Style** resources (stand-alone CSS files) in the site. Styles are externally accessible through routed URLs (such as `/site.css`) and can be referenced in pages or layouts.

::: The difference between tip and layouts / views
`k.site.styles` supports **`getByUrl`**, **`getUrl`**, **`getAbsUrl`** and other routing-related APIs (similar to [k.site.pages](./page.md)); [k.site.layouts](./layout.md) and [k.site.views](./view.md) have no routing API.
:::

The sidebar entry "css" is this API.

## TypeScript Definition

```ts
interface StyleRepository {
  add(style: StyleInput): void;
  all(): Style[];
  get(nameOrId: string): Style | null;
  getByUrl(url: string): Style | null;
  update(style: Style): void;
  updateBody(nameOrId: string, body: string): void;
  delete(nameOrId: string): void;
  getUrl(id: string): string | null;
  getAbsUrl(id: string): string | null;
  getLogs(nameOrId: string): ChangeLog[] | null;
  getByLog(logId: number): Style | null;
}

interface StyleInput {
  name: string;
  body: string;
  url?: string;
}
```

## add()

Create styles and register routes. If `url` is not passed, it defaults to `/{name}`, and if the path does not contain an extension, the style extension (usually `.css`) will be added automatically.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `style.name` | `string` | yes | Style name (unique) |
| `style.body` | `string` | yes | CSS content |
| `style.url` | `string` | no | Access path, such as `/assets/theme.css` |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-style-" + stamp
    const url = "/ai-style-" + stamp + ".css"

    k.site.styles.add({
        name,
        url,
        body: `.ai-style-${stamp} { color: #333; }`
    })

    const style = k.site.styles.getByUrl(url)
    return {
        verified: !!style && style.name === name,
        id: style?.id,
        route: style ? k.site.styles.getUrl(style.id) : null
    }
})
```

## all()

Return all styles.

**Parameters:** None.

**Returns:** `Style[]`.

```ts
k.api.get(() => {
    const styles = k.site.styles.all()
    return { count: styles.length, names: styles.slice(0, 10).map((s) => s.name) }
})
```

## get()

Get a style by name or Id.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Style name or ID |

**Returns:** `Style | null`.

```ts
k.api.get(() => {
    const style = k.site.styles.get("site")
    return style ? { id: style.id, name: style.name, extension: style.extension } : null
})
```

## getByUrl()

Get the style by **routing URL** (e.g. `/main.css`).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `url` | `string` | yes | Style route URL |

**Returns:** `Style | null`.

```ts
k.api.get(() => {
    const style = k.site.styles.getByUrl("/main.css")
    return style ? { id: style.id, name: style.name } : null
})
```

## update()

Update the complete style object (usually `get` / `getByUrl` first and then `body`).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `style` | `Style` | yes | Modified full Style object |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-style-upd-" + stamp
    const url = "/ai-style-upd-" + stamp + ".css"

    k.site.styles.add({ name, url, body: ".v1 {}" })
    const style = k.site.styles.getByUrl(url)
    style.body = ".v2 {}"
    k.site.styles.update(style)

    const after = k.site.styles.getByUrl(url)
    return {
        verified: after?.body?.indexOf("v2") >= 0,
        hasV1: after?.body?.indexOf("v1") >= 0
    }
})
```

## updateBody()

Only updates `body`.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Style name or ID |
| `body` | `string` | yes | New CSS content |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-style-body-" + stamp
    const url = "/ai-style-body-" + stamp + ".css"

    k.site.styles.add({ name, url, body: ".before {}" })
    k.site.styles.updateBody(name, ".after-body {}")

    const after = k.site.styles.getByUrl(url)
    return { verified: after?.body?.indexOf("after-body") >= 0 }
})
```

## delete()

Delete by Name or Id. Use `get(name)` for verification after deletion. Do not rely on `getByUrl` after deletion.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Style name or ID |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-style-del-" + stamp
    const url = "/ai-style-del-" + stamp + ".css"

    k.site.styles.add({ name, url, body: ".del {}" })
    const before = k.site.styles.getByUrl(url)

    k.site.styles.delete(name)
    const after = k.site.styles.get(name)

    return {
        verified: !!before && !after,
        hadBefore: !!before
    }
})
```

## getUrl() / getAbsUrl()

| Method | Parameters | Returns | Description |
|------|------|------|------|
| `getUrl(id)` | `id: string` | `string | null` | Gets the relative URL |
| `getAbsUrl(id)` | `id: string` | `string | null` | Gets the absolute URL |

```ts
k.api.get(() => {
    const styles = k.site.styles.all()
    for (const style of styles) {
        const rel = k.site.styles.getUrl(style.id)
        if (!rel) continue
        const abs = k.site.styles.getAbsUrl(style.id)
        return { relative: rel, absolute: abs }
    }
    return null
})
```

Only styles with registered routes can return URLs; styles embedded in the page may not have independent routes.

## getLogs() / getByLog()

| Method | Parameters | Returns | Description |
|------|------|------|------|
| `getLogs(nameOrId)` | `nameOrId: string` | `ChangeLog[] | null` | Gets version logs |
| `getByLog(logId)` | `logId: number` | `Style | null` | Gets a historical version by log ID |

```ts
k.api.get(() => {
    const styles = k.site.styles.all()
    if (!styles.length) return { error: "no styles" }

    const logs = k.site.styles.getLogs(styles[0].id)
    return {
        verified: logs === null || Array.isArray(logs),
        logCount: logs ? logs.length : 0
    }
})
```

## Common Style Fields

| Field | Type | Description |
|------|------|------|
| ID | `string` | StyleId |
| name | `string` | name |
| body | `string` | CSS content |
| extension | `string` | extension, usually `css` |
| media | `string` | Media query (such as `screen`) |
| online | `boolean` | Is it online? |
| version | `number` | version number |

## Related Docs

- [k.site](./)
- [k.site.scripts](./js.md)
- [k.site.pages](./page.md)
