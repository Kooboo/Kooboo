# k.cache

> 站点级内存缓存，在同一站点内跨请求共享

## 概述

`k.cache` 将键值缓存在服务器内存中，按**当前站点**隔离（内部按站点 Id 分组）。适合缓存 API 计算结果、外部请求响应等，减少重复计算。

::: tip 与 k.session 的区别
| API | 存储 | 典型用途 |
|-----|------|----------|
| **`k.cache`** | 站点内存缓存，可设过期秒数 | 热点数据、计算结果、全站共享 |
| **`k.session`** | 会话存储，绑定访客会话 | 购物车、登录前临时状态 |
:::

未命中时 `get` 返回 **`undefined`**（与 `containsKey` 配合使用）。

## TypeScript 定义（节选）

```ts
interface KCache {
  readonly group: string;
  set(key: string, value: any, seconds: number): void;
  set(key: string, value: any): void;
  set(key: string, value: any, options: CacheOptions): void;
  get(key: string): any;
  getOrCreate(key: string, factory: () => any, seconds: number): any;
  getOrCreate(key: string, factory: () => any, options?: CacheOptions): any;
  remove(key: string): void;
  removeAll(): void;
  containsKey(key: string): boolean;
  localCache(externalUrl: string, hours: number): string;
  getOrSet(key: string, scriptOrFunctionName: string, timeOutMinutes: number): any;
}

interface CacheOptions {
  absoluteExpiration?: TimeSpan;
  slidingExpiration?: TimeSpan;
  cascades: CacheCascade[];
}

interface CacheCascade {
  key: string;
  removeChangeToken: boolean;
}
```

## group

当前站点缓存使用的只读分组标识。同一站点内该值保持稳定，并用于将缓存键与其他站点隔离。请将它视为不透明字符串，不要解析或依赖其具体格式。

**返回：** `string`。

```ts
k.api.get(() => {
    return { group: k.cache.group }
})
```

## set()

写入缓存。

| 调用 | 说明 |
|------|------|
| `set(key, value, seconds)` | `seconds` 为绝对过期时间（秒） |
| `set(key, value)` | 默认约 **120 分钟** 过期 |
| `set(key, value, options)` | 使用 `CacheOptions` 控制过期与级联失效（见下节） |

键名最长 **1024** 字符；`set(key, value, seconds)` 的秒数受服务端 `Cache:MaxExpireSeconds` 配置上限（默认约 2 小时）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 缓存键 |
| `value` | `any` | 是 | 要缓存的值 |
| `seconds` | `number` | 否 | 过期秒数 |
| `options` | `CacheOptions` | 否 | 过期与级联失效配置 |

**返回：** `void`。

```ts
k.api.post(() => {
    const key = "summary-" + Date.now()
    k.cache.set(key, { ok: true }, 60)
    return {
        stored: k.cache.containsKey(key),
        value: k.cache.get(key)
    }
})
```

## Options 重载（`set` / `getOrCreate`）

第三参数传入 **`options`** 时，可分别设置绝对过期、滑动过期，以及 **级联键（`cascades`）**：当某级联键被 `remove` / `CascadeRemove` 触发时，关联缓存项一并失效。`removeAll()` 即对当前站点 Id 做级联清理。

| 字段 | 说明 |
|------|------|
| `absoluteExpiration` | 自写入起经过该时长后过期（类型为 **`TimeSpan`**，非裸数字秒） |
| `slidingExpiration` | 滑动过期：该时长内无访问则过期（`TimeSpan`） |
| `cascades` | `{ key, removeChangeToken }[]`；`key` 为级联分组名，`removeChangeToken` 控制项被淘汰时是否移除 token |

日常过期用 **`set(key, value, seconds)`** 即可；`set(key, value, seconds)` 在内部等价于带 `absoluteExpiration` 且 `cascades` 含当前站点 Id 的简化写法。

需要**滑动过期**或**自定义级联键**时使用 `options`。`absoluteExpiration` / `slidingExpiration` 须按智能提示传入 **`TimeSpan`**（不能写成 `set(key, value, 120)` 这种裸数字）。

`cascades` 项为 `CacheCascade` 结构（`key`、`removeChangeToken`），具体对象写法以编辑器类型为准。若在脚本中配置不便，可继续使用 **`set` / `getOrCreate` 的秒数重载**（已自动绑定当前站点的级联键）。

```ts
k.api.post(() => {
    const key = "demo-" + Date.now()
    k.cache.set(key, { ok: true }, 60)
    return {
        stored: k.cache.containsKey(key),
        value: k.cache.get(key)
    }
})
```

## get()

读取缓存；不存在时返回 `undefined`。

```ts
k.api.get(() => {
    const key = k.request.queryString.key
    return k.cache.get(key)
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 缓存键 |

**返回：** `any`，缓存值；不存在时返回 `undefined`。

## getOrCreate()

不存在时执行 `factory` 写入缓存并返回值。

| 调用 | 说明 |
|------|------|
| `getOrCreate(key, factory, seconds)` | 秒数过期（常用） |
| `getOrCreate(key, factory, options?)` | 使用 `CacheOptions` |

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 缓存键 |
| `factory` | `() => any` | 是 | 未命中时执行并返回要缓存的值 |
| `seconds` | `number` | 否 | 过期秒数 |
| `options` | `CacheOptions` | 否 | 过期与级联失效配置 |

**返回：** `any`，命中的缓存值或 `factory` 生成的新值。

```ts
k.api.get(() => {
    const key = "expensive-" + k.request.queryString.id
    const value = k.cache.getOrCreate(
        key,
        () => {
            return { computedAt: Date.now(), id: k.request.queryString.id }
        },
        300
    )
    return value
})
```

## containsKey()

检查指定缓存键是否存在。

```ts
k.api.get(() => {
    return { exists: k.cache.containsKey("my-key") }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 缓存键 |

**返回：** `boolean`。

## remove() / removeAll()

`remove(key)` 删除单项；`removeAll()` 清除**当前站点**下本缓存分组内的全部项。

```ts
k.api.post(() => {
    const key = "rm-" + Date.now()
    k.cache.set(key, 1, 60)
    k.cache.remove(key)
    return { exists: k.cache.containsKey(key) }
})
```

| 方法 | 参数 | 返回 | 说明 |
|------|------|------|------|
| `remove(key)` | `key: string` | `void` | 删除指定缓存项 |
| `removeAll()` | 无 | `void` | 清除当前站点缓存分组 |

## localCache()

将外部 URL 资源缓存到站点本地，返回可访问的本地 URL。

```ts
k.api.get(() => {
    const url = k.cache.localCache("https://example.com/asset.png", 24)
    return { localUrl: url }
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `externalUrl` | `string` | 是 | 外部资源 URL |
| `hours` | `number` | 是 | 本地缓存小时数 |

**返回：** `string`，站点内可访问的本地 URL。

## getOrSet()

未命中时执行一行 KScript（`scriptOrFunctionName`），将结果写入缓存。

| 参数 | 说明 |
|------|------|
| key | 缓存键 |
| scriptOrFunctionName | 要执行的脚本片段 |
| timeOutMinutes | 过期分钟数 |

**返回：** `any`，命中的缓存值或脚本执行结果。

```ts
k.api.get(() => {
    const data = k.cache.getOrSet(
        "report-summary",
        "k.DB.sqlite.query('SELECT 1')",
        30
    )
    return data
})
```

## 相关文档

- [k.session](../session/)
- [k.cookie](../cookie/)
