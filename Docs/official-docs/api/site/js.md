# k.site.scripts

> Addition, deletion, modification, routing URL and version log of site JavaScript resources

## Overview

`k.site.scripts` Manages **Script** resources (stand-alone JS files) in the site. Scripts are externally accessed through routed URLs (such as `/app.js`), which can be referenced in pages or layouts.

::: The difference between tip and layouts / views
`k.site.scripts` supports **`getByUrl`**, **`getUrl`**, **`getAbsUrl`** and other routing-related APIs (similar to [k.site.pages](./page.md)); [k.site.layouts](./layout.md) and [k.site.views](./view.md) have no routing API.
:::

The sidebar entry "js" is this API.

## TypeScript Definition

```ts
interface ScriptRepository {
  add(script: ScriptInput): void;
  all(): Script[];
  get(nameOrId: string): Script | null;
  getByUrl(url: string): Script | null;
  update(script: Script): void;
  updateBody(nameOrId: string, body: string): void;
  delete(nameOrId: string): void;
  getUrl(id: string): string | null;
  getAbsUrl(id: string): string | null;
  getLogs(nameOrId: string): ChangeLog[] | null;
  getByLog(logId: number): Script | null;
}

interface ScriptInput {
  name: string;
  body: string;
  url?: string;
  async?: boolean;
  defer?: boolean;
}
```

## add()

Create the script and register the route. If `url` is not passed, it defaults to `/{name}`, and if the path does not contain an extension, the script extension (usually `.js`) will be added automatically.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `script.name` | `string` | yes | Script name (unique) |
| `script.body` | `string` | yes | JavaScript content |
| `script.url` | `string` | no | Access path, such as `/assets/app.js` |
| `script.async` | `boolean` | no | Whether to load with `async` |
| `script.defer` | `boolean` | no | Whether to load with `defer` |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-script-" + stamp
    const url = "/ai-script-" + stamp + ".js"

    k.site.scripts.add({
        name,
        url,
        body: `console.log("ai-script-${stamp}");`
    })

    const script = k.site.scripts.getByUrl(url)
    return {
        verified: !!script && script.name === name,
        id: script?.id,
        route: script ? k.site.scripts.getUrl(script.id) : null
    }
})
```

## all()

Return all scripts.

**Parameters:** None.

**Returns:** `Script[]`.

```ts
k.api.get(() => {
    const scripts = k.site.scripts.all()
    return { count: scripts.length, names: scripts.slice(0, 10).map((s) => s.name) }
})
```

## get()

Get the script by name or Id.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Script name or ID |

**Returns:** `Script | null`.

```ts
k.api.get(() => {
    const script = k.site.scripts.get("main")
    return script ? { id: script.id, name: script.name, extension: script.extension } : null
})
```

## getByUrl()

Get the script by **routing URL** (e.g. `/main.js`).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `url` | `string` | yes | Script route URL |

**Returns:** `Script | null`.

```ts
k.api.get(() => {
    const script = k.site.scripts.getByUrl("/main.js")
    return script ? { id: script.id, name: script.name } : null
})
```

## update()

Update the complete script object.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `script` | `Script` | yes | Modified full Script object |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-script-upd-" + stamp
    const url = "/ai-script-upd-" + stamp + ".js"

    k.site.scripts.add({ name, url, body: "// v1" })
    const script = k.site.scripts.getByUrl(url)
    script.body = "// v2"
    k.site.scripts.update(script)

    const after = k.site.scripts.getByUrl(url)
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
| `nameOrId` | `string` | yes | Script name or ID |
| `body` | `string` | yes | New JavaScript content |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-script-body-" + stamp
    const url = "/ai-script-body-" + stamp + ".js"

    k.site.scripts.add({ name, url, body: "// before" })
    k.site.scripts.updateBody(name, "// after-body")

    const after = k.site.scripts.getByUrl(url)
    return { verified: after?.body?.indexOf("after-body") >= 0 }
})
```

## delete()

Delete by Name or Id. Verify with `get(name)` after deletion.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `nameOrId` | `string` | yes | Script name or ID |

**Returns:** `void`.

```ts
k.api.post(() => {
    const stamp = Date.now().toString()
    const name = "ai-script-del-" + stamp
    const url = "/ai-script-del-" + stamp + ".js"

    k.site.scripts.add({ name, url, body: "// del" })
    const before = k.site.scripts.getByUrl(url)

    k.site.scripts.delete(name)
    const after = k.site.scripts.get(name)

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
    const scripts = k.site.scripts.all()
    for (const script of scripts) {
        const rel = k.site.scripts.getUrl(script.id)
        if (!rel) continue
        const abs = k.site.scripts.getAbsUrl(script.id)
        return { relative: rel, absolute: abs }
    }
    return null
})
```

Only scripts with registered routes can return URLs.

## getLogs() / getByLog()

| Method | Parameters | Returns | Description |
|------|------|------|------|
| `getLogs(nameOrId)` | `nameOrId: string` | `ChangeLog[] | null` | Gets version logs |
| `getByLog(logId)` | `logId: number` | `Script | null` | Gets a historical version by log ID |

```ts
k.api.get(() => {
    const scripts = k.site.scripts.all()
    if (!scripts.length) return { error: "no scripts" }

    const logs = k.site.scripts.getLogs(scripts[0].id)
    return {
        verified: logs === null || Array.isArray(logs),
        logCount: logs ? logs.length : 0
    }
})
```

## Common Script Fields

| Field | Type | Description |
|------|------|------|
| ID | `string` | ScriptId |
| name | `string` | name |
| body | `string` | JS content |
| extension | `string` | extension, usually `js` |
| async | `boolean` | Is async |
| defer | `boolean` | whether to defer |
| crossOrigin | `string` | CORS attributes |
| online | `boolean` | Is it online? |
| version | `number` | version number |

## Related Docs

- [k.site](./)
- [k.site.codes](./code.md) — API and CodeBlock (`codeType`)
- [k.site.styles](./css.md)
- [k.site.pages](./page.md)
