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
  set(name: string, value: string): void;  // 默认 1 天过期

  get(name: string): string;
  containsKey(name: string): boolean;
  remove(name: string): boolean;
  clear(): void;
}
```

## 方法

### set()

设置 Cookie（按天过期）。

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

| 参数 | 类型 | 必填 | 默认值 | 描述 |
|------|------|------|--------|------|
| `name` | string | 是 | - | Cookie 名称 |
| `value` | string | 是 | - | Cookie 值 |
| `days` | number | 是 | - | 过期天数 |
| `domain` | string | 否 | 当前域 | 可设置的域名 |

**返回：** `void`。

### setByMinutes()

设置 Cookie（按分钟过期）。

```ts
k.api.get(() => {
    // 设置 4 小时过期的 Cookie
    k.cookie.setByMinutes("token", "xyz789", 240)
    return "cookie set for 240 minutes"
})
```

| 参数 | 类型 | 必填 | 描述 |
|------|------|------|------|
| `name` | string | 是 | Cookie 名称 |
| `value` | string | 是 | Cookie 值 |
| `mins` | number | 是 | 过期分钟数 |

**返回：** `void`。

### get()

获取 Cookie 值。

```ts
k.api.get(() => {
    const username = k.cookie.get("username")
    return { username }
})
```

| 参数 | 类型 | 必填 | 描述 |
|------|------|------|------|
| `name` | string | 是 | Cookie 名称 |

**返回：** `string`，Cookie 值；不存在时为空字符串或 `undefined`（以运行时为准）。

### containsKey()

检查 Cookie 是否存在。

```ts
k.api.get(() => {
    const hasToken = k.cookie.containsKey("token")
    return { hasToken }
})
```

| 参数 | 类型 | 必填 | 描述 |
|------|------|------|------|
| `name` | string | 是 | Cookie 名称 |

**返回：** `boolean`。

### remove()

删除指定 Cookie。

```ts
k.api.get(() => {
    const success = k.cookie.remove("username")
    return { removed: success }
})
```

| 参数 | 类型 | 必填 | 描述 |
|------|------|------|------|
| `name` | string | 是 | Cookie 名称 |

**返回：** `boolean`，表示是否删除成功。

### clear()

清除所有 Cookie。

```ts
k.api.get(() => {
    k.cookie.clear()
    return "all cookies cleared"
})
```

**参数：** 无。

**返回：** `void`。

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

### 用户登录记住我

```ts
// 登录 API
k.api.post(() => {
    const { username, password, remember } = JSON.parse(k.request.body || '{}')

    // 验证用户...
    const user = { id: "123", name: username }

    if (remember) {
        // 记住我：保存 30 天
        k.cookie.set("remember_user", JSON.stringify(user), 30)
    } else {
        // 普通登录：保存 1 天
        k.cookie.set("current_user", JSON.stringify(user), 1)
    }

    return { success: true, user }
})

// 检查登录状态 API
k.api.get(() => {
    let user = null

    // 优先检查记住我
    if (k.cookie.containsKey("remember_user")) {
        const userStr = k.cookie.get("remember_user")
        user = JSON.parse(userStr)
    } else if (k.cookie.containsKey("current_user")) {
        const userStr = k.cookie.get("current_user")
        user = JSON.parse(userStr)
    }

    return { loggedIn: !!user, user }
})

// 登出 API
k.api.get(() => {
    k.cookie.remove("remember_user")
    k.cookie.remove("current_user")
    return { success: true }
})
```

## 注意事项

1. **大小限制**: Cookie 单个值一般不超过 4KB
2. **安全性**: 不要在 Cookie 中存储敏感信息（建议使用 session）
3. **域名规则**: 跨域 Cookie 需要设置正确的 domain
4. **编码**: 如果存储中文，需要 encodeURIComponent/decodeURIComponent

## 相关文档

- [k.session](../session/) - 会话状态管理
- [k.request](../request/) - HTTP 请求信息
- [k.response](../response/) - HTTP 响应处理
