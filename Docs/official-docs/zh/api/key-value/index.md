# k.DB.keyValue

> 站点键值对存储 — 与后台 [键值存储](../../cms/database/key-value.md) 相同

## 概述

`k.DB.keyValue`（及顶层 **`k.keyValue`**）读写当前站点的 **字符串键值** 集合。适合缓存时间戳、开关标记、短文本配置等，无需定义 IndexedDB 表结构。

::: tip 先配后台
也可在 [键值存储](../../cms/database/key-value.md) 列表中维护；脚本 `set` 与后台保存写入同一存储。
:::

## 访问方式

```ts
k.DB.keyValue.set(key, value)
k.DB.keyValue.get(key)
k.DB.keyValue[key]          // 与 get 等价，返回 KeyValueObject
```

`get` / 属性访问在键不存在时得到 `null`（或等价空值）。

## set(key, value)

写入或更新一条键值。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 非空键名 |
| `value` | `string` | 是 | 值；`null` 按空处理。UTF-8 编码长度不得超过约 **4096 字节**，否则抛出长度错误 |

**返回：** `void`。

```typescript
k.DB.keyValue.set("featureFlags", JSON.stringify({ beta: true }))
```

## get(key)

按键读取，返回 **`KeyValueObject`**（可 `toString()` 或当字符串用）；不存在时返回 `null`。

```typescript
const raw = k.DB.keyValue.get("featureFlags")
const text = raw ? raw.toString() : "{}"
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 键名 |

**返回：** `KeyValueObject | null`。

## 字典式用法

`kKeyValue` 实现键值字典接口，可遍历：

```typescript
for (const entry of k.DB.keyValue) {
  k.console.log(entry.key, entry.value)
}
```

`Count` / `length` 表示当前键数量。`Remove(key)` 删除指定键。

## TypeScript 形状（节选）

```ts
interface KDB {
  keyValue: kKeyValue
}

// k.keyValue 与 k.DB.keyValue 相同实例
```

完整成员以 IDE 中 `kooboo.d.ts` 为准。

## 注意事项

1. **值类型**：以 **字符串** 存储；对象请自行 `JSON.stringify` / `parse`。  
2. **键名**：与后台一致，新建时须字母或数字开头且唯一。  
3. **服务端**：在 `env="server"` 的 Code、API、定时任务等环境使用。  
4. **与 IndexedDB 表无关**：不要用 `indexedDb` 表名访问键值数据。

## 相关文档

- [键值存储（CMS）](../../cms/database/key-value.md)  
- [k.DB 概述](../database/)  
- [k.DB.indexedDb](../indexed-db/) — 结构化动态表  
