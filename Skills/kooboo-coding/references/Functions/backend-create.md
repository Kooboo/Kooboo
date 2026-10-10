# Backend Object Creation

Before creating, check for an object with the same name; use [Read and update](read-update.md) for an existing object.

For bundled helpers, use `install_backend_templates` rather than recreating their source. See [Optional backend templates](../Backend/templates.md) for selection, App Base defaults and conflict handling.

## `create_api`

| Field | Description |
|---|---|
| `name` | Concise lowercase API identifier |
| `url` | API route beginning with `/` |
| `content` | KScript content |

## `create_codeblock`

| Field | Description |
|---|---|
| `name` | Module name such as `Services.xxx` or `Utils.xxx` |
| `content` | KScript content |

Services must use CodeBlock; do not replace them with APIs. When an API calls a Service, use `import { fn } from "./Services.xxx"` or `"./Utils.xxx"` at the top of the file, and the corresponding function must `export`.

Before implementation, continue reading the [Backend entry](../Backend/index.md) and the KScript modules actually used; do not invent a Service-call pattern from a request object. After creation, read it back and run Preflight and runtime validation for the current stage.
