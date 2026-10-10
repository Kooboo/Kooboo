# k.content

> 内容管理：在 **KScript 运行时** 对站点内容做查询与增删改。**内容类型与内容夹的定义**优先通过后台或内置函数完成，参见 [function_guide.md](function_guide.md)。

**语义对齐:** 与 `kb-docs/docs/api/data/content.md` 保持一致；若两处冲突，以 kb-docs 与引擎行为为准。本文同步占位：2026-04-21。

## 概述

`k.content` 提供内容 CRUD 与查询。**前置条件**：已在后台或通过内置函数配置 **数据类型（ContentType）** 与 **内容夹（ContentFolder）**。文件夹名即访问键，例如夹名为 `Article` 则使用 `k.content.Article`。

## TypeScript 定义

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

## 全局方法

### `k.content.operators()`

返回查询用的逻辑/比较操作符常量（`AND`、`OR`、`EQ`、`CONTAINS` 等），用于 **对象形式** 的 `find` / `findAll`。

### `k.content.getFolders()`

返回站点中所有内容夹名称数组。

## 文件夹实例：`k.content.<FolderName>`

动态属性名为内容夹名称，例如 `k.content.Article`。

### `all(options?)`

返回该夹下内容数组。`QueryOptions`：


| 参数                 | 说明                                  |
| ------------------ | ----------------------------------- |
| excludeEmpty       | 为 true 时排除「用户字段全为空」的记录；仅在站点启用多语言内容时生效              |
| includeOfflineData | 为 true 时包含 `online === false` 的离线内容 |
| includeOffline     | `includeOfflineData` 的别名 |
| skip               | 跳过的匹配记录数 |
| take               | 最多返回的记录数；`all()` 的有效值不大于 0 时最多返回 999 条 |
| orderBy            | 按指定用户字段升序排列 |
| orderByDescending  | 按指定用户字段降序排列 |
| select             | 要加载的用户字段名；返回内容对象只物化所选用户字段 |


### `get(nameOrId)`

按 **Id** 或 **UserKey** 取单条；不存在返回 `null`。

### `find(query, options?)`

- **字符串条件**：如 `"title == 'Hello'"`，支持 `==`、`>=`、`>`、`<=`、`<`、`&&`、`||`、`contains`、`startwith`。
- **对象条件**：可解构 `operators()`，组合 `AND` / `OR` 等（见下文 OR 示例）。

### `findAll(query, options?)`

与 `find` 相同，返回数组。

### `add(content)`

传入 **普通对象**（用户字段键值）。可选系统字段：`userKey`、`online`、`order` / `sequence`。返回带 `id` 等系统字段的内容对象。

### `update(content)`

传入 **由 `get` / `find` / `all` 得到的完整 `Content` 对象**，修改属性后调用；**不要**用手捏的半截对象替代，以免缺少 `id`、`version` 等导致更新失败或错更。

### `delete(nameOrId | content)`

可传 Id / UserKey，或 **内容对象**。

### `move(source, prev?, next?)`

调整列表排序。`source`、`prev`、`next` 都应为 GUID 字符串；`prev` 为参考元素 Id（插在其后）；`next` 可选。

## 多语言

在调用查询前使用 `k.request.setCulture("zh")` 等设置当前请求文化，再访问 `k.content`；多语言字段按当前 culture 返回。

## 系统字段（Content）


| 字段                          | 说明     |
| --------------------------- | ------ |
| id                          | GUID   |
| userKey                     | 用户定义键  |
| lastModified / creationDate | ISO 时间 |
| parentId                    | 父级 Id  |
| version                     | 版本     |
| sequence                    | 排序     |
| online                      | 是否在线   |


## 查询操作符速查


| 含义  | 字符串示例                | 对象常量                        |
| --- | -------------------- | --------------------------- |
| 与   | `a && b`             | `AND`                       |
| 或   | 逻辑或（`                |                             |
| 等于  | `f == 'x'`           | `EQ`                        |
| 大于等 | `views > 100`        | `GT` / `GTE` / `LT` / `LTE` |
| 包含  | `title contains 'x'` | `CONTAINS`                  |
| 开头  | `name startwith 'A'` | `STARTS_WITH`               |


### OR 的三种对象写法（摘自官方文档模式）

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

## 典型 API 模板

下列模板中请将 `Article`、字段名替换为实际内容夹与 schema 字段。

### 列表（全部 / 条件）

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

### 详情

```ts
k.api.get(() => {
  const item = k.content.Article.get("id-or-userkey");
  return { item };
});
```

### 新建

```ts
k.api.post(() => {
  const created = k.content.Article.add({
    title: "标题",
    body: "正文",
  });
  return { id: created.id, userKey: created.userKey };
});
```

### 更新

```ts
k.api.post(() => {
  const item = k.content.Article.get("id-or-userkey");
  if (!item) return { ok: false };
  item.title = "新标题";
  k.content.Article.update(item);
  return { ok: true };
});
```

### 删除

```ts
k.api.post(() => {
  const item = k.content.Article.find({ title: "待删除" });
  if (!item) return { ok: false };
  k.content.Article.delete(item.id);
  return { ok: true };
});
```

### 多语言读取对比

```ts
k.api.get(() => {
  k.request.setCulture("zh");
  const zhTitle = k.content.Article.get("id")?.title;
  k.request.setCulture("en");
  const enTitle = k.content.Article.get("id")?.title;
  return { zhTitle, enTitle };
});
```

## AI 注意事项

1. **update 必须基于查询返回的对象**，以保证 `id` / `version` 等系统字段正确。
2. 默认不返回离线内容；需要时传 `includeOfflineData: true`。
3. **文件夹名大小写与拼写** 必须与后台一致，否则 `k.content.XXX` 不存在。

## 相关

- [function_guide.md](function_guide.md) — 内置函数建模与 `DataJson`
- [KScript 总索引](../index.md)
