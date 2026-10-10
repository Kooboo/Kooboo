# k.cookie

> Cookie Management - Reading and Writing Client Cookies

## Overview

`k.cookie` provides the function of adding, deleting, modifying and checking cookies. Cookies can be used to store a small amount of data on the client, and are suitable for scenarios such as user preferences and simple status tracking.

::: The difference between tip and session
- **Cookie**: stored on the client, expiration time can be set
- **Session**: stored on the server side, automatically cleared after the session ends
:::

## TypeScript definition

```ts
interface Cookie {
  keys: string[];        // All cookie names
  values: string[];      // All cookie values

  set(name: string, value: string, days: number): void;
  set(name: string, value: string, days: number, domain: string): void;
  setByMinutes(name: string, value: string, mins: number): void;
  set(name: string, value: string): void;  // Expires in 1 day by default

  get(name: string): string;
  containsKey(name: string): boolean;
  remove(name: string): boolean;
  clear(): void;
}
```

## Methods

### set()

Set cookie (expires in days).

```ts
// Set a cookie that expires in 30 days
k.api.get(() => {
    k.cookie.set("username", "kooboo", 30)
    return "cookie set"
})

// Set a cookie with a domain
k.api.get(() => {
    k.cookie.set("session", "abc123", 7, ".example.com")
    return "cookie with domain set"
})
```

| Parameter | Type | Required | Default | Description |
|------|------|------|--------|------|
| `name` | string | yes | - | Cookie name |
| `value` | string | yes | - | Cookie value |
| `days` | number | yes | - | Expiration days |
| `domain` | string | no | current domain | Configurable domain name |

**Returns:** `void`.

### setByMinutes()

Set cookie (expires by minutes).

```ts
k.api.get(() => {
    // Set a cookie that expires in 4 hours
    k.cookie.setByMinutes("token", "xyz789", 240)
    return "cookie set for 240 minutes"
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `name` | string | yes | Cookie name |
| `value` | string | yes | Cookie value |
| `mins` | number | yes | Expiration minutes |

**Returns:** `void`.

### get()

Get the cookie value.

```ts
k.api.get(() => {
    const username = k.cookie.get("username")
    return { username }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `name` | string | yes | Cookie name |

**Returns:** `string`, the cookie value; when missing, an empty string or `undefined` depending on the runtime.

### containsKey()

Check if the cookie exists.

```ts
k.api.get(() => {
    const hasToken = k.cookie.containsKey("token")
    return { hasToken }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `name` | string | yes | Cookie name |

**Returns:** `boolean`.

### remove()

Delete the specified cookie.

```ts
k.api.get(() => {
    const success = k.cookie.remove("username")
    return { removed: success }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `name` | string | yes | Cookie name |

**Returns:** `boolean`, whether the cookie was removed successfully.

### clear()

Clear all cookies.

```ts
k.api.get(() => {
    k.cookie.clear()
    return "all cookies cleared"
})
```

**Parameters:** None.

**Returns:** `void`.

## Properties

### keys

Get a list of key names for all cookies.

```ts
k.api.get(() => {
    return k.cookie.keys
})
// Returns: ["_site_culture", "_site_id_", "username"]
```

### values

Get a list of values ​​for all cookies.

```ts
k.api.get(() => {
    return k.cookie.values
})
// Returns: ["zh-CN", "abc123...", "kooboo"]
```

## Complete Example

### User login remember me

```ts
// Login API
k.api.post(() => {
    const { username, password, remember } = JSON.parse(k.request.body || '{}')

    // Validate user...
    const user = { id: "123", name: username }

    if (remember) {
        // Remember me: keep for 30 days
        k.cookie.set("remember_user", JSON.stringify(user), 30)
    } else {
        // Regular login: keep for 1 day
        k.cookie.set("current_user", JSON.stringify(user), 1)
    }

    return { success: true, user }
})

// Check login status API
k.api.get(() => {
    let user = null

    // Check remember-me first
    if (k.cookie.containsKey("remember_user")) {
        const userStr = k.cookie.get("remember_user")
        user = JSON.parse(userStr)
    } else if (k.cookie.containsKey("current_user")) {
        const userStr = k.cookie.get("current_user")
        user = JSON.parse(userStr)
    }

    return { loggedIn: !!user, user }
})

// Logout API
k.api.get(() => {
    k.cookie.remove("remember_user")
    k.cookie.remove("current_user")
    return { success: true }
})
```

## Notes

1. **Size limit**: Cookie single value generally does not exceed 4KB
2. **Security**: Do not store sensitive information in cookies (session is recommended)
3. **Domain name rules**: Cross-domain cookies need to set the correct domain
4. **Encoding**: If you store Chinese, encodeURIComponent/decodeURIComponent is required

## Related Docs

- [k.session](../session/) - Session state management
- [k.request](../request/) - HTTP request information
- [k.response](../response/) - HTTP response handling
