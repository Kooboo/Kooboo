# k-data 标签语法

> `<k-data>` 内各标签的属性与组合方式

## `k-data`

数据流程根节点，子标签按顺序执行。

```html
<k-data>
    <!-- let / query / map / when / try / redirect -->
</k-data>
```

## `let`

使用 `as` 指定变量名，使用 `default` 设置默认值：

```html
<let as="page_size" default="10" />
```

`let` 还可以读取请求值、提取字段或调用函数：

```html
<let as="keyword" source="queryString" from="keyword" default="''" />
<let from="list[0]" as="firstItem" />
<let from="create_time" as="createdDate" use="dateFormat" params="'yyyy-MM-dd'" />
```

| 属性 | 必填 | 说明 |
|------|------|------|
| `as` | 是 | 输出变量名 |
| `from` | 否 | 变量名、字段路径，或配合 `source` 的请求字段 |
| `source` | 否 | `queryString` / `cookie` / `header` / `path` / `form`（大小写不敏感） |
| `use` | 否 | 内置函数名，见 [内置函数](./functions.md) |
| `params` | 否 | 函数参数：JSON5 字符串，或变量名 |
| `default` | 否 | 默认值（JSON5 或变量名） |
| `export` | 否 | 布尔属性；存在且不为 `false` 时，将 `as` 变量暴露给模板层 |

`action="list"` 只需第一条时：`<let from="items[0]" as="item" />`。

## `query`

执行查询并写入 `as` 变量。列表查询写法如下：

```html
<query
    as="articles"
    source="content"
    resource="article"
    action="list"
></query>
```

分页查询可指定过滤、页码、每页条数和排序：

```html
<query
    as="article_page"
    source="content"
    resource="article"
    action="paged"
    where="{title:'News'}"
    page-number="1"
    page-size="10"
    order-desc="create_time"
></query>
```

`<query>` 必须位于 `<k-data>` 内，并显式写结束标签 `</query>`。

| 属性 | 必填 | 说明 |
|------|------|------|
| `as` | 是 | 结果变量名 |
| `source` | 是 | 见 [query 数据源](./query.md) |
| `resource` | 是 | 内容文件夹名、commerce 资源类型、表名等 |
| `action` | 是 | `item` / `list` / `paged` |
| `id` | `item` 时 | 记录 id 或变量名（如商品 seoName） |
| `select` | 否 | 逗号分隔字段列表 |
| `where` | 否 | JSON5 条件，见 [条件表达式](./condition.md) |
| `order` | 否 | 升序字段 |
| `order-desc` | 否 | 降序字段 |
| `page-number` | `paged` 时 | 页码（变量名或整数） |
| `page-size` | `paged` 时 | 每页条数 |
| `export` | 否 | 布尔属性；存在且不为 `false` 时，将 `as` 结果暴露给模板层 |

### `action` 与返回结构

| action | 返回 |
|--------|------|
| `item` | 单条对象，无结果时为 `null` |
| `list` | 对象数组 |
| `paged` | `{ list, total, pageIndex, pageSize, pageCount }` |

## `map`

对数组逐项映射，或对空 `from` 构造新对象。

```html
<map from="articles" as="article_cards">
    <let from="id" as="id" />
    <let from="title" as="title" />
</map>
```

| 属性 | 必填 | 说明 |
|------|------|------|
| `as` | 是 | 输出变量名（数组或对象） |
| `from` | 否 | 源数组/对象；省略时根据子 `let` 生成新对象 |

## `when` / `else-if` / `else`

仅在 `<k-data>` 内分支；`test` 为 JSON5 条件。

```html
<when test="{status:{$eq:'published'}}">
    <query as="items" source="content" resource="blog" action="list"></query>
    <else-if test="{status:{$eq:'draft'}}">
        <let as="items" default="[]" />
    </else-if>
    <else>
        <redirect to="'/404'" />
    </else>
</when>
```

## `try` / `catch`

`try` 后紧跟同级 `catch`（解析器约定）。

```html
<try>
    <query as="orders" source="commerce" resource="order" action="list"></query>
</try>
<catch>
    <let as="orders" default="[]" />
</catch>
```

## `redirect`

服务端重定向。

```html
<redirect to="'/login'" />
```

| 属性 | 说明 |
|------|------|
| `to` | 目标路径：变量名或 JSON5 字符串（如 `"'/login'"`） |

## `export` 属性

`export` 是 `<query>` 和 `<let>` 的布尔属性。添加后，该标签通过 `as` 创建的变量可供 **k-data 外部的** 模板绑定使用。

```html
<query as="articles" source="content" resource="article" action="list" export></query>
<let as="keyword" source="queryString" from="keyword" default="''" export />
```

`export`、`export="true"` 都表示暴露变量；`export="false"` 或省略该属性时，变量只在当前 `<k-data>` 数据流程内可见。

## 相关文档

- [k-data 概述](./)
- [query 数据源](./query.md)
- [内置函数](./functions.md)
- [条件表达式](./condition.md)
