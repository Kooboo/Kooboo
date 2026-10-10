# k.DB.keyValue

> Site key-value store — same as backend [Key-Value Storage](../../cms/database/key-value.md)

## Overview

`k.DB.keyValue` (and top-level **`k.keyValue`**) reads and writes the current site's **string key** collection. Suitable for caching timestamps, switch tags, short text configurations, etc., without defining the IndexedDB table structure.

::: tip configure the background first
Also maintained in the [Key-Value Storage](../../cms/database/key-value.md) list; script `set` writes to the same storage as saving from the admin UI.
:::

## Access method

```ts
k.DB.keyValue.set(key, value)
k.DB.keyValue.get(key)
k.DB.keyValue[key]          // Equivalent to get(), returns KeyValueObject
```

`get` / Attribute access gets `null` (or equivalent null) when the key does not exist.

## set(key, value)

Write or update a key value.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `key` | `string` | yes | Non-empty key name |
| `value` | `string` | yes | Value; `null` is treated as empty. UTF-8 encoding length must not exceed approximately **4096 bytes**, otherwise a length error is thrown |

**Returns:** `void`.

```typescript
k.DB.keyValue.set("featureFlags", JSON.stringify({ beta: true }))
```

## get(key)

Read by key, return **`KeyValueObject`** (can be `toString()` or used as a string); return `null` if it does not exist.

```typescript
const raw = k.DB.keyValue.get("featureFlags")
const text = raw ? raw.toString() : "{}"
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `key` | `string` | yes | Key name |

**Returns:** `KeyValueObject | null`.

## Dictionary usage

`kKeyValue` implements the key-value dictionary interface and can be traversed:

```typescript
for (const entry of k.DB.keyValue) {
  k.console.log(entry.key, entry.value)
}
```

`Count` / `length` represents the current number of keys. `Remove(key)` deletes the specified key.

## TypeScript Shapes

```ts
interface KDB {
  keyValue: kKeyValue
}

// k.keyValue and k.DB.keyValue are the same instance
```

The complete member is based on `kooboo.d.ts` in the IDE.

## Notes

1. **Value type**: stored as **string**; please use `JSON.stringify` / `parse` for the object.
2. **Key name**: Consistent with the backend, it must start with a letter or number and be unique when creating a new key.
3. **Server**: Used in `env="server"`'s Code, API, scheduled tasks and other environments.
4. **Not related to IndexedDB tables**: Do not use the `indexedDb` table name to access key-value data.

## Related Docs

- [Key-Value Storage (CMS)](../../cms/database/key-value.md)
- [k.DB Overview](../database/)
- [k.DB.indexedDb](../indexed-db/) — structured dynamic table
