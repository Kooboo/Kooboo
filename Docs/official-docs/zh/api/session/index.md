# k.session

> 会话级缓存 - 在服务器端存储会话数据

## 概述

`k.session` 提供会话级缓存功能，数据存储在服务器端，在同一个会话期间可以跨请求访问。

::: tip 与 Cookie 的区别
- **Session**: 存储在服务器端，适合存储大量数据或敏感信息
- **Cookie**: 存储在客户端，适合存储少量简单数据
:::

## 生命周期

| 配置 | 值 |
|------|-----|
| Cookie 名称 | `_kb_session_id` |
| Cookie 过期 | 7 天 |
| 服务端过期 | **30 分钟无活动**后自动清除 |

**说明**：
- 浏览器关闭不会清除 session（Cookie 7 天有效）
- 如果 30 分钟内没有任何操作，服务端数据会被清除
- 再次访问时会创建新的 session

## TypeScript 定义

```ts
interface Session {
  keys: string[];       // 所有键名
  values: any[];        // 所有值
  length: number;       // 键值对数量

  set(key: string, value: any): void;      // 设置值
  get(key: string): any;                    // 获取值
  containsKey(key: string): boolean;        // 检查键是否存在
  remove(key: string): boolean;              // 删除键
  clear(): void;                            // 清空所有
}
```

## 方法

### set()

设置会话值。

```ts
k.api.get(() => {
    k.session.set("username", "kooboo")
    k.session.set("role", "admin")
    return "session set"
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 会话键名 |
| `value` | `any` | 是 | 要保存的值 |

**返回：** `void`。

### get()

获取会话值。

```ts
k.api.get(() => {
    const username = k.session.get("username")
    return { username }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 会话键名 |

**返回：** `any`，键对应的会话值；不存在时为空或 `undefined`（以运行时为准）。

### containsKey()

检查键是否存在。

```ts
k.api.get(() => {
    k.session.set("token", "abc123")
    const hasToken = k.session.containsKey("token")
    return { hasToken }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 会话键名 |

**返回：** `boolean`。

### remove()

删除指定键。

```ts
k.api.get(() => {
    k.session.set("temp", "data")
    const removed = k.session.remove("temp")
    return { removed, stillExists: k.session.containsKey("temp") }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 会话键名 |

**返回：** `boolean`，表示是否删除成功。

### clear()

清空所有会话数据。

```ts
k.api.get(() => {
    k.session.set("key1", "value1")
    k.session.clear()
    return "cleared"
})
```

**参数：** 无。

**返回：** `void`。

## 属性

### keys

获取所有键名列表。

```ts
k.api.get(() => {
    k.session.set("key1", "value1")
    k.session.set("key2", "value2")
    return { keys: k.session.keys }
})
// 返回: { "keys": ["key1", "key2"] }
```

### values

获取所有值列表。

```ts
k.api.get(() => {
    k.session.set("key1", "value1")
    k.session.set("key2", "value2")
    return { values: k.session.values }
})
// 返回: { "values": ["value1", "value2"] }
```

### length

获取键值对数量。

```ts
k.api.get(() => {
    k.session.set("key1", "value1")
    k.session.set("key2", "value2")
    return { length: k.session.length }
})
// 返回: { "length": 2 }
```

## 完整示例

### 用户登录状态

```ts
// 登录
k.api.post(() => {
    const { username, password } = JSON.parse(k.request.body)

    // 验证用户...
    const user = { id: "123", name: username, role: "admin" }

    // 存储会话
    k.session.set("user", JSON.stringify(user))

    return { success: true, user }
})

// 检查登录状态
k.api.get(() => {
    const userStr = k.session.get("user")
    if (userStr) {
        const user = JSON.parse(userStr)
        return { loggedIn: true, user }
    }
    return { loggedIn: false, user: null }
})

// 登出
k.api.get(() => {
    k.session.remove("user")
    return { success: true }
})
```

## 相关文档

- [k.cookie](../cookie/) - Cookie 管理
- [k.request](../request/) - HTTP 请求信息
