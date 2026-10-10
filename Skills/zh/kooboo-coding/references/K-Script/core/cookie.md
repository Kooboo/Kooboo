# k.cookie

> Cookie 管理 - 读写客户端 Cookie

## 概述

`k.cookie` 提供 Cookie 的增删改查功能。Cookie 可用于在客户端存储少量数据，适合用户偏好设置、简单的状态跟踪等场景。

::: tip 与 session 的区别

- **Cookie**: 存储在客户端，可设置过期时间
- **Session**: 存储在服务器端，会话结束后自动清除
:::

## TypeScript 定义

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

## 方法

### set()

设置 Cookie（按天过期）。

两参数重载对非空值默认设置 1 天过期；传入空字符串或仅包含空白的值时会改为删除 Cookie。

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


| 参数       | 类型     | 必填  | 默认值 | 描述        |
| -------- | ------ | --- | --- | --------- |
| `name`   | string | 是   | -   | Cookie 名称 |
| `value`  | string | 是   | -   | Cookie 值  |
| `days`   | number | 是   | -   | 过期天数      |
| `domain` | string | 否   | 当前域 | 可设置的域名    |


### setByMinutes()

设置 Cookie（按分钟过期）。

```ts
k.api.get(() => {
    // 设置 4 小时过期的 Cookie
    k.cookie.setByMinutes("token", "xyz789", 240)
    return "cookie set for 240 minutes"
})
```


| 参数      | 类型     | 必填  | 描述        |
| ------- | ------ | --- | --------- |
| `name`  | string | 是   | Cookie 名称 |
| `value` | string | 是   | Cookie 值  |
| `mins`  | number | 是   | 过期分钟数     |


### get()

获取 Cookie 值；Cookie 不存在时返回 `null`。

```ts
k.api.get(() => {
    const username = k.cookie.get("username")
    return { username }
})
```

### containsKey()

检查 Cookie 是否存在。

```ts
k.api.get(() => {
    const hasToken = k.cookie.containsKey("token")
    return { hasToken }
})
```

### remove()

删除指定 Cookie。

`remove(name, domain, sameSite?)` 重载可删除指定域名下的 Cookie。当 `sameSite` 为 `true` 时，运行时还会在删除响应上设置 `SameSite=None` 和 `Secure=true`。

```ts
k.api.get(() => {
    const success = k.cookie.remove("username")
    return { removed: success }
})
```

### clear()

清除所有 Cookie。

```ts
k.api.get(() => {
    k.cookie.clear()
    return "all cookies cleared"
})
```

## 属性

### keys

获取所有 Cookie 的键名列表。

```ts
k.api.get(() => {
    return k.cookie.keys
})
// 返回: ["_site_culture", "_site_id_", "username"]
```

### values

获取所有 Cookie 的值列表。

```ts
k.api.get(() => {
    return k.cookie.values
})
// 返回: ["zh-CN", "abc123...", "kooboo"]
```

## 完整示例

### 使用 JWT 实现登录状态

JWT token 通常存储在 Cookie 中，配合 `k.security.jwt` 使用：

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

### 记住用户偏好设置

Cookie 也适合存储简单的用户偏好：

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

## 注意事项

1. **大小限制**: Cookie 单个值一般不超过 4KB
2. **安全性**: 不要在 Cookie 中存储密码或敏感用户资料。存储在 Cookie 中的 JWT 属于 bearer 凭证，应防护登录流程的 CSRF/XSS 风险并设置合适的过期时间。
3. **域名规则**: 跨域 Cookie 需要设置正确的 domain
4. **编码**: 如果存储中文，需要 encodeURIComponent/decodeURIComponent

## 相关文档

- [k.request](request.md) - HTTP 请求信息
- [k.response](response.md) - HTTP 响应处理
