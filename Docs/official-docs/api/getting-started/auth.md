# Authentication and Authorization

> APIs related to user login, session and access control within the site

## Overview

Operations related to "whether the current user is logged in" in Kooboo are mainly completed through `k.account`; see [k.session](../session/) for request-level sessions, and [k.cookie](../cookie/) for cookies. For encryption and tokens see [k.security](../security/).

## k.account.isLogin

Determine whether the current request has been logged in.

**Parameters**: none.

**Returns**: `boolean`. `true` when the current request is logged in.

```ts
k.api.get("isLogin", () => {
    return { isLogin: k.account.isLogin }
})
```

## k.account.login()

Log in using username and password.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `username` | `string` | Yes | Username. |
| `password` | `string` | Yes | Password. |
| `code` | `string` | No | Verification code, etc. |

**Returns**: `User | null`. User object on success; `null` on failure.

```ts
k.api.post("login", () => {
    const username = k.request.form.username
    const password = k.request.form.password
    k.account.login(username, password)
    return { isLogin: k.account.isLogin }
})
```

## k.account.logout()

Log out, optionally jump to the specified URL.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `returnUrl` | `string` | No | URL to redirect to after logout. |

**Returns**: `void`.

```ts
k.api.get("logout", () => {
    k.account.logout("/")
    return { isLogin: k.account.isLogin }
})
```

## k.account.ensureLogin()

Redirect to the login page when not logged in (commonly used for pages or APIs that require login).

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `redirectUrl` | `string` | Yes | URL to redirect to when the user is not logged in. |

**Returns**: `void`.

```ts
k.api.get("profile", () => {
    k.account.ensureLogin("/login?return=" + encodeURIComponent(k.request.url))
    return k.account.user.current
})
```

## Related Docs

- [k.session](../session/) — Server-side session storage
- [k.security](../security/) — encryption, hashing, etc.
- [Quick Start](./index.md)
