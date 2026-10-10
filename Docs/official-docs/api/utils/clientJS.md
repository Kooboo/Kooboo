# k.utils.clientJS

> Output client-side JavaScript variables to the page

## Overview

`k.utils.clientJS` injects `<script>` into the current HTML response and exposes the server object to the browser in the form of `JSON.parse` (must be in the page rendering process).

## setVariable()

```ts
const data = { firstname: "Kooboo", count: 3 }
k.utils.clientJS.setVariable("pageData", data)
// Browser output: var pageData = JSON.parse("...")
```

| parameter | type | illustrate |
|------|------|------|
| `key` | `string` | global variable name |
| `obj` | `any` | JSON serializable objects |

**Returns:** `void`; writes directly to the current HTML response.

## monacoResource()

Returns the resource tag HTML fragment required by the Monaco editor for use by callback `onload`.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `callback` | `string` | Yes | Global function name called after the Monaco main script loads |

**Returns:** `string`, an HTML fragment containing `<script>` / `<link>` tags.

```ts
const resources = k.utils.clientJS.monacoResource("initEditor")
k.response.write(resources)
k.response.write(`
<div id="editor"></div>
<script>
function initEditor() {
    monaco.editor.create(document.getElementById("editor"), {
        value: "console.log('Kooboo')",
        language: "javascript"
    })
}
</script>
`)
```

## i18n()

Returns the current site multi-language configuration object.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `all` | `boolean` | No | Whether to return all language configuration |

**Returns:** `any`, the current site i18n configuration object.

```ts
const cfg = k.utils.clientJS.i18n()
const all = k.utils.clientJS.i18n(true)
```

## Related Docs

- [k.label](../label/) — multilingual tags
