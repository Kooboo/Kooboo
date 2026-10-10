# Open API

> Menu: **Development → Open API**  
> Deep link: `/_Admin/development/openapis?SiteId={siteGuid}`

Import external **OpenAPI (Swagger) documents** into the site, configure auth credentials and response cache, and call remote HTTP APIs in KScript via **`k.openApi.{name}`**. Unlike Swagger generated for this site in [Code](./code.md) (`codeOpenApi`): this feature aggregates **third-party APIs**.

::: tip Permissions
List: `openApi` (view). **Create**, edit definitions and authorizations, **Update document** (URL type) require `openApi·edit`; multi-select **Delete** requires `openApi·delete`.
:::

::: info vs k.net
| Approach | Use when |
|------|------|
| **Open API** (this page) | Existing OpenAPI spec, OAuth/API Key, call by operation name, cacheable |
| **[k.net.httpClient](../../api/net/httpclient.md)** | Hand-written URL, headers, no spec document |
:::

## How to Open

1. In [Edit Menu](../navigation.md#edit-menu), enable **Development → Open API**.
2. Open **Development → Open API** in the left sidebar.

<DocImage src="/cms/development/openapis-overview.png" alt="Open API list overview" width="1120" />

## Definition list

### Toolbar

| Button | Permission | Description |
|------|------|------|
| **Create** | `openApi·edit` | Enter [edit Open API](#edit-open-api) (new) |

<DocImage src="/cms/development/openapis-toolbar.png" alt="Open API toolbar" width="1120" />

### List table

| Column | Description |
|----|------|
| **Name** | Unique id; cannot rename after save; maps to `k.openApi.{name}` in scripts |
| **Type** | `url` / `code` / `template` (see [edit page types](#types-selectable-on-create)) |
| **URL** | Spec source address or template detail link |
| **Last modified** | Save time |
| **Actions** | See table below |

| Icon | Condition | Description |
|------|------|------|
| **Update document** | Type `url` | Re-download OpenAPI JSON from **URL** and write to site |
| **Authorize** | Always | Enter [authorization config](#authorization-config) |
| **Edit** | Always | Enter [edit Open API](#edit-open-api) |

Supports multi-select **Delete** (`openApi·delete`); also deletes all **authorization** records for that definition.

<DocImage src="/cms/development/openapis-list-table.png" alt="Open API list" width="1120" />

## Edit Open API

Path: `/_Admin/development/openapi/edit?SiteId=...` (new without `id`; edit with `id={OpenApiId}`)

Footer **Save** / **Return**; unsaved leave prompts. New save returns to list.

<DocImage src="/cms/development/openapis-edit.png" alt="Edit Open API" width="1120" />

### Common fields

| Field | Description |
|------|------|
| **Name** | Required, unique, 1–50 chars, letter or digit first; **read-only when editing** |
| **Base URL** | Optional; overrides `servers` base in spec (absolute HTTP(S) URL) |
| **Cache** | Cache responses by **HTTP method** + **path pattern** — [response cache](#response-cache) |
| **Use comma array** | Serialize array query params as comma-separated single value (else duplicate keys) |
| **Custom authentication** | Toggle shows KScript editor; inject header/query/cookie before request |

**Custom authentication** script can use global `request` (read-only structure injected at runtime): `body`, `querys`, `paths`, `headers`, `cookies`. Example (default editor comment):

```typescript
// request.headers.Authorization = 'bearer xxxx'
// request.querys.token = 'xxxx'
```

### Types (selectable on create)

**Type cannot change** after create.

| Type | Description |
|------|------|
| **URL** | OpenAPI JSON **URL** (e.g. `https://petstore3.swagger.io/api/v3/openapi.json`); downloads and normalizes JSON on save |
| **Code** | Paste **OpenAPI JSON** body in Monaco (`jsonData`) |
| **Template** | Pick from Kooboo **template library** (`openapi_template.kooboo.net`); auto-fills `baseUrl`, `authUrl`, document, optional sample code |

<DocImage src="/cms/development/openapis-edit-url.png" alt="Type URL: spec address" width="1120" />

<DocImage src="/cms/development/openapis-edit-code.png" alt="Type code: JSON body" width="1120" />

### Response cache

Each cache rule:

| Field | Description |
|------|------|
| **Method** | `GET` / `POST` / … / `ALL` |
| **Path pattern** | Fragment matching spec paths (`Contains` match) |
| **Expiry** | Minutes |

On cache hit, returns last response from memory until expiry.

<DocImage src="/cms/development/openapis-edit-cache.png" alt="Response cache rules" width="1120" />

## Authorization config

Path: `/_Admin/development/openapi/authorizes?SiteId=...&id={OpenApiId}`

From list **Authorize** (lock icon). Breadcrumb: **OpenApis → Authorizes**.

Maintain multiple **authorization names** per Open API definition (e.g. `production`, `sandbox`), each storing credentials required by spec `securitySchemes`. Scripts must configure here (or via `k.openApi.{name}.authorize`) before calling remote APIs, else **Api not authorize**.

### Toolbar

| Button | Description |
|------|------|
| **Create** | New authorization record |

### List

| Column | Description |
|----|------|
| **Name** | `authorizeName`; which credential set the script uses |
| **Last modified** | Save time |
| **Actions** | **Challenge** (OAuth one-click redirect), **Edit** |

<DocImage src="/cms/development/openapis-authorizes.png" alt="Authorization list" width="1120" />

### New/edit authorization dialog

| Field | Description |
|------|------|
| **Name** | Authorization config name (required on create, unique) |
| **Security scheme** | Dynamic form from OpenAPI `components.securitySchemes` |

Common credential fields (by scheme type):

| Scheme | Fields |
|------|------|
| **HTTP Basic** | `username`, `password` |
| **Bearer / API Key** | `accessToken`, `name` (e.g. `Bearer` prefix) |
| **OAuth2** | `clientId`, `clientSecret`, `accessToken`, `refreshToken`, `expiresIn`, etc. |

**Challenge**: for definitions with `authUrl` or OAuth2 `authorizationCode` flow, opens browser for authorization; callback handled by Kooboo `/_api/openapioauth2callback/...` or `/_api/OpenApi/SaveToken` and writes tokens back.

<DocImage src="/cms/development/openapis-authorize-dialog.png" alt="Edit authorization dialog" width="1120" />

## Use in the site

1. **Create** Open API here and save (ensure valid `jsonData`).
2. Add at least one credential set in [authorization config](#authorization-config).
3. Call from server scripts in Layout / Page / Code, for example:

```javascript
// Name matches admin Name column; operation names in k.openApi API docs
const result = k.openApi.myService.somePath_get({ query: { page: 1 } })
```

Operation names and parameter shapes match IDE `OpenApi` types for the site — see **[k.openApi](../../api/openapi/index.md)**.

## Related

| Doc | Description |
|------|------|
| [k.openApi](../../api/openapi/index.md) | KScript: call operations, `authorize` credentials |
| [k.net.httpClient](../../api/net/httpclient.md) | Generic HTTP client |
| [k.security.jwt](../../api/security/index.md) | JWT (with Bearer in spec) |
| [Code](./code.md) | This site's API / CodeBlock |
| [Authentication](./authentication.md) | Inbound request auth (not external API calls) |
| [Development overview](./index.md) | Development group index |
