# Response helpers

Installs one ordinary Module CodeBlock: `Utils.response`. No dependencies or runtime hooks.

```ts
import { successResponse, errorResponse } from "./Utils.response"

k.api.get(() => {
    const id = k.request.get("id")
    if (!id) {
        return errorResponse(400, "INVALID_ARGUMENT", "Request ID is required")
    }
    return successResponse({ id })
})
```

- `successResponse(data = null, message = "", httpStatus = 200)` returns `k.api.httpCode(httpStatus, { success: true, code: "OK", message, data })`.
- `errorResponse(httpStatus, code, message, data = null)` returns `k.api.httpCode(httpStatus, { success: false, code, message, data })`.
- API handlers should return these helpers directly. The first `errorResponse` argument is the HTTP status; `code` is the business code.
- Prefer these helpers for new JSON business APIs when this template is selected. Preserve existing API contracts and custom formats; downloads and redirects do not need a wrapper.
- Installation does not modify existing App Base bootstrap/notification APIs or frontend request utilities. Adapt response parsing in the App's own service when consuming this format.
- Read the installed CodeBlock before relying on its contract in an existing App: the owner can customize it. Replace or remove it by changing its explicit import/call sites; Kooboo does not enforce the envelope.

The source is TypeScript (`files/Utils.response.ts`). Installation preserves its type annotations and sets the CodeBlock extension to `.ts`; Kooboo transpiles it at runtime.
