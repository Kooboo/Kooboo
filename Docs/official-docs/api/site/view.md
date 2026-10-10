# k.site.views

> Addition, deletion, modification and version log of reusable view (View) fragments

## Overview

View is an HTML fragment that can be referenced through `<view id="view name">` in a page or layout, and is suitable for splitting reusable blocks such as headers, cards, and lists. `k.site.views` is used to manage view resources in the site.

::: The difference between tip and pages
Same as [k.site.layouts](./layout.md), `k.site.views` provides `add` / `all` / `get` / `update` / `updateBody` / `delete` and version log APIs, **not** `getByUrl` / `getUrl` / `getAbsUrl` (only [k.site.pages](./page.md) supports finding pages by URL).
:::

## TypeScript Definition

```ts
interface ViewRepository {
  add(view: ViewInput): void;
  all(): View[];
  get(nameOrId: string): View | null;
  update(view: View): void;
  updateBody(nameOrId: string, body: string): void;
  delete(nameOrId: string): void;
  getLogs(nameOrId: string): ChangeLog[] | null;
  getByLog(logId: number): View | null;
}

interface ViewInput {
  name: string;
  body: string;
}
```

## add()

Create a view.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `view.name` | `string` | yes | View name, referenced by `<view id="name">` in the page |
| `view.body` | `string` | yes | HTML fragment |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-view-" + stamp

    k.site.views.add({
        name,
        body: `<div class="ai-view">${name}</div>`
    })

    const view = k.site.views.get(name)
    return {
        verified: !!view && view.name === name,
        id: view?.id
    }
})
```

## all()

Return to full view.

**Parameters:** None.

**Returns:** `View[]`.

```ts
k.api.get(() => {
    const views = k.site.views.all()
    return { count: views.length, names: views.slice(0, 10).map((v) => v.name) }
})
```

## get()

Get a view by name or Id.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | View name or ID |

**Returns:** `View | null`.

```ts
k.api.get(() => {
    const view = k.site.views.get("hero")
    return view ? { id: view.id, name: view.name, bodyLength: view.body?.length } : null
})
```

## update()

Update the full view object.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `view` | `View` | yes | Modified full View object |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-view-upd-" + stamp

    k.site.views.add({ name, body: "<div>v1</div>" })
    const view = k.site.views.get(name)
    view.body = "<div>v2</div>"
    k.site.views.update(view)

    const after = k.site.views.get(name)
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
| `nameOrId` | `string` | yes | View name or ID |
| `body` | `string` | yes | New View HTML |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-view-body-" + stamp

    k.site.views.add({ name, body: "<div>before</div>" })
    k.site.views.updateBody(name, "<div>after-body</div>")

    const after = k.site.views.get(name)
    return { verified: after?.body?.indexOf("after-body") >= 0 }
})
```

## delete()

Delete a view by name or Id. Verify with `get(name)` after deletion.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | View name or ID |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-view-del-" + stamp

    k.site.views.add({ name, body: "<div>del</div>" })
    const before = k.site.views.get(name)

    k.site.views.delete(name)
    const after = k.site.views.get(name)

    return {
        verified: !!before && !after,
        hadBefore: !!before
    }
})
```

## getLogs() / getByLog()

| Method | Parameters | Returns | Description |
|------|------|------|------|
| `getLogs(nameOrId)` | `nameOrId: string` | `ChangeLog[] | null` | Gets version logs |
| `getByLog(logId)` | `logId: number` | `View | null` | Gets a historical version by log ID |

```ts
k.api.get(() => {
    const views = k.site.views.all()
    if (!views.length) return { error: "no views" }

    const logs = k.site.views.getLogs(views[0].id)
    return {
        verified: logs === null || Array.isArray(logs),
        logCount: logs ? logs.length : 0
    }
})
```

## Common View Fields

| Field | Type | Description |
|------|------|------|
| ID | `string` | ViewId |
| name | `string` | name |
| body | `string` | HTML content |
| moduleId | `string` | The ID of the module it belongs to (when viewed in the module) |
| extension | `string` | Default `html` |
| online | `boolean` | Is it online? |
| version | `number` | version number |

## Quote in page

```html
<layout id="main">
  <placeholder id="Main">
    <view id="hero"></view>
  </placeholder>
</layout>
```

A view named `hero` needs to exist and `k.site.views.get("hero")` can return objects.

## Related Docs

- [k.site](./)
- [k.site.layouts](./layout.md)
- [k.site.pages](./page.md)
