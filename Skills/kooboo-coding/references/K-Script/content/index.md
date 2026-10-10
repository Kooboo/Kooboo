# k.content

> Content management: query, create, update, and delete site content in the **KScript runtime**. Define **content types and content folders** through the admin UI or built-in functions first; see [function_guide.md](function_guide.md).

**Semantic alignment:** Keep this document consistent with `kb-docs/docs/api/data/content.md`. If the two sources conflict, follow kb-docs and engine behavior. Synchronization placeholder: 2026-04-21.

## Overview

`k.content` provides content CRUD and query operations. **Prerequisite**: configure the **data type (ContentType)** and **content folder (ContentFolder)** in the admin UI or through built-in functions. The folder name is the access key; for example, use `k.content.Article` when the folder is named `Article`.

## TypeScript definition

```ts
interface KContent {
  operators(): ContentOperators;
  getFolders(): string[];
  [folderName: string]: KContentFolder | (() => ContentOperators) | (() => string[]) | undefined;
}

interface ContentOperators {
  AND: string;
  OR: string;
  EQ: string;
  NE: string;
  GT: string;
  GTE: string;
  LT: string;
  LTE: string;
  STARTS_WITH: string;
  CONTAINS: string;
}

interface KContentFolder {
  all(options?: QueryOptions): Content[];
  get(nameOrId: string): Content | null;
  find(query: string | object, options?: QueryOptions): Content | null;
  findAll(query: string | object, options?: QueryOptions): Content[];
  add(content: object): Content;
  update(content: Content): void;
  delete(nameOrId: string | object): void;
  move(source: string, prev?: string, next?: string): void;
}

interface QueryOptions {
  excludeEmpty?: boolean;
  includeOfflineData?: boolean;
  includeOffline?: boolean;
  skip?: number;
  take?: number;
  orderBy?: string;
  orderByDescending?: string;
  select?: string[];
}
```

## Global methods

### `k.content.operators()`

Returns logical and comparison operator constants for queries (`AND`, `OR`, `EQ`, `CONTAINS`, and others), for use with the **object form** of `find` / `findAll`.

### `k.content.getFolders()`

Returns an array containing all content folder names in the site.

## Folder instance: `k.content.<FolderName>`

The dynamic property name is the content folder name, for example `k.content.Article`.

### `all(options?)`

Returns an array of content in the folder. `QueryOptions`:


| Parameter            | Description                           |
| ------------------ | ----------------------------------- |
| excludeEmpty       | When true, excludes records whose user fields are all empty; this option takes effect only when the site has multilingual content enabled |
| includeOfflineData | When true, includes offline content where `online === false` |
| includeOffline     | Alias of `includeOfflineData` |
| skip               | Number of matching records to skip |
| take               | Maximum number of records to return; `all()` uses a maximum of 999 when the effective value is not positive |
| orderBy            | User field name used for ascending ordering |
| orderByDescending  | User field name used for descending ordering |
| select             | User field names to load; only the selected user fields are materialized in the returned content objects |


### `get(nameOrId)`

Gets a single item by **Id** or **UserKey**; returns `null` when it does not exist.

### `find(query, options?)`

- **String condition**: for example, `"title == 'Hello'"`; supports `==`, `>=`, `>`, `<=`, `<`, `&&`, `||`, `contains`, and `startwith`.
- **Object condition**: destructure `operators()` to combine `AND`, `OR`, and others (see the OR examples below).

### `findAll(query, options?)`

Same as `find`, but returns an array.

### `add(content)`

Pass a **plain object** containing user-field values. Optional system fields include `userKey`, `online`, and `order` / `sequence`. Returns a content object with system fields such as `id`.

### `update(content)`

Pass the complete `Content` object **returned by `get` / `find` / `all`**, modify its properties, and then call the method. **Do not** substitute a hand-built partial object, which may lack `id`, `version`, or other fields and cause the update to fail or affect the wrong item.

### `delete(nameOrId | content)`

Accepts an Id / UserKey or a **content object**.

### `move(source, prev?, next?)`

Adjusts list ordering. `source`, `prev`, and `next` are GUID strings; `prev` is the reference element Id to insert after it, and `next` is optional.

## Multilingual content

Before querying, set the current request culture with `k.request.setCulture("zh")` or similar, then access `k.content`; multilingual fields are returned for the current culture.

## System fields (Content)


| Field                       | Description |
| --------------------------- | ------ |
| id                          | GUID   |
| userKey                     | User-defined key |
| lastModified / creationDate | ISO timestamp |
| parentId                    | Parent Id   |
| version                     | Version     |
| sequence                    | Sort order  |
| online                      | Whether the content is online |


## Query operator quick reference


| Meaning | String example            | Object constant                 |
| --- | -------------------- | --------------------------- |
| AND     | `a && b`                  | `AND`                           |
| OR      | Logical OR (`             |                                 |
| Equals  | `f == 'x'`                | `EQ`                            |
| Greater/less than | `views > 100` | `GT` / `GTE` / `LT` / `LTE` |
| Contains | `title contains 'x'`     | `CONTAINS`                      |
| Starts with | `name startwith 'A'` | `STARTS_WITH`                   |


### Three object forms for OR (following the official documentation pattern)

```ts
k.api.get(() => {
  const { OR } = k.content.operators();
  // 1) 同字段多值 OR
  return k.content.Article.findAll({ title: { [OR]: ["A", "B"] } });
});

k.api.get(() => {
  const { OR } = k.content.operators();
  // 2) 数组 OR
  return k.content.Article.findAll({
    [OR]: [{ title: "News1" }, { title: "News2" }],
  });
});
```

## Typical API templates

In the templates below, replace `Article` and the field names with the actual content folder and schema fields.

### List (all / filtered)

```ts
k.api.get(() => {
  const items = k.content.Article.all();
  return { count: items.length, items };
});

k.api.get(() => {
  const items = k.content.Article.findAll({ category: "news" });
  return { items };
});
```

### Detail

```ts
k.api.get(() => {
  const item = k.content.Article.get("id-or-userkey");
  return { item };
});
```

### Create

```ts
k.api.post(() => {
  const created = k.content.Article.add({
    title: "标题",
    body: "正文",
  });
  return { id: created.id, userKey: created.userKey };
});
```

### Update

```ts
k.api.post(() => {
  const item = k.content.Article.get("id-or-userkey");
  if (!item) return { ok: false };
  item.title = "新标题";
  k.content.Article.update(item);
  return { ok: true };
});
```

### Delete

```ts
k.api.post(() => {
  const item = k.content.Article.find({ title: "待删除" });
  if (!item) return { ok: false };
  k.content.Article.delete(item.id);
  return { ok: true };
});
```

### Compare multilingual reads

```ts
k.api.get(() => {
  k.request.setCulture("zh");
  const zhTitle = k.content.Article.get("id")?.title;
  k.request.setCulture("en");
  const enTitle = k.content.Article.get("id")?.title;
  return { zhTitle, enTitle };
});
```

## Notes for AI

1. **update must use an object returned by a query** so system fields such as `id` / `version` remain correct.
2. Offline content is not returned by default; pass `includeOfflineData: true` when needed.
3. The **folder name's casing and spelling** must match the admin configuration; otherwise `k.content.XXX` does not exist.

## Related

- [function_guide.md](function_guide.md) — Built-in function modeling and `DataJson`
- [KScript index](../index.md)
