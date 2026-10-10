# Key-Value Storage

> Menu: **Database → Key-Value Storage**  
> Deep link: `/_Admin/database/key-value?SiteId={siteGuid}`

**Key-value storage** provides simple site-level **key / value** pairs: suitable for cache flags, lightweight configuration, temporary state, and other scenarios that do not need a table schema. Values are stored as strings; scripts read and write through **`k.DB.keyValue`** (same data source as the admin).

::: tip Permissions
List and view: `keyValue` (view). **New** and editing values require `keyValue·edit`; multi-select **Delete** requires `keyValue·delete`.
:::

::: info Difference from IndexedDB tables
| | **Key-value storage** | **IndexedDB tables** |
|--|----------------------|----------------------|
| Model | Flat key → string value | Multiple columns, types, indexes |
| Admin | Key-value list on this page | [IndexedDB tables](./table.md) |
| Script | `k.DB.keyValue.set` / `get` | `k.DB.indexedDb.{tableName}` |

Use IndexedDB tables when you need structured queries or relations; use key-value storage when you only need a few string settings.
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), check **Database → Key-Value Storage**.
2. Open **Database → Key-Value Storage** in the left sidebar.

<DocImage src="/cms/database/key-value-overview.png" alt="Key-value storage list" width="1120" />

## Key-Value List

### Toolbar

| Button | Permission | Description |
|--------|------------|-------------|
| **New** | `keyValue·edit` | Opens the [new/edit dialog](#new-and-edit-dialog) (empty key) |

<DocImage src="/cms/database/key-value-toolbar.png" alt="Key-value storage toolbar" width="1120" />

### List table

| Column | Description |
|--------|-------------|
| **Key** | Unique identifier; list sorted by key name |
| **Value** | Currently stored string (summary display in the list) |
| Row **Edit** | Opens the dialog to change the **value** (key name cannot be changed) |

Supports multi-select **Delete** (deletes by **key**).

<DocImage src="/cms/database/key-value-list.png" alt="Key-value list" width="1120" />

## New and Edit Dialog

| Field | Description |
|-------|-------------|
| **Key** | Required when creating; must **start with a letter or digit**; unique within the site (prompts if the key is already in use). When **editing**, the key is disabled; only the value can be changed |
| **Value** | Required; multiline text. UTF-8 length per value is capped at about **4096 bytes** (save fails if exceeded) |

Save calls `KeyValue/Update` (same endpoint for create and update). The list refreshes after a successful create.

<DocImage src="/cms/database/key-value-edit-dialog.png" alt="New or edit key-value dialog" width="1120" />

## Script access (development)

```typescript
k.DB.keyValue.set("lastSync", "2026-05-27T12:00:00Z")
const v = k.DB.keyValue.get("lastSync")
// Or property access (equivalent to get)
const v2 = k.DB.keyValue.lastSync
```

You can also use top-level **`k.keyValue`** (same entry point as `k.DB.keyValue`). See [k.DB.keyValue](../../api/key-value/index.md).

::: warning Not Content "Parameter Config"
Site **Content → Parameter Config** (UserOptions) is a separate structured JSON configuration system, corresponding to `k.paramConfig` and related APIs. Key-value storage is independent; do not confuse the two menus.
:::

## Typical workflow

1. **New** a key-value pair with key name and initial value.  
2. In Code / scheduled jobs, `get` / `set` to read and write.  
3. Adjust via list **Edit** or script `set` to overwrite.  
4. Multi-select **Delete** when no longer needed.

## Related

| Doc | Description |
|-----|-------------|
| [Database overview](./index.md) | Database group |
| [k.DB.keyValue](../../api/key-value/index.md) | Script API |
| [IndexedDB tables](./table.md) | Structured data |
| [Parameter config](../content/user-options.md) | Structured site parameters (different feature) |
