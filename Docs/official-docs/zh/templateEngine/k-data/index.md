# k-data

> 在 HTML 模板中用标签声明数据流程（**推荐**，替代已弃用的 `k-query`）

## 概述

**k-data** 用纯 HTML 标签（`<k-data>`、`<let>`、`<query>` 等）描述「读参数 → 查数据 → 整形 → 暴露变量」，在**服务端**执行。需要交给模板层使用的结果，在生成变量的 `<query>` 或 `<let>` 上添加 `export` 属性。

语法以 Kooboo **`kooboo-coding`** 技能包中 `references/Frontend/k-data/` 为准，本文档与其保持一致。

与历史 **`k-query`** 单行属性相比，k-data：

- 结构固定，便于 AI 生成与代码审查
- 支持分支、异常、重定向、字段映射
- `where` / `test` 使用 **JSON5 条件**，而非 SQL 字符串

::: tip 与模板绑定的分工
| 层级 | 语法 | 用途 |
|------|------|------|
| `<k-data>` 内 | `<let>`、`<query>`、`<when>`… | 准备数据 |
| 模板 DOM | `k-content`、`k-for`、`k-if`、`k-attribute` | 渲染 UI |

详见 [模板绑定语法](../binding/)。
:::

## 最小可运行示例

下面只完成一件事：查询 `article` 内容并显示标题。

```html
<k-data>
    <query
        as="articles"
        source="content"
        resource="article"
        action="list"
        export
    ></query>
</k-data>

<ul>
    <li k-for="article in articles">
        <span k-content="article.title"></span>
    </li>
</ul>
```

这段代码形成了最小闭环：

1. `<query>` 查询数据，并把结果保存为 `articles`。
2. `<query>` 上的 `export` 属性将 `articles` 暴露给模板。
3. `k-for` 遍历结果，`k-content` 显示标题。

::: tip 运行前提
站点中需要存在名为 `article` 的 Content Folder，并包含 `title` 字段。实际项目请替换为自己的资源名和字段名。
:::

## 相关主题

| 主题 | 说明 | 文档 |
|------|------|------|
| 字段与过滤 | 使用 `select`、`where` 控制查询结果 | [query 数据源](./query.md#content) |
| 请求参数 | 从 URL、Cookie、Header 等位置读取值 | [标签语法 · let](./tags.md#let) |
| 分页与排序 | 使用 `paged`、`page-number`、`order-desc` | [query 数据源 · paged](./query.md#paged) |
| 映射与格式化 | 调整字段结构和展示格式 | [标签语法 · map](./tags.md#map)、[内置函数](./functions.md) |
| 条件与异常 | 条件分支、异常处理和重定向 | [条件表达式](./condition.md)、[标签语法](./tags.md) |

## 编写约定

1. 外部输入用 `source` + `from`（如 `queryString`、`cookie`）。
2. 查询用 `source` + `resource` + `action`；分页统一 `page-number`、`page-size`。
3. `action="paged"` 的列表在 `map` 里用 `from="结果.list"`。
4. 模板层**只使用**通过 `export` 属性暴露的变量。
5. `<when>` / `<else-if>` / `<else>` 只能写在 `<k-data>` 内；DOM 条件用 `k-if` / `k-else-if` / `k-else`。
6. `k-attribute` 的 `{...}` 只做变量插值，不做运算；先 `<let use="add|subtract|...">` 再绑定。
7. `<query>` 不是 HTML void element，必须写结束标签 `</query>`，不要使用 `<query ... />`。

## 标签一览

| 标签 | 作用 |
|------|------|
| `k-data` | 数据流程根容器 |
| `let` | 变量、请求读取、函数调用 |
| `query` | 查询 content / commerce / 数据库等 |
| `map` | 数组或对象逐项映射 |
| `when` / `else-if` / `else` | k-data 内条件分支 |
| `try` / `catch` | 异常处理 |
| `redirect` | 重定向 |

`export` 是 `<query>` 和 `<let>` 的布尔属性，用于将其 `as` 变量暴露给模板层。

属性与用法见 [标签语法](./tags.md)。

## 文档导航

| 主题 | 说明 |
|------|------|
| [标签语法](./tags.md) | `let`、`query`、`map`、分支、变量暴露 |
| [query 数据源](./query.md) | content、commerce、sqlite、mysql、culture 等 |
| [内置函数](./functions.md) | `dateFormat`、`jwtDecode`、数学与文本 |
| [条件表达式](./condition.md) | JSON5 `where` / `test` |

## 与 KScript API 的关系

| 场景 | 文档 |
|------|------|
| 模板内声明式取数 | 本目录 k-data |
| 脚本里 CRUD、业务逻辑 | [k.content](../../api/content/index.md)、[k.DB](../../api/database/index.md)、[k.commerce](../../api/commerce/commerce.md) 等 |

## 相关文档

- [Page](../page/) · [View](../view/)
- [模板绑定语法](../binding/)
