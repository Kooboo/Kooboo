# k.content

> Content management - CRUD and query APIs for site content

## Overview

`k.content` provides a complete set of content management APIs for creating, querying, updating, deleting, and sorting site content.

**Prerequisite:** configure **Content Types** and **Content Folders** in the admin first. See [Admin CMS -> Content](../../cms/content/index.md) ([Content types](../../cms/content/content-types.md) and [Content folder list](../../cms/content/contents-folders-list.md)).

## TypeScript Definition

```ts
interface KContent {
  getFolders(): string[];
  [folderName: string]: KContentFolder;
}

interface KContentFolder {
  all(options?: QueryOptions): Content[];
  get(nameOrId: string): Content | null;
  find(query: string | object, options?: QueryOptions): Content | null;
  findAll(query: string | object, options?: QueryOptions): Content[];
  findAllByCategory(
    categoryFolder: string,
    categorySlugOrId: string,
    options?: QueryOptions
  ): Content[];
  add(content: object): Content;
  update(content: Content): void;
  delete(nameOrId: string | object): void;
  move(source: string, prev?: string, next?: string): void;
}

interface QueryOptions {
  excludeEmpty?: boolean;         // Exclude empty records
  includeOfflineData?: boolean;   // Include offline data
  skip?: number;                  // Number of records to skip
  take?: number;                  // Maximum number of records to return
  orderBy?: string;               // Sort ascending by a field
  orderByDescending?: string;     // Sort descending by a field
  select?: string[];              // Return only selected fields
}
```

## Global Methods

### `k.content.getFolders()`

Gets the names of all content folders in the site.

**Parameters:** None.

**Returns:** `string[]`.

```ts
k.api.get(() => {
    const folders = k.content.getFolders();
    return { folders };
})
// Returns: { folders: ["Article", "Product", "News"] }
```

## Query Methods

### `all()`

Gets all content in a folder.

```ts
k.api.get(() => {
    const articles = k.content.Article.all();
    return { count: articles.length, articles };
})
```

**Parameters (QueryOptions):**

| Parameter | Type | Description |
|------|------|------|
| excludeEmpty | `boolean` | Whether to exclude empty records, default `false` |
| includeOfflineData | `boolean` | Whether to include offline data, default `false` |
| skip | `number` | Number of records to skip |
| take | `number` | Maximum number of records to return |
| orderBy | `string` | Sort ascending by the specified field |
| orderByDescending | `string` | Sort descending by the specified field |
| select | `string[]` | Read and return only the specified fields |

**Returns:** `Content[]`.

```ts
// Exclude empty records
k.api.get(() => {
    return k.content.Article.all({ excludeEmpty: true });
})

// Include offline content
k.api.get(() => {
    return k.content.Article.all({ includeOfflineData: true });
})

// Paginate, sort, and limit returned fields
k.api.get(() => {
    return k.content.Article.all({
        skip: 20,
        take: 10,
        orderByDescending: "creationDate",
        select: ["title", "slug", "creationDate"]
    });
})
```

### `get()`

Gets a single content item by ID or UserKey.

```ts
k.api.get(() => {
    const article = k.content.Article.get("article-id-or-userkey");
    return article;
})
```

**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| nameOrId | `string` | Content ID or UserKey |

**Returns:** `Content | null`.

### `find()`

Queries a single content item. Both string queries and object queries are supported.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `query` | `string | object` | yes | String condition or object query condition |
| `options` | `QueryOptions` | no | Query options |

**Returns:** `Content | null`.

**String query:**

```ts
k.api.get(() => {
    // Use a string condition
    const article = k.content.Article.find("title == 'Hello World'");
    return article;
})
```

**Supported operators:**
- `==` equals
- `!=` / `<>` not equals
- `>=` greater than or equal to
- `>` greater than
- `<=` less than or equal to
- `<` less than
- `&&` / `and` and
- `||` / `or` or
- `contains` contains
- `startwith` starts with

```ts
// Numeric fields are compared numerically; do not quote the number
k.api.get(() => {
    const adult = k.content.People.find("age > 23");
    return adult;
})

// Date fields support range comparisons, including system date fields
k.api.get(() => {
    const recent = k.content.Article.find(
        "creationDate >= '2026-01-01T00:00:00Z'"
    );
    return recent;
})

// Multiple conditions
k.api.get(() => {
    const article = k.content.Article.find("title == 'Hello' && author == 'John'");
    return article;
})

// Fuzzy search
k.api.get(() => {
    const article = k.content.Article.find("title contains 'World'");
    return article;
})
```

Kooboo compares values according to the condition value type: unquoted numbers are treated as numbers, date fields are treated as date-time values, and strings are compared as text. System fields such as `userKey`, `slug`, `creationDate`, and `lastModified` can also be used in query conditions.

**Object query:**

```ts
// Use an object-query operator for a numeric comparison
k.api.get(() => {
    return k.content.People.find({
        age: { $gt: 23 }
    });
})

// Compare a system date field
k.api.get(() => {
    return k.content.Article.findAll({
        creationDate: { $gte: "2026-01-01T00:00:00Z" }
    });
})

// Combine conditions with $and and $or
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

Object-query operators start with `$` and are written directly in the query object; no separate operator lookup or declaration is required. Multiple fields at the same level are combined with AND. Use `$and` or `$or` when explicit grouping is needed. The same object syntax also applies to `findAll()`.

**find + options:**

```ts
k.api.get(() => {
    const article = k.content.Article.find(
        { title: "Hello" },
        { includeOfflineData: true }
    );
    return article;
})
```

### `findAll()`

Queries multiple content items. The usage is the same as `find()`, but it returns an array.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `query` | `string | object` | yes | String condition or object query condition |
| `options` | `QueryOptions` | no | Query options |

**Returns:** `Content[]`.

```ts
k.api.get(() => {
    const articles = k.content.Article.findAll({ category: "news" });
    return { count: articles.length, articles };
})

// Use an object condition for an OR query
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

### `findAllByCategory()`

Returns content from the current folder that is linked to a specific category item. Identify the category by its content folder name and the category item's `slug`, `userKey`, or ID.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `categoryFolder` | `string` | yes | Name of the content folder that stores the category item |
| `categorySlugOrId` | `string` | yes | Category item `slug`, `userKey`, or ID |
| `options` | `QueryOptions` | no | Pagination, sorting, field selection, and offline-data options |

**Returns:** `Content[]`. Returns an empty array when the category does not exist, belongs to another folder, or has no linked content.

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

In this example, `Tags` is the category content folder name and `technology` is the `slug` or `userKey` of a category item in that folder. You can also pass the category item's ID as the second argument.

## CRUD Methods

### `add()`

Creates new content.

```ts
k.api.post(() => {
    const article = k.content.Article.add({
        title: "My Article",
        content: "This is the article content",
        author: "John Doe",
        views: 0
    });
    return { id: article.id, title: article.title };
})
```

**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| content | `object` | Content object, where keys are field names and values are field values |

**Returns:** `Content`, the created content object.

**System fields (optional):**

| Field | Type | Description |
|------|------|------|
| userKey | `string` | User-defined unique identifier |
| online | `boolean` | Whether it is online, default `true` |
| order / sequence | `number` | Sort order |

### `update()`

Updates existing content.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `content` | `Content` | yes | Full content object fetched and modified from `find`, `get`, `all`, or similar methods |

**Returns:** `void`.

```ts
k.api.post(() => {
    // Query the content to update first
    const article = k.content.Article.find({ title: "Old title" });

    // Modify fields
    article.title = "New title";
    article.views = article.views + 1;

    // Perform the update
    k.content.Article.update(article);

    return { success: true, article };
})
```

::: warning Note
The `update` method requires the full content object returned by `find`, `get`, `all`, and similar methods, not a newly created object. The content object contains system fields such as `id` and `version`, and those fields are used to identify the record to update.
:::

### `delete()`

Deletes content.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `contentOrId` | `string | Content` | yes | Content ID or full content object |

**Returns:** `void`.

```ts
k.api.post(() => {
    const article = k.content.Article.find({ title: "Article to delete" });

    if (article) {
        k.content.Article.delete(article.id);
        return { success: true };
    }
    return { success: false, error: "Article not found" };
})

// You can also pass the content object directly
k.api.post(() => {
    const article = k.content.Article.find({ title: "Article to delete" });

    if (article) {
        k.content.Article.delete(article);
        return { success: true };
    }
    return { success: false };
})
```

## Sorting Methods

### `move()`

Adjusts the sort position of content in a list.

```ts
k.api.post(() => {
    // Move the article to a position after targetId
    // prev: reference element ID
    // next: optional ID after which to insert
    k.content.Article.move(sourceId, prevId, nextId);

    return { success: true };
})
```

**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| source | `string` | Content ID to move |
| prev | `string` | Reference element ID (insert after this element) |
| next | `string` | Optional; insert after this ID |

**Returns:** `void`.

## Multilingual Support

### Set the language context

Set the current request culture with `k.request.setCulture()` before querying content.

```ts
k.api.get(() => {
    // Set to Chinese
    k.request.setCulture("zh");
    const zhArticle = k.content.Article.get("article-id");

    // Set to English
    k.request.setCulture("en");
    const enArticle = k.content.Article.get("article-id");

    return {
        zh: zhArticle.title,
        en: enArticle.title
    };
})
```

::: tip When to use
You must set the culture before calling any `k.content` query method.
:::

### Multilingual fields

When a field is marked as **multilingual** in the content type, Kooboo returns the translated value for the current language context.

## Content Structure

### Content system fields

All content objects contain the following system fields:

| Field | Type | Description |
|------|------|------|
| id | `string` | Unique content identifier (GUID) |
| userKey | `string` | User-defined unique identifier |
| slug | `string` | Query and output alias of `userKey` |
| lastModified | `string` | Last modified time (ISO 8601) |
| creationDate | `string` | Creation time (ISO 8601) |
| parentId | `string` | Parent ID |
| version | `number` | Version number |
| sequence | `number` | Sort sequence |
| online | `boolean` | Whether it is online (`true` = online, `false` = offline) |

You can reference system fields directly in `find()` and `findAll()` conditions. The most common system-field queries are:

| Field | Comparisons | Example |
|------|-------------|---------|
| `userKey` / `slug` | Exact matching; `findAll()` also supports inequality, contains, and starts-with | `slug == 'release-notes'` |
| `creationDate` | Date equality, inequality, and range comparisons | `creationDate >= '2026-01-01T00:00:00Z'` |
| `lastModified` | Date equality, inequality, and range comparisons | `lastModified < '2026-08-01T00:00:00Z'` |

### Example content object

```json
{
    "title": "Article Title",
    "content": "Article content",
    "author": "Author name",
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

## Query Operator Details

Both `find()` and `findAll()` support object queries and string queries. Object queries use `$...` operator keys, while string queries use expression operators such as `>`, `contains`, and `&&`.

### Object-query operators

| Operator | Description | Example |
|--------|------|------|
| `$eq` | Equals; assigning a field value directly is also an equality query | `{ status: { $eq: "active" } }` |
| `$ne` | Not equal | `{ status: { $ne: "deleted" } }` |
| `$gt` | Greater than | `{ age: { $gt: 23 } }` |
| `$gte` | Greater than or equal to | `{ age: { $gte: 18 } }` |
| `$lt` | Less than | `{ stock: { $lt: 10 } }` |
| `$lte` | Less than or equal to | `{ creationDate: { $lte: "2026-12-31T23:59:59Z" } }` |
| `$contains` | Contains, commonly used with string fields | `{ title: { $contains: "Kooboo" } }` |
| `$startswith` | Starts with a prefix, commonly used with string fields | `{ code: { $startswith: "SKU-" } }` |
| `$and` | All conditions must match | `{ $and: [{ age: { $gte: 18 } }, { status: "active" }] }` |
| `$or` | Any condition may match | `{ $or: [{ status: "active" }, { status: "pending" }] }` |

Write numeric values as numbers in object conditions. ISO 8601 strings are recommended for dates. Object operators are written directly in the query object and require no extra declaration.

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

### String-query comparison operators

| Operator | Description | Example |
|--------|------|------|
| `==` / `=` | Equals | `title == 'Hello'` |
| `!=` / `<>` | Not equals | `status != 'draft'` |
| `>` | Greater than | `age > 23` |
| `>=` | Greater than or equal to | `price >= 10` |
| `<` | Less than | `stock < 5` |
| `<=` | Less than or equal to | `creationDate <= '2026-12-31T23:59:59Z'` |

Numbers, dates, and strings use the same comparison operators, but Kooboo compares them according to their value type. Do not quote numeric values; use ISO 8601 for dates.

### String-query text operators

| Operator | Description | Example |
|--------|------|------|
| `contains` | Contains | `title contains 'news'` |
| `startwith` | Starts with | `name startwith 'A'` |

### String-query logical operators

| Operator | Description | Example |
|--------|------|------|
| `&&` / `and` | And | `a == 1 && b == 2` |
| `\|\|` / `or` | Or | `a == 1 \|\| a == 2` |

### String OR query examples

```ts
// Use ||
k.api.get(() => {
    return k.content.Article.findAll(
        "title == 'News1' || title == 'News2'"
    );
})

// Use or
k.api.get(() => {
    return k.content.Article.findAll(
        "status == 'published' or status == 'scheduled'"
    );
})
```

## QueryOptions Details

### excludeEmpty

Excludes records where all user fields are empty.

```ts
k.api.get(() => {
    // Return all content with at least one field value
    return k.content.Article.all({ excludeEmpty: true });
})
```

### includeOfflineData

Includes offline content where the `online` field is `false`. Offline content is excluded by default.

```ts
k.api.get(() => {
    // Return all content including offline content
    return k.content.Article.all({ includeOfflineData: true });
})
```

### skip and take

Use `skip` to omit leading records and `take` to limit the result size. They apply to `all()`, `findAll()`, and `findAllByCategory()`.

```ts
k.api.get(() => {
    return k.content.Article.findAll("age > 23", {
        skip: 20,
        take: 10
    });
})
```

### orderBy and orderByDescending

Pass a field name to sort ascending or descending. Choose one direction per query. The field can be a custom field or a system field such as `creationDate` or `lastModified`.

```ts
k.api.get(() => {
    return k.content.Article.findAll("age > 23", {
        orderByDescending: "creationDate",
        take: 10
    });
})
```

### select

Reads and returns only the specified fields, reducing unnecessary content-field reads.

```ts
k.api.get(() => {
    return k.content.Article.all({
        select: ["title", "slug", "creationDate"]
    });
})
```

## Related Docs

- [k.DB](../database/) - Database operations
- [k.request](../request/) - Request object
