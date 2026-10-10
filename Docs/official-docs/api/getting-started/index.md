# API Getting Started

> Learn how to use KScript APIs inside Kooboo.

## What KScript Is

KScript is Kooboo's server-side scripting layer. It uses JavaScript / TypeScript-like syntax and can be written in API routes, code blocks, and modules.

## Basic Shape

```ts
// GET API
k.api.get(() => {
    return { message: "Hello Kooboo" }
})

// POST API
k.api.post(() => {
    const form = k.request.form
    const body = k.request.body
    return { received: form, body }
})
```

## The Global `k` Object

`k` is the entry point to all built-in Kooboo capabilities:

| Module | Description |
|--------|-------------|
| `k.request` | HTTP request data |
| `k.response` | HTTP response handling |
| `k.session` | Session state |
| `k.DB` | Database access |
| `k.content` | Content management |
| `k.file` | File operations |
| `k.site` | Site resources such as pages, layouts, and menus |
| `k.net` | Network access |
| `k.commerce` | Commerce features |
| `k.payment` | Payment integrations |
| `k.mail` | Email sending |

## Type Hints

When you work in the `kb-doc` project or an IDE, reference `kooboo.d.ts` to get full TypeScript hints:

```ts
/// <reference path="kooboo.d.ts" />
```

## Next Steps

- [k.content](../content/) — structured content CRUD and querying
- [k.site.layouts](../site/layout.md) — manage Layout resources
