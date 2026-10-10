# Development Resources - Open API

## Page Purpose

- Admin list: `/development/openapis`; definition editor: `/development/openapi/edit`; authorization page: `/development/openapi/authorizes`.
- This page saves an external service's OpenAPI description as a site resource so KScript can call remote APIs according to the described operations.
- It manages remote API client definitions, not Swagger documentation for the Kooboo site's own APIs, and does not create public routes for the site.

## Document Sources and Addresses

- At creation, an OpenAPI document can be downloaded from a URL, JSON can be pasted directly, or a remote template can be selected. On save, it is parsed and normalized as an OpenAPI 3 document; an invalid document or inaccessible URL makes saving fail.
- After creation, the current editor does not allow the name or source type to be changed directly. Editing focuses on the current definition's address, Base URL, cache, and custom request code.
- A URL type downloads the document once when saved. Update Document in the list downloads again from the original URL and overwrites the saved description. Later changes in the remote service are not synchronized without an update operation.
- Base URL takes priority as the actual call address. When empty, the system tries the server address in the document. If the document provides only a relative server address, the URL source also participates in completion.
- A successful save proves only that the definition was saved and parsed; it does not prove the remote address is reachable, parameters are correct, credentials are valid, or the business call succeeds.

## Authorization and Custom Requests

- The Authorization page maintains named credentials from the document's security schemes, supporting common Basic, Bearer, API Key, and OAuth2 data. A matching authorization is required to call a protected operation.
- An OAuth2 challenge opens an external authorization page and exchanges and saves a token through a site callback. Opening the authorization page does not mean authorization succeeded; the callback address, client configuration, scope, and token endpoint must all match the remote service.
- Credentials contain passwords, client secrets, and tokens and must be managed as sensitive information. Deleting an authorization invalidates calls using its name and does not automatically replace it with another correct credential set.
- Custom KScript can be enabled to modify the request body, query parameters, path parameters, headers, or Cookie before sending. A successful code save does not mean every operation is safe or correct; erroneous code can affect every call using the definition.
- Saving at the bottom of the editor persists the entire OpenAPI definition and returns to the list. The leave-page prompt is not a remote-document backup and does not replace real-call validation after saving.

## Cache and Call Boundaries

- Cache rules are selected by request method and path containment and expire in minutes. A cache hit may return an old result from the site process without requesting the remote service again.
- A cache key mainly consists of the final URL and authorization name. For operations whose response strongly depends on request body, headers, or Cookie, do not assume a cache result is safely reusable only because the URL is the same.
- Cache does not make remote writes more reliable and cannot replace idempotency, permissions, or error handling by the remote service.
- Calls can still fail because of network conditions, remote status codes, document/service mismatch, missing parameters, or expired tokens. Acceptance must execute the target operation and verify the remote business result rather than only checking that the OpenAPI definition exists.

## Deletion and Change Risks

- Deleting an OpenAPI definition also deletes authorization records maintained here, but does not automatically repair calls in KScript, pages, or jobs.
- Updating the document may change operation names, parameter order, security schemes, or server addresses. Confirm callers before updating and reverify key reads and writes afterward.
