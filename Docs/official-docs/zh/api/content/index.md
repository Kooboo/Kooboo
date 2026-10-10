# k.content

> 内容管理 - 内容 CRUD 和查询

## 概述

`k.content` 提供了一套完整的内容管理 API，用于对站点内容进行创建、查询、更新、删除等操作。

**前置条件**：须在后台配置**数据类型**与**内容夹**。操作步骤见 [后台：内容](../../cms/content/index.md)（[数据类型](../../cms/content/content-types.md) · [内容夹列表](../../cms/content/contents-folders-list.md)）。

## TypeScript 定义

```ts
interface KContent {
  // 获取所有内容文件夹名称
  getFolders(): string[];

  // 获取指定文件夹
  [folderName: string]: KContentFolder;
}

interface KContentFolder {
  // 查询方法
  all(options?: QueryOptions): Content[];
  get(nameOrId: string): Content | null;
  find(query: string | object, options?: QueryOptions): Content | null;
  findAll(query: string | object, options?: QueryOptions): Content[];
  findAllByCategory(
    categoryFolder: string,
    categorySlugOrId: string,
    options?: QueryOptions
  ): Content[];

  // 增删改方法
  add(content: object): Content;
  update(content: Content): void;
  delete(nameOrId: string | object): void;

  // 排序方法
  move(source: string, prev?: string, next?: string): void;
}

interface QueryOptions {
  excludeEmpty?: boolean;         // 排除空值记录
  includeOfflineData?: boolean;   // 包含离线数据
  skip?: number;                  // 跳过的记录数
  take?: number;                  // 返回的最大记录数
  orderBy?: string;               // 按字段升序排列
  orderByDescending?: string;     // 按字段降序排列
  select?: string[];              // 只返回指定字段
}
```

## 全局方法

### k.content.getFolders()

获取站点中所有内容文件夹的名称列表。

**参数：** 无。

**返回：** `string[]`。

```ts
k.api.get(() => {
    const folders = k.content.getFolders();
    return { folders };
})
// 返回: { folders: ["Article", "Product", "News"] }
```

## 查询方法

### all()

获取文件夹中的所有内容。

```ts
k.api.get(() => {
    const articles = k.content.Article.all();
    return { count: articles.length, articles };
})
```

**参数 (QueryOptions):**

| 参数 | 类型 | 说明 |
|------|------|------|
| excludeEmpty | `boolean` | 是否排除空值记录，默认 `false` |
| includeOfflineData | `boolean` | 是否包含离线数据，默认 `false` |
| skip | `number` | 跳过的记录数 |
| take | `number` | 返回的最大记录数 |
| orderBy | `string` | 按指定字段升序排列 |
| orderByDescending | `string` | 按指定字段降序排列 |
| select | `string[]` | 只读取并返回指定字段 |

**返回：** `Content[]`。

```ts
// 排除空值
k.api.get(() => {
    return k.content.Article.all({ excludeEmpty: true });
})

// 包含离线内容
k.api.get(() => {
    return k.content.Article.all({ includeOfflineData: true });
})

// 分页、排序并限制返回字段
k.api.get(() => {
    return k.content.Article.all({
        skip: 20,
        take: 10,
        orderByDescending: "creationDate",
        select: ["title", "slug", "creationDate"]
    });
})
```

### get()

根据 ID 或 UserKey 获取单条内容。

```ts
k.api.get(() => {
    const article = k.content.Article.get("article-id-or-userkey");
    return article;
})
```

**参数:**

| 参数 | 类型 | 说明 |
|------|------|------|
| nameOrId | `string` | 内容的 ID 或 UserKey |

**返回：** `Content | null`。

### find()

查询单条内容，支持字符串查询和对象查询两种方式。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `query` | `string | object` | 是 | 字符串条件或对象查询条件 |
| `options` | `QueryOptions` | 否 | 查询选项 |

**返回：** `Content | null`。

**字符串查询：**

```ts
k.api.get(() => {
    // 使用字符串条件
    const article = k.content.Article.find("title == 'Hello World'");
    return article;
})
```

**支持的运算符：**
- `==` 等于
- `!=` / `<>` 不等于
- `>=` 大于等于
- `>` 大于
- `<=` 小于等于
- `<` 小于
- `&&` / `and` 并且
- `||` / `or` 或者
- `contains` 包含
- `startwith` 开头匹配

```ts
// 数字字段按数值比较；数字不要加引号
k.api.get(() => {
    const adult = k.content.People.find("age > 23");
    return adult;
})

// 日期字段支持大小比较；系统日期字段也可直接用于查询
k.api.get(() => {
    const recent = k.content.Article.find(
        "creationDate >= '2026-01-01T00:00:00Z'"
    );
    return recent;
})

// 多条件查询
k.api.get(() => {
    const article = k.content.Article.find("title == 'Hello' && author == 'John'");
    return article;
})

// 模糊查询
k.api.get(() => {
    const article = k.content.Article.find("title contains 'World'");
    return article;
})
```

查询会根据条件值的类型执行比较：未加引号的数字按数值处理，日期字段按日期时间处理，字符串则按文本处理。`userKey`、`slug`、`creationDate`、`lastModified` 等系统字段也可以写在查询条件中。

**对象查询：**

```ts
// 使用对象查询运算符进行数字比较
k.api.get(() => {
    return k.content.People.find({
        age: { $gt: 23 }
    });
})

// 使用系统日期字段进行日期比较
k.api.get(() => {
    return k.content.Article.findAll({
        creationDate: { $gte: "2026-01-01T00:00:00Z" }
    });
})

// 使用 $and 和 $or 组合多个条件
k.api.get(() => {
    return k.content.People.findAll({
        $and: [
            { age: { $gte: 18 } },
            {
                $or: [
                    { status: "active" },
                    { status: "pending" }
                ]
            }
        ]
    });
})
```

对象查询的运算符以 `$` 开头，直接写在查询对象中，无需预先获取或声明运算符常量。同一级直接书写的多个字段按 AND 关系组合；需要显式分组时使用 `$and` 或 `$or`。同样的对象语法也适用于 `findAll()`。

**find + options：**

```ts
k.api.get(() => {
    const article = k.content.Article.find(
        { title: "Hello" },
        { includeOfflineData: true }
    );
    return article;
})
```

### findAll()

查询多条内容，用法与 `find()` 相同，但返回数组。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `query` | `string | object` | 是 | 字符串条件或对象查询条件 |
| `options` | `QueryOptions` | 否 | 查询选项 |

**返回：** `Content[]`。

```ts
k.api.get(() => {
    const articles = k.content.Article.findAll({ category: "news" });
    return { count: articles.length, articles };
})

// 使用对象条件表达 OR 查询
k.api.get(() => {
    const articles = k.content.Article.findAll({
        $or: [
            { title: "News1" },
            { title: "News2" }
        ]
    });
    return { count: articles.length };
})
```

### findAllByCategory()

根据分类内容查询当前内容夹中的所有关联内容。分类内容可以用所属内容夹名称，以及该分类的 `slug`、`userKey` 或 ID 指定。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `categoryFolder` | `string` | 是 | 分类内容所在的内容夹名称 |
| `categorySlugOrId` | `string` | 是 | 分类内容的 `slug`、`userKey` 或 ID |
| `options` | `QueryOptions` | 否 | 分页、排序、字段选择和离线数据选项 |

**返回：** `Content[]`。分类内容不存在、分类内容夹不匹配或没有关联内容时返回空数组。

```ts
k.api.get(() => {
    const articles = k.content.Article.findAllByCategory(
        "Tags",
        "technology",
        {
            take: 10,
            orderByDescending: "creationDate"
        }
    );

    return { count: articles.length, articles };
})
```

这里的 `Tags` 是分类内容夹名称，`technology` 是该内容夹中某条分类内容的 `slug` 或 `userKey`。也可以把第二个参数替换为该分类内容的 ID。

## 增删改方法

### add()

创建新内容。

```ts
k.api.post(() => {
    const article = k.content.Article.add({
        title: "我的文章",
        content: "这是文章内容",
        author: "张三",
        views: 0
    });
    return { id: article.id, title: article.title };
})
```

**参数:**

| 参数 | 类型 | 说明 |
|------|------|------|
| content | `object` | 内容对象，键为字段名，值为字段值 |

**返回：** `Content`，创建后的内容对象。

**系统字段（可选）：**

| 字段 | 类型 | 说明 |
|------|------|------|
| userKey | `string` | 用户定义的唯一标识 |
| online | `boolean` | 是否上线，默认 `true` |
| order / sequence | `number` | 排序序号 |

### update()

更新已有内容。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `content` | `Content` | 是 | 从 `find`、`get`、`all` 等方法取得并修改后的完整内容对象 |

**返回：** `void`。

```ts
k.api.post(() => {
    // 先查询要更新的内容
    const article = k.content.Article.find({ title: "旧标题" });

    // 修改字段
    article.title = "新标题";
    article.views = article.views + 1;

    // 执行更新
    k.content.Article.update(article);

    return { success: true, article };
})
```

::: warning 注意
update 方法需要传入从 `find`、`get`、`all` 等方法获取的完整内容对象，而不是新建的对象。因为内容对象包含系统字段（如 `id`、`version` 等），这些字段用于标识要更新的记录。
:::

### delete()

删除内容。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `contentOrId` | `string | Content` | 是 | 内容 Id 或完整内容对象 |

**返回：** `void`。

```ts
k.api.post(() => {
    const article = k.content.Article.find({ title: "要删除的文章" });

    if (article) {
        k.content.Article.delete(article.id);
        return { success: true };
    }
    return { success: false, error: "文章不存在" };
})

// 也可以直接传入内容对象
k.api.post(() => {
    const article = k.content.Article.find({ title: "要删除的文章" });

    if (article) {
        k.content.Article.delete(article);
        return { success: true };
    }
    return { success: false };
})
```

## 排序方法

### move()

调整内容在列表中的排序位置。

```ts
k.api.post(() => {
    // 将文章从原位置移动到 targetId 之后
    // prev: 参考元素的 ID
    // next: 参考元素之后的 ID（可选）
    k.content.Article.move(sourceId, prevId, nextId);

    return { success: true };
})
```

**参数:**

| 参数 | 类型 | 说明 |
|------|------|------|
| source | `string` | 要移动的内容 ID |
| prev | `string` | 参考元素 ID（插入到此元素之后） |
| next | `string` | 可选，插入到这个 ID 之后 |

**返回：** `void`。

## 多语言支持

### 设置语言环境

通过 `k.request.setCulture()` 设置当前请求的语言环境，然后再查询内容。

```ts
k.api.get(() => {
    // 设置为中文
    k.request.setCulture("zh");
    const zhArticle = k.content.Article.get("article-id");

    // 设置为英文
    k.request.setCulture("en");
    const enArticle = k.content.Article.get("article-id");

    return {
        zh: zhArticle.title,
        en: enArticle.title
    };
})
```

::: tip 使用时机
必须在调用 `k.content` 查询方法之前设置语言环境。
:::

### 多语言字段

在数据类型中设置字段的 **多语言** 属性后，该字段的内容会根据当前语言环境返回对应的翻译版本。

## 内容结构

### Content 系统字段

所有内容对象都包含以下系统字段：

| 字段 | 类型 | 说明 |
|------|------|------|
| id | `string` | 内容的唯一标识符（GUID） |
| userKey | `string` | 用户定义的唯一标识 |
| slug | `string` | `userKey` 的查询与输出别名 |
| lastModified | `string` | 最后修改时间（ISO 8601） |
| creationDate | `string` | 创建时间（ISO 8601） |
| parentId | `string` | 父级 ID |
| version | `number` | 版本号 |
| sequence | `number` | 排序序号 |
| online | `boolean` | 是否上线（true=在线，false=离线） |

`find()` 和 `findAll()` 的查询条件可以直接引用系统字段。当前最常用的系统字段查询如下：

| 字段 | 比较方式 | 示例 |
|------|----------|------|
| `userKey` / `slug` | 精确匹配；`findAll()` 还支持不等于、包含和开头匹配 | `slug == 'release-notes'` |
| `creationDate` | 日期等于、不等于及大小比较 | `creationDate >= '2026-01-01T00:00:00Z'` |
| `lastModified` | 日期等于、不等于及大小比较 | `lastModified < '2026-08-01T00:00:00Z'` |

### 示例内容对象

```json
{
    "title": "文章标题",
    "content": "文章内容",
    "author": "作者名",
    "id": "cd6b8f09-2146-73d3-cade-4e832627b4f6",
    "userKey": "my-article-key",
    "slug": "my-article-key",
    "lastModified": "2025-10-28T09:32:30.3162015Z",
    "creationDate": "2025-10-28T09:32:30.3137662Z",
    "parentId": "00000000-0000-0000-0000-000000000000",
    "version": 303,
    "sequence": 0,
    "online": true
}
```

## 查询操作符详解

`find()` 和 `findAll()` 同时支持对象查询和字符串查询。对象查询使用 `$...` 运算符键，字符串查询使用 `>`、`contains`、`&&` 等表达式运算符。

### 对象查询运算符

| 操作符 | 说明 | 示例 |
|--------|------|------|
| `$eq` | 等于；字段直接赋值也是等值查询 | `{ status: { $eq: "active" } }` |
| `$ne` | 不等于 | `{ status: { $ne: "deleted" } }` |
| `$gt` | 大于 | `{ age: { $gt: 23 } }` |
| `$gte` | 大于等于 | `{ age: { $gte: 18 } }` |
| `$lt` | 小于 | `{ stock: { $lt: 10 } }` |
| `$lte` | 小于等于 | `{ creationDate: { $lte: "2026-12-31T23:59:59Z" } }` |
| `$contains` | 包含，常用于字符串字段 | `{ title: { $contains: "Kooboo" } }` |
| `$startswith` | 以前缀开头，常用于字符串字段 | `{ code: { $startswith: "SKU-" } }` |
| `$and` | 所有条件都成立 | `{ $and: [{ age: { $gte: 18 } }, { status: "active" }] }` |
| `$or` | 任一条件成立 | `{ $or: [{ status: "active" }, { status: "pending" }] }` |

对象条件中的数字直接写为数字；日期建议使用 ISO 8601 字符串。对象运算符直接写在查询对象中，不需要额外声明。

```ts
k.api.get(() => {
    return k.content.Article.findAll({
        $and: [
            { creationDate: { $gte: "2026-01-01T00:00:00Z" } },
            { title: { $contains: "Kooboo" } }
        ]
    });
})
```

### 字符串查询比较操作符

| 操作符 | 说明 | 示例 |
|--------|------|------|
| `==` / `=` | 等于 | `title == 'Hello'` |
| `!=` / `<>` | 不等于 | `status != 'draft'` |
| `>` | 大于 | `age > 23` |
| `>=` | 大于等于 | `price >= 10` |
| `<` | 小于 | `stock < 5` |
| `<=` | 小于等于 | `creationDate <= '2026-12-31T23:59:59Z'` |

数字、日期和字符串使用同一组比较符，但按照字段值类型进行比较。数字条件不要加引号；日期建议使用 ISO 8601 格式。

### 字符串查询文本操作符

| 操作符 | 说明 | 示例 |
|--------|------|------|
| `contains` | 包含 | `title contains 'news'` |
| `startwith` | 开头匹配 | `name startwith 'A'` |

### 字符串查询逻辑操作符

| 操作符 | 说明 | 示例 |
|--------|------|------|
| `&&` / `and` | 并且 | `a == 1 && b == 2` |
| `\|\|` / `or` | 或者 | `a == 1 \|\| a == 2` |

### 字符串 OR 查询示例

```ts
// 使用 ||
k.api.get(() => {
    return k.content.Article.findAll(
        "title == 'News1' || title == 'News2'"
    );
})

// 使用 or
k.api.get(() => {
    return k.content.Article.findAll(
        "status == 'published' or status == 'scheduled'"
    );
})
```

## QueryOptions 详解

### excludeEmpty

排除所有用户字段都为空的记录。

```ts
k.api.get(() => {
    // 返回所有至少有 一个字段有值的内容
    return k.content.Article.all({ excludeEmpty: true });
})
```

### includeOfflineData

包含 `online` 字段为 `false` 的离线内容。默认不返回离线内容。

```ts
k.api.get(() => {
    // 返回包括离线内容的所有内容
    return k.content.Article.all({ includeOfflineData: true });
})
```

### skip 与 take

使用 `skip` 跳过前面的记录，使用 `take` 限制返回数量。它们可用于 `all()`、`findAll()` 和 `findAllByCategory()`。

```ts
k.api.get(() => {
    return k.content.Article.findAll("age > 23", {
        skip: 20,
        take: 10
    });
})
```

### orderBy 与 orderByDescending

传入字段名进行升序或降序排列。一次查询选择其中一种排序方向即可；字段可以是自定义字段，也可以是 `creationDate`、`lastModified` 等系统字段。

```ts
k.api.get(() => {
    return k.content.Article.findAll("age > 23", {
        orderByDescending: "creationDate",
        take: 10
    });
})
```

### select

只读取并返回指定字段，可减少不需要的内容字段读取。

```ts
k.api.get(() => {
    return k.content.Article.all({
        select: ["title", "slug", "creationDate"]
    });
})
```

## 相关文档

- [k.DB](../database/) - 数据库操作
- [k.request](../request/) - 请求对象
