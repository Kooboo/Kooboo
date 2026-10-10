# k.site.pages

> Site page addition, deletion, modification, routing URL and version log

## Overview

`k.site.pages` manages Kooboo **Page** resources (page HTML/layout placeholder, routing, version). Different from global [k.page](../page/): the latter only changes the Meta of the page being rendered in the current request and is not responsible for the site page warehouse.

::: The difference between tip and k.page
| API | Purpose |
|-----|------|
| `k.site.pages` | CRUD, search by URL for all pages in the site |
| `k.page` | Current page `setTitle` / `setMeta` etc. |
:::

## TypeScript Definition

```ts
interface PageRepository {
  add(page: PageInput): void;
  all(): Page[];
  get(nameOrId: string): Page | null;
  getByUrl(url: string): Page | null;
  update(page: Page): void;
  updateBody(nameOrId: string, body: string): void;
  delete(nameOrId: string): void;
  getUrl(id: string): string | null;
  getAbsUrl(id: string): string | null;
  getLogs(nameOrId: string): ChangeLog[] | null;
  getByLog(logId: number): Page | null;
  getUrls(nameOrId: string): Record<string, string>;
  updateRoute(id: string, url: string, culture?: string): void;
}

interface PageInput {
  name: string;
  body: string;
  url: string;
  layoutName?: string;
}
```

## add()

Create a page and register a route. If `url` is not passed, it defaults to `/{name}`.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `page.name` | `string` | yes | Page name (unique) |
| `page.body` | `string` | yes | Page content, which can be a `<layout>` placeholder structure |
| `page.url` | `string` | yes | Access path, such as `/about` |
| `page.layoutName` | `string` | no | Associated layout name |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "doc-check-" + stamp
    const url = "/doc-check-" + stamp

    k.site.pages.add({
        name,
        url,
        body: `<layout id="main"><placeholder id="Main"><h1>${name}</h1></placeholder></layout>`
    })

    const page = k.site.pages.getByUrl(url)
    return {
        created: !!page,
        id: page?.id,
        name: page?.name,
        url: page ? k.site.pages.getUrl(page.id) : null
    }
})
```

## all()

Returns an array of all page objects on the site.

**Parameters:** None.

**Returns:** `Page[]`.

```ts
k.api.get(() => {
    const pages = k.site.pages.all()
    return { count: pages.length, names: pages.slice(0, 5).map((p) => p.name) }
})
```

## get()

Get the page by **Name** or **Id** (GUID string).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Page name or ID |

**Returns:** `Page | null`.

```ts
k.api.get(() => {
    const page = k.site.pages.get("home")
    return page ? { id: page.id, name: page.name } : null
})
```

## getByUrl()

Get the page by **routing URL** (routing aliases will be resolved).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `url` | `string` | yes | Page route URL |

**Returns:** `Page | null`.

```ts
k.api.get(() => {
    const page = k.site.pages.getByUrl("/")
    return page ? { id: page.id, name: page.name } : null
})
```

## update()

Update with complete page object (usually `get` / `getByUrl` first and then change fields).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `page` | `Page` | yes | Modified full page object |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const url = "/doc-upd-" + stamp
    k.site.pages.add({
        name: "doc-upd-" + stamp,
        url,
        body: "<layout id=\"main\"><placeholder id=\"Main\"><h1>v1</h1></placeholder></layout>"
    })

    const page = k.site.pages.getByUrl(url)
    page.body = "<layout id=\"main\"><placeholder id=\"Main\"><h1>v2</h1></placeholder></layout>"
    k.site.pages.update(page)

    const after = k.site.pages.getByUrl(url)
    return {
        verified: after?.body?.indexOf("v2") >= 0,
        snippet: after?.body?.substring(0, 80)
    }
})
```

## updateBody()

Only the `body` field is updated.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Page name or ID |
| `body` | `string` | yes | New page body HTML |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const url = "/doc-body-" + stamp
    k.site.pages.add({
        name: "doc-body-" + stamp,
        url,
        body: "<h1>before</h1>"
    })

    const page = k.site.pages.getByUrl(url)
    k.site.pages.updateBody(page.id, "<h1>after</h1>")

    const after = k.site.pages.getByUrl(url)
    return {
        verified: after?.body?.indexOf("after") >= 0
    }
})
```

## delete()

Delete pages by name or Id.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Page name or ID |

**Returns:** `void`.

::: warning
Please confirm with `get(nameOrId)` after deletion; calling `getByUrl` on the deleted page may trigger a runtime error on some sites due to remaining routing.
:::

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const url = "/doc-del-" + stamp
    const name = "doc-del-" + stamp

    k.site.pages.add({ name, url, body: "<h1>del</h1>" })
    const before = k.site.pages.getByUrl(url)

    k.site.pages.delete(name)
    const after = k.site.pages.get(name)

    return {
        hadPage: !!before,
        deleted: !!before && !after
    }
})
```

## getUrl() / getAbsUrl()

Get a relative path or absolute URL based on the page **Id**.

| Method | Parameters | Returns | Description |
|------|------|------|------|
| `getUrl(id)` | `id: string` | `string | null` | Gets the relative URL |
| `getAbsUrl(id)` | `id: string` | `string | null` | Gets the absolute URL |

```ts
k.api.get(() => {
    const page = k.site.pages.all()[0]
    if (!page) return { error: "no pages" }
    return {
        id: page.id,
        relative: k.site.pages.getUrl(page.id),
        absolute: k.site.pages.getAbsUrl(page.id)
    }
})
```

## getLogs() / getByLog()

`getLogs` returns the change log of the specified page; `getByLog` restores the version object according to the log ID.

| Method | Parameters | Returns | Description |
|------|------|------|------|
| `getLogs(nameOrId)` | `nameOrId: string` | `ChangeLog[] | null` | Gets version logs |
| `getByLog(logId)` | `logId: number` | `Page | null` | Gets a historical version by log ID |

```ts
k.api.get(() => {
    const page = k.site.pages.all()[0]
    if (!page) return { error: "no pages" }
    const logs = k.site.pages.getLogs(page.id)
    return { logCount: logs ? logs.length : 0 }
})
```

## getUrls()

Returns the routing path dictionary of the page in each culture.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Page name or ID |

**Returns:** `Record<string, string>`.

```ts
k.api.get(() => {
    const page = k.site.pages.all()[0]
    if (!page) return { error: "no pages" }
    return k.site.pages.getUrls(page.id)
})
```

## updateRoute()

Update the routing path of the page in the specified culture.

| Parameter | Type | Required | Description |
|------|------|------|------|
| ID | `string` | yes | Page ID |
| url | `string` | yes | New path |
| culture | `string` | no | Culture code; defaults to the site's main culture |

**Returns:** `void`.

```ts
// Update the URL for a specific language in a multilingual site
k.api.post(() => {
    const page = k.site.pages.get("about")
    if (!page) return { error: "page not found" }
    k.site.pages.updateRoute(page.id, "/about-us", "en")
    return k.site.pages.getUrls(page.id)
})
```

## Common Page Fields

| Field | Type | Description |
|------|------|------|
| ID | `string` | PageId |
| name | `string` | name |
| body | `string` | Page body |
| layoutName | `string` | layout name |
| url | — | Get route via `getUrl(id)` |
| online | `boolean` | Is it online? |
| version | `number` | version number |

## Related Docs

- [k.site](./) — Overview of site resources
- [k.site.layouts](./layout.md) — Layout
- [k.page](../page/) — Current page Meta
