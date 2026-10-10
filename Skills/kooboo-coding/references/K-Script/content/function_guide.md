# Content Built-in Function Guide

> Guides AI in using **Kooboo built-in functions** (`BuiltIn/Core/Content`) to create content types and folders and to read/write content with JSON. For content CRUD in KScript at **runtime**, read the [k.content module reference](index.md) in the same directory.

## When to read this guide

- The [site-building SOP](../../Scenario/site-building/index.md) maps a data source to `content`; model it before writing data.
- You need to call functions such as `create_content_type`, `get_content_schema`, or `create_content` but are unsure about their parameters or order.

## Division of responsibility with `index.md` (`k.content`)

| Scenario | Use |
| --- | --- |
| Define/adjust ContentType or ContentFolder; write or list by schema with tool functions | This guide + the corresponding built-in function |
| Access `k.content.文件夹名` in an API / CodeBlock / SSR KScript | [index.md](index.md) |

## Recommended call order

1. **`create_content_type`** — Create the data type.
2. Call **`create_content_folder`** explicitly, normally using the same name as the type and providing its `ContentTypeNameOrId`. The current implementation persists the ContentType but does not create a folder.
3. Before writing or validating fields, call **`get_content_schema`** to obtain field names, control types, and read-only information.
4. **`create_content`** / **`update_content`** — `DataJson` keys and values must match the schema (see the contract below).
5. For read-only listing or retrieving all fields, use **`list_content`** / **`get_content`**.
6. Evolve the model with **`update_content_type`** (append or delete properties).

## Approval

The following functions have `ApprovalRequired == true` and normally require user confirmation before execution:

- `create_content_type`
- `create_content_folder`
- `create_content`
- `update_content`
- `update_content_type`

The following have `ApprovalRequired == false`:

- `get_content_schema`
- `get_content`
- `list_content`

## `create_content_type`

Creates a ContentType. The current implementation does **not** create a ContentFolder; create the folder explicitly before creating content.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| Name | string | Yes | Type name (English identifier) |
| Properties | ContentProperty[] | Yes | Array of field definitions |

## `create_content_folder`

Creates a folder for an existing ContentType. If a folder with the requested name already exists, the function returns without changing it.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| Name | string | Yes | Folder identifier (used later as `k.content.Name`) |
| DisplayName | string | No | Display name in the admin UI |
| ContentTypeNameOrId | string | Yes | Name or ID of an existing type |

## `get_content_schema`

Gets the field schema for a content folder (for forms or building `DataJson`).

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| FolderNameOrId | string | Yes | Folder name or ID |

**Key output:** `Name` (folder name) and a `Properties` list; each item contains `Name`, `ControlType`, and `ReadOnly` (true for system or non-editable fields).

## `create_content`

**Creates** one content item in the specified folder.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| FolderNameOrId | string | Yes | Folder name or ID |
| DataJson | string | Yes | Static JSON string whose keys are **user field names** and match `get_content_schema`; for example `{"title":"x","views":"0"}` |

The implementation assigns each user field in the default language (as determined by the site's default culture).

The successful return value is the string `Save success`.

## `update_content`

Updates existing content by ID; `DataJson` may contain only the fields to change.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| ContentId | string | Yes | Content ID |
| FolderNameOrId | string | Yes | Folder name or ID |
| Culture | string | No | When specified, writes only to that culture; when empty, follows the implementation and writes the default-culture path (see `UpdateContent.cs` in the source) |
| DataJson | string | Yes | JSON of fields to modify; must match the schema |

## `get_content`

Reads the current values of all typed fields for a **single** content item.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| ContentId | string | Yes | Content ID or a name-resolvable identifier (consistent with `GetByNameOrId`) |
| Culture | string | No | Language used for the returned values |

## `list_content`

Lists all content in a folder; the returned fields can be restricted.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| FolderNameOrId | string | Yes | Folder name or ID |
| Culture | string | No | Language used for field values |
| Skip | int | No | Nonnegative offset; negative values are treated as `0` |
| Take | int | No | Result limit; omitted returns all remaining items, `0` returns no items, and negative values are treated as `0` |
| Fields | string[] | No | User field names to return; **if empty or omitted, only each item's `id` is returned** |
| Keyword | string | No | Case-insensitive substring search; searches selected fields, or all text fields when `Fields` is empty |

The result contains `List`, `Skip`, `Take`, and `Total`. When `Take` is omitted, `Total` counts all matching items. When `Take` is set, the current implementation may stop scanning early, so `Total` is not a reliable count of all matches before pagination.

## `update_content_type`

Appends or deletes properties by type name.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| Name | string | Yes | ContentType name |
| AppendProperties | object[] | No | Each item contains `Name`, `DisplayName`, and `ControlType` (a string matching an enum name in the table below) |
| DeleteProperties | string[] | No | List of property **Name** values to delete |

The current implementation ignores `SelectionOptions` when appending properties.

## ContentProperty (for `create_content_type`)

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| Name | string | Yes | Field identifier |
| DisplayName | string | No | Display name |
| ControlType | enum | Yes | See the values below |
| MultipleLanguage | bool | No | Defaults to false |
| IsSummary | bool | No | List-summary field |
| SelectionOptions | {Key,Value}[] | No | Used when `ControlType` is `Selection` |

### ControlType enum values

`TextBox`, `TextArea`, `RichEditor`, `Selection`, `CheckBox`, `RadioBox`, `Switch`, `MediaFile`, `File`, `DateTime`, `Number`, `ColorPicker`, `KeyValues`, `ValueList`

**Selection:** `SelectionOptions` is part of the input shape, but the current `create_content_type` and `update_content_type` implementations do not persist it.

## Common mistakes

1. Building `DataJson` by guesswork **without calling `get_content_schema`**, resulting in missing fields or a control-type mismatch.
2. Expecting full records from **`list_content`** without passing `Fields`; the implementation returns only `id`. Use `Skip`, `Take`, and `Keyword` for bounded or filtered reads.
3. **`update_content_type`**'s `AppendProperties.ControlType` must be a **string enum name** (matching the names used by `create_content_type`), distinct from the strongly typed `ContentProperty` used by `create_content_type`.

## Implementation source

The source of truth is `Kooboo.Sites/AI/Functions/BuiltIn/Core/Content/*.cs`.
