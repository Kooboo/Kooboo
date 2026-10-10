# k.cookie

> Cookie management — read and write client cookies

## Overview

`k.cookie` provides create, read, update, and delete operations for cookies. Cookies can store small amounts of client-side data, such as user preferences and simple state-tracking values.

::: tip Cookie vs. session

- **Cookie**: Stored on the client and can have an expiration time
- **Session**: Stored on the server and cleared automatically when the session ends
:::

## TypeScript definition

```ts
interface Cookie {
  keys: string[];        // 所有 Cookie 的键名
  values: string[];      // 所有 Cookie 的值

  set(name: string, value: string, days: number): void;
  set(name: string, value: string, days: number, domain: string): void;
  setByMinutes(name: string, value: string, mins: number): void;
  set(name: string, value: string): void;  // 非空值默认 1 天过期；空值会删除 Cookie

  get(name: string): string | null;
  containsKey(name: string): boolean;
  remove(name: string): boolean;
  remove(name: string, domain: string, sameSite?: boolean): boolean;
  clear(): void;
}
```

## Methods

### set()

Sets a cookie with expiration in days.

The two-argument overload uses a one-day expiration for a non-empty value. Passing an empty or whitespace-only value deletes the cookie instead.

```ts
// 设置 30 天过期的 Cookie
k.api.get(() => {
    k.cookie.set("username", "kooboo", 30)
    return "cookie set"
})

// 设置带域名的 Cookie
k.api.get(() => {
    k.cookie.set("session", "abc123", 7, ".example.com")
    return "cookie with domain set"
})
```


| Parameter | Type | Required | Default | Description |
| -------- | ------ | --- | --- | --------- |
| `name` | string | Yes | - | Cookie name |
| `value` | string | Yes | - | Cookie value |
| `days` | number | Yes | - | Expiration in days |
| `domain` | string | No | Current domain | Configurable domain |


### setByMinutes()

Sets a cookie with expiration in minutes.

```ts
k.api.get(() => {
    // 设置 4 小时过期的 Cookie
    k.cookie.setByMinutes("token", "xyz789", 240)
    return "cookie set for 240 minutes"
})
```


| Parameter | Type | Required | Description |
| ------- | ------ | --- | --------- |
| `name`  | string | Yes | Cookie name |
| `value` | string | Yes | Cookie value |
| `mins`  | number | Yes | Expiration in minutes |


### get()

Gets a cookie value, or `null` when the cookie is not present.

```ts
k.api.get(() => {
    const username = k.cookie.get("username")
    return { username }
})
```

### containsKey()

Checks whether a cookie exists.

```ts
k.api.get(() => {
    const hasToken = k.cookie.containsKey("token")
    return { hasToken }
})
```

### remove()

Deletes the specified cookie.

The overload `remove(name, domain, sameSite?)` deletes a cookie for a specific domain. When `sameSite` is `true`, the runtime also sets `SameSite=None` and `Secure=true` on the deletion response.

```ts
k.api.get(() => {
    const success = k.cookie.remove("username")
    return { removed: success }
})
```

### clear()

Clears all cookies.

```ts
k.api.get(() => {
    k.cookie.clear()
    return "all cookies cleared"
})
```

## Properties

### keys

Gets the list of all Cookie keys.

```ts
k.api.get(() => {
    return k.cookie.keys
})
// 返回: ["_site_culture", "_site_id_", "username"]
```

### values

Gets the list of all Cookie values.

```ts
k.api.get(() => {
    return k.cookie.values
})
// 返回: ["zh-CN", "abc123...", "kooboo"]
```

## Complete examples

### Implement login state with JWT

JWT tokens are commonly stored in a Cookie and used with `k.security.jwt`:

```ts
// 登录 API
k.api.post(() => {
    const { username, password } = JSON.parse(k.request.body || '{}')

    // 验证用户（示例）
    const user = { id: "123", name: username, role: "admin" }

    // 生成 JWT 并存入 Cookie（7天过期）
    const token = k.security.jwt.encode(user)
    k.cookie.set("auth_token", token, 7)

    return { success: true, user }
})

// 验证登录状态 API
k.api.get(() => {
    if (!k.cookie.containsKey("auth_token")) {
        return { loggedIn: false, user: null }
    }

    try {
        const result = JSON.parse(k.security.jwt.decode(k.cookie.get("auth_token")))
        if (result.code === 0) {
            return { loggedIn: true, user: result.value }
        } else {
            return { loggedIn: false, error: result.value }
        }
    } catch (e) {
        return { loggedIn: false, error: "Invalid token" }
    }
})

// 登出 API
k.api.get(() => {
    k.cookie.remove("auth_token")
    return { success: true }
})
```

### Remember user preferences

Cookies are also suitable for storing simple user preferences:

```ts
// 保存偏好
k.api.post(() => {
    const { theme, language } = JSON.parse(k.request.body || '{}')
    k.cookie.set("user_theme", theme, 365)
    k.cookie.set("user_lang", language, 365)
    return { success: true }
})

// 读取偏好
k.api.get(() => {
    return {
        theme: k.cookie.get("user_theme") || "light",
        language: k.cookie.get("user_lang") || "zh-CN"
    }
})
```

## Notes

1. **Size limit**: A single Cookie value is generally no larger than 4KB.
2. **Security**: Do not store passwords or sensitive profile data in Cookies. A JWT stored in a cookie is a bearer credential; protect the surrounding login flow against CSRF/XSS and use an appropriate expiration.
3. **Domain rules**: Cross-domain Cookies require the correct domain setting.
4. **Encoding**: If storing Chinese text, use encodeURIComponent/decodeURIComponent.

## Related documents

- [k.request](./request.md) - HTTP request information
- [k.response](./response.md) - HTTP response handling
