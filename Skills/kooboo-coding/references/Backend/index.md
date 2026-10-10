---
name: kooboo-coding-backend
description: Use when implementing Kooboo backend API/Service/Data layer tasks.
---

# Backend Task Guide

For file upload, import, generation, replacement or deletion, follow the saving and cleanup contract in [Images and file management](../Frontend/images-and-files.md), reusing existing file services first.

## Three-layer architecture

Backend tasks are divided into three layers, each independent with a clearly defined responsibility:

| Layer | Code object | Name convention | Description |
|------|----------|-----------|------|
| API layer | Code(CodeType=Api) | `product` | Defines the public interface and imports Services |
| Service layer | Code(CodeType=CodeBlock) | `Services.xxx` / `Utils.xxx` | Business logic; can be imported by an API or another Service |
| Data layer | Conceptual layer | — | Self-checks data-source routing and decides where data is read from and written to |

Use dot-separated API names such as `app.user`. Prefer one `Services.xxx` per business module including data access, and one shared `ENV` with settings grouped by module. Split only when complexity warrants it; no mandatory Models layer. Package manifests reference `database.json` for explicit storage preparation before real use; requests do not initialize schema.

## Execution flow

Read [App Base user and notification services](app-base-services.md) for the installed working reference, dynamic routes, parameter handling and server mock removal.

For optional backend capabilities, read [Optional backend templates](templates.md): discover the catalog and install selected application-owned CodeBlocks and APIs. New App Bases select response and notifications by default; an explicit selection may omit either. Existing API contracts remain authoritative.

1. **Determine the layer**: first identify whether this is an API-layer or Service-layer task.
2. **Load the guide**: read the corresponding layer guide.
3. **Before calling a Function**: **you must first read [Backend object creation](../Functions/backend-create.md)** to confirm the create_api / create_codeblock choice and parameter format.
4. **Self-check**: use the [API-layer checklist](api-layer-checklist.md) before writing code.

::: warning Important
**You must load [Backend object creation](../Functions/backend-create.md) before calling create_api / create_codeblock**; otherwise the Name format or creation Function choice may be wrong.
:::

## How an API calls a Service (do not invent syntax)

- **Correct**: at the top of the API KScript, write `import { getCurrentAppUser } from "./Services.user"`, then call `getCurrentAppUser()` directly inside the `k.api.get/post/...` callback. Functions exposed by a Service CodeBlock must use `export function`.
- **Incorrect**: `k.req.services.getCurrentAppUser()`, `k.request.services.xxx()`, or any pattern that attaches a Service to request and calls it by method name. **Kooboo does not provide** APIs such as `k.req.services` / `k.request.services`; the model must not carry this habit over from another Web framework.
- Use documented APIs such as `k.request` / `k.api` for routes and parameters; cross-file business logic always uses **import + call an exported function**. See “Calling a Service” in the [API-layer guide](api-layer-guide.md).

## Module navigation

See the references for detailed guides:

- [API-layer guide](api-layer-guide.md) — Route design, parameter access, and response formats
- [Service-layer guide](service-layer-guide.md) — Business logic, import patterns, and error handling
- [Data-layer self-check](data-layer-guide.md) — Data-source selection and three-layer collaboration
- [Backend object creation](../Functions/backend-create.md) — **create_api / create_codeblock choice and parameters** (required)
- [API-layer checklist](api-layer-checklist.md) — Pre-writing self-check
