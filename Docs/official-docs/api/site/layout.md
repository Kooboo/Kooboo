# k.site.layouts

> Create, read, update, delete, and inspect version logs for Layout resources

## Overview

Layouts are the HTML skeleton of a page. They define shared structure and `k-placeholder` regions, and pages reference them with `<layout id="...">` inside the `body`. `k.site.layouts` manages Layout objects in the site repository.

::: tip Difference from pages
`k.site.layouts` provides `add`, `all`, `get`, `update`, `updateBody`, `delete`, and version-log APIs, but it does **not** provide `getByUrl`, `getUrl`, or `getAbsUrl`. Those URL lookup APIs belong to [k.site.pages](./page.md).
:::

## TypeScript Definition

```ts
interface LayoutRepository {
  add(layout: LayoutInput): void;
  all(): Layout[];
  get(nameOrId: string): Layout | null;
  update(layout: Layout): void;
  updateBody(nameOrId: string, body: string): void;
  delete(nameOrId: string): void;
  getLogs(nameOrId: string): ChangeLog[] | null;
  getByLog(logId: number): Layout | null;
}

interface LayoutInput {
  name: string;
  body: string;
}
```

## `add()`

Creates a Layout.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `layout.name` | `string` | Yes | Layout name; use the `layout id` when referencing it from a page |
| `layout.body` | `string` | Yes | HTML that includes `k-placeholder` or placeholder `div` elements |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-layout-" + stamp

    k.site.layouts.add({
        name,
        body: `<!DOCTYPE html><html><body><div k-placeholder="Main">Layout ${name}</div></body></html>`
    })

    const layout = k.site.layouts.get(name)
    return {
        verified: !!layout && layout.name === name,
        id: layout?.id
    }
})
```

## `all()`

Returns all Layout resources.

**Parameters:** None.

**Returns:** `Layout[]`.

```ts
k.api.get(() => {
    const layouts = k.site.layouts.all()
    return { count: layouts.length, names: layouts.slice(0, 5).map((l) => l.name) }
})
```

## `get()`

Gets a Layout by name or ID.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Layout name or ID |

**Returns:** `Layout | null`.

```ts
k.api.get(() => {
    const layout = k.site.layouts.get("main")
    return layout ? { id: layout.id, name: layout.name } : null
})
```

## `update()`

Updates the full Layout object.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `layout` | `Layout` | yes | Modified full Layout object |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-layout-upd-" + stamp

    k.site.layouts.add({
        name,
        body: `<div k-placeholder="Main">v1</div>`
    })

    const layout = k.site.layouts.get(name)
    layout.body = `<div k-placeholder="Main">v2</div>`
    k.site.layouts.update(layout)

    const after = k.site.layouts.get(name)
    return {
        verified: after?.body?.indexOf("v2") >= 0,
        hasV1: after?.body?.indexOf("v1") >= 0
    }
})
```

## `updateBody()`

Updates only the `body` field.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Layout name or ID |
| `body` | `string` | yes | New Layout HTML |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-layout-body-" + stamp

    k.site.layouts.add({ name, body: "<div>before</div>" })
    k.site.layouts.updateBody(name, "<div>after-body</div>")

    const after = k.site.layouts.get(name)
    return { verified: after?.body?.indexOf("after-body") >= 0 }
})
```

## `delete()`

Deletes a Layout by name or ID.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Layout name or ID |

**Returns:** `void`.

::: warning
After deleting, confirm with `get(nameOrId)`. Do not depend on a nonexistent `getByUrl`.
:::

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-layout-del-" + stamp

    k.site.layouts.add({ name, body: "<div>del</div>" })
    const before = k.site.layouts.get(name)

    k.site.layouts.delete(name)
    const after = k.site.layouts.get(name)

    return {
        verified: !!before && !after,
        hadBefore: !!before
    }
})
```

## `getLogs()` / `getByLog()`

| Method | Parameters | Returns | Description |
|------|------|------|------|
| `getLogs(nameOrId)` | `nameOrId: string` | `ChangeLog[] | null` | Gets version logs |
| `getByLog(logId)` | `logId: number` | `Layout | null` | Gets a historical version by log ID |

```ts
k.api.get(() => {
    const layouts = k.site.layouts.all()
    if (!layouts.length) return { error: "no layouts" }

    const logs = k.site.layouts.getLogs(layouts[0].id)
    return {
        verified: logs === null || Array.isArray(logs),
        logCount: logs ? logs.length : 0
    }
})
```

`getByLog(logId)` restores a Layout snapshot from a version log ID, just like [k.site.pages](./page.md).

## Common Layout Fields

| Field | Type | Description |
|------|------|------|
| id | `string` | Layout ID |
| name | `string` | Name |
| body | `string` | HTML content |
| extension | `string` | Usually `html` |
| online | `boolean` | Whether it is online |
| version | `number` | Version number |

## Use with Pages

Example of referencing a Layout from a page `body`:

```html
<layout id="main">
  <placeholder id="Main">
    <h1>Page content</h1>
  </placeholder>
</layout>
```

The `main` Layout must already exist in `k.site.layouts`, and its body must define `k-placeholder="Main"`.

## Related Docs

- [k.site](./)
- [k.site.pages](./page.md)
- [k.site.views](./view.md)
