# k.utils.clientJS

> Inject browser-side JavaScript variables into page HTML

## Overview

In the **SSR / page-rendering** flow, write server data into the current response HTML (`<script>`) so the browser can read it through the injected global variable name.

This applies only to the **current page's HTML output**; a pure JSON API response does not need this module.

For multilingual copy and language-switch URLs, use `k.label` / `k.t` and `k.site.multilingual`; see the [multilingual site scenario guide](../../Scenario/multilingual-site-guide.md). This document does not cover `clientJS.i18n`.

## Relevant TypeScript signatures

```ts
interface ClientJs {
  setVariable(key: string, obj: any): void;
  monacoResource(callback: string): string;
}
```

## setVariable()

Write a variable declaration to the current response HTML. `key` is inserted into `var {key} = ...`, so use a safe JavaScript identifier such as `pageContext`; never pass untrusted text, a hyphenated name, quotes, semicolons or a code fragment.

Prefer plain objects, arrays and other server-side data that Kooboo's JSON serializer can handle. This wrapper is not a direct JavaScript `JSON.stringify` call: primitive values use a separate string-conversion path and should not be assumed to preserve every JavaScript type. Avoid data that the serializer cannot handle, including:

- Circular references
- Functions, DOM, and undefined (lost or causing errors during serialization)
- Complex types that the Kooboo runtime cannot serialize

```ts
const payload = {
  siteName: k.site.info.name,
  count: 3,
  tags: ["a", "b"],
};

k.utils.clientJS.setVariable("pageContext", payload);
```

On the browser side, use the injected variable name (`pageContext` in the example).

## monacoResource()

Read the `<script>` / `<link>` fragments required by **Monaco Editor** from the Kooboo server's `_Admin/monaco.html`, combine them into an HTML string, and insert it into the `<head>` of a custom page.

### What it does

1. Parse the `<head>` dependencies in the admin template `monaco.html` (Monaco JS/CSS).
2. For `<script>` paths beginning with `/_Admin/assets/monaco`, automatically add `onload="yourCallback()"` so initialization can run after the Monaco script loads.
3. Return a multiline HTML string (not a URL), which must be output **as-is** into `<head>` in the page template or Layout.

### Typical uses

- Embed a code editor in a **custom Site page** (similar to the admin Code editor experience), rather than calling it from an ordinary business API.
- After the Monaco script loads, create the editor with **`_monaco.instance`** in the `onload` callback (see the next section).

### Global object after loading: `_monaco`

After the script injected by `monacoResource` (for example `/_Admin/assets/monaco.{hash}.js`) executes, the current Kooboo Monaco asset exposes the wrapper object **`_monaco`** globally, **not** the `monaco` global commonly shown in CDN documentation. `monacoResource()` only returns the dependency tags; the `_monaco` object comes from the referenced frontend asset.

| Member | Description |
| --- | --- |
| `_monaco.instance` | Complete Monaco AMD module, including `editor`, `languages`, `Range`, `Uri`, `KeyCode`, and more |
| `_monaco.init` | Kooboo custom initialization (async), with completion options such as `kview` / `kscript` / `classCompletion` |

Create the editor with:

```js
_monaco.instance.editor.create(container, options);
```

**Do not** write `monaco.editor.create(...)` unless you separately load the standard Monaco global yourself.

Optional: put `var monaco = _monaco.instance;` at the start of the callback, then follow the official Monaco documentation.

### Complete example

```html
<div id="editor" style="height: 400px;"></div>

<script env="server">
  k.response.write(k.utils.clientJS.monacoResource("initMonacoEditor"));
</script>
<script>
  function initMonacoEditor() {
    var monaco = _monaco.instance;
    monaco.editor.create(document.getElementById("editor"), {
      value: "// code",
      language: "javascript",
      theme: "vs-dark",
      automaticLayout: true,
    });
  }
</script>
```

### Limitations

- Depends on `_Admin/monaco.html` on the server disk; returns an empty string when it does not exist.
- Intended for **editor pages with Monaco**; ordinary content and list pages do not need to call it.
- **Global name** — `_monaco` after the script loads; access the Monaco API through `_monaco.instance`.

## Notes

1. **Call timing** — `setVariable` writes immediately to the current response. Call it before browser code reads the variable and at the document position where the generated `<script>` should appear.
2. **Monaco callback** — Pass the name of an already-defined function, such as `"initMonacoEditor"`; do not pass `"initMonacoEditor()"` or an HTML/JavaScript fragment because the value is inserted directly into `onload="...()"`.
