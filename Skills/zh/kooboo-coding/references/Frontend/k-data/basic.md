# k-data 基础语法

## 1. 目标

`k-data` 使用纯标签 + 属性表达数据流程，避免类 JS 模板逻辑，便于：

- 服务端执行器做稳定解析和 SSR 渲染
- AI 生成时遵循固定结构，减少语义漂移

**SSR 动态化时，k-data 是默认取数方式**（见 `Scenario/site-building/dynamic-delivery.md` S9）。`<script env="server">` 只用于 k-data 无法表达的服务端逻辑或 `type="module"` 导入；普通模板容器不需要 `env="server"`。

---

## 1.1 取数路线决策（动态化必读）

实现 SSR 数据绑定时，**先问能否用 k-data，再考虑 env=server 脚本**。

### 用 k-data（默认）

- `source="commerce"` / `content` / `sqlite` / `mysql` / `sqlserver` / `culture` 的 `<query>`（list、paged、item）
- `source="queryString"` / `cookie` / `header` / `path` / `form` 的 `<let>`
- `<map>` 整形列表、`<when>` 分支选查询、`<let use="...">` 做日期/金额/数学
- 模板层只消费通过 `<query export>` 或 `<let export>` 暴露的变量

### 降级到 `<script env="server">`（须能说明原因）

- `import` CodeBlock / Service（须 `type="module"` + `export`）
- 调用的 `k.*` API **没有**对应 k-data `source` / `resource`（如部分站点级工具 API）
- 无法拆成声明式 query/let 链的复杂命令式编排

### 反模式（禁止）

```html
<!-- ❌ 动态化时用脚本直接查库，应改为 k-data query -->
<script env="server">
    var products = k.commerce.product.list({ page: 1 })
</script>
<div k-for="item in products">
    <span k-content="item.title"></span>
</div>
```

```html
<!-- ✅ 同等语义应用 k-data -->
<k-data>
    <query as="products" source="commerce" resource="product" action="list" export></query>
</k-data>
<div k-for="item in products">
    <span k-content="item.title"></span>
</div>
```

数据流优先在 `<k-data>` 内完成，模板层直接通过原生 `href` 插值、`k-for`、`k-content` 或 `k-attribute` 消费 `export` 暴露的变量。不要为了启用模板绑定给外层元素添加 `env="server"`。

---

## 2. 标签总览

顶级容器：

- `k-data`

变量、请求读取与函数调用：

- `let`

查询与整形：

- `query`
- `map`

条件分支：

- `when`
- `else-if`
- `else`

异常与跳转：

- `try`
- `catch`
- `redirect`

变量暴露：

- `query` / `let` 的 `export` 属性

---

## 3. 全局编写约定

1. 外部数据源统一使用 `source` / `resource` 表达。
2. 变量、字段路径、请求参数名统一使用 `from` 表达。
3. 分页参数统一使用 `page-number` 与 `page-size`。
4. 分页查询结果（`action="paged"`）进行列表映射时，统一使用 `map from="xxx.list"`。
5. 已知唯一标识并且语义是“取一条详情”时，直接使用 `action="item"`。内容项的名称 / ID 使用 `id="变量名"`；按自定义字段定位时使用 `where`。不要先执行 `list` 再用 `[0]` 模拟详情查询。
6. `select` 只在确实需要字段投影时使用；需要完整对象就省略。内容项通过名称 / ID 执行 `item` 查询时返回完整对象，详情页通常不需要声明 `select`。
7. Kooboo 页面动态路由参数（如 `/post/{slug}`）已进入渲染数据上下文，使用 `<let as="slug" from="slug"></let>` 读取。`source="path"` 对应 ASP.NET `Request.RouteValues`，不是 Kooboo 页面路由参数的默认读取方式。
8. 模板层只消费通过 `query` / `let` 的 `export` 属性暴露的变量。
9. 需要鉴权或高风险查询时，建议统一放在 `try/catch` 中。
10. 链接 URL 使用原生 `href` 与 `{...}` 插值，其它模板属性绑定使用 `k-attribute`，值绑定使用 `k-content`。
11. 只有业务语义确实是“有序结果的第一项”时，才对 `list` 结果使用 `<let from="xxx[0]" as="yyy">`；这不能替代唯一键的 `item` 查询。
12. `when` / `else-if` / `else` 是 `k-data` 内部标签，只能写在 `<k-data>` 的分支结构里；模板层条件渲染请使用 `k-if` / `k-else-if` / `k-else`。
13. 原生 `href` 与 `k-attribute` 中的 `{...}` 只用于变量 / 字段路径插值，不做数学运算或条件运算；需要预计算时，先在 `<k-data>` 里用 `<let use="...">` 生成结果变量。
14. `query` 不是 void element，必须显式写 `</query>`；禁止使用 `<query ... />`，否则后续 `k-data` 步骤可能被解析成 `query` 的子节点。
15. `query` 必须声明非空 `as`、`source`、`resource`、`action`；缺失或 source/action 不受支持时，运行时会静默不生成结果变量。
16. `export` 是 `query` / `let` 的布尔属性；推荐直接写 `export`，也支持 `export="true"`。`export="false"` 与省略属性都不会暴露变量。
17. `export` 暴露当前标签 `as` 指定的变量，因此带 `export` 的 `query` / `let` 必须声明非空 `as`。
18. `query`、`try`、`catch`、`redirect` 必须位于 `<k-data>` 内；DataEvaluator 只解析 `<k-data>` 的内部内容。

---

## 4. 语法说明

## `let`

`let` 用于定义变量、读取请求上下文、提取字段，或通过 `use` 调用函数。

```html
<let as="keyword" source="queryString" from="keyword" default=""></let>
<let as="token" source="cookie" from="id" default="123456"></let>
<let as="products" default="[]"></let>
<let from="token" as="payload" use="jwtDecode"></let>
<let from="payload.userId" as="userId"></let>
<let from="create_time" as="createdDate" use="dateFormat" params="'yyyy-MM-dd'"></let>
<let from="create_time" as="displayText" use="dateFormat" params="{format:'yyyy-MM-dd',fallback:'--'}"></let>
```

| 属性      | 必填 | 说明                                           |
| --------- | ---- | ---------------------------------------------- |
| `as`      | 是   | 输出变量名                                     |
| `from`    | 否   | 变量名、字段路径，或外部输入源数据     |
| `source`  | 否   | 外部输入来源，如 `queryString` / `cookie` / `header` / `path` / `form` |
| `use`     | 否   | 使用的函数名（见[functions.md](functions.md)）              |
| `params`  | 否   | 函数参数，传变量名或者JSON5字符串 |
| `default` | 否   | 默认值, 传变量名或者JSON5字符串 |
| `export`  | 否   | 布尔属性；暴露 `as` 变量给模板层 |

## `query`

执行查询并绑定结果变量。

```html
<query
    as="product_page"
    source="commerce"
    resource="product"
    action="paged"
    select="id,title,featuredImage,variants"
    where="{title:{$contains:'hello'}}"
    page-number="page_number"
    page-size="page_size"
    order-desc="price"
></query>
```

| 属性          | 必填 | 说明                                         |
| ------------- | ---- | -------------------------------------------- |
| `as`          | 是   | 查询结果变量名                               |
| `source`      | 是   | 数据源，如 `content` / `commerce` / `sqlite` / `mysql` \ `sqlserver` / `indexeddb` / `culture` |
| `resource`    | 是   | 资源名，如 `product` / `payment_records`     |
| `action`      | 是   | 查询动作，示例使用 `paged` / `item` / `list` |
| `select`      | 否   | 字段列表                                     |
| `where`       | 否   | JSON5过滤表达式 （见[condition.md](condition.md)）                |
| `order`       | 否   | 升序字段，如 `create_time`                   |
| `order-desc`  | 否   | 降序字段，如 `price`                         |
| `page-number` | 否   | 页码变量或整数                               |
| `page-size`   | 否   | 每页大小变量或整数                         |
| `export`      | 否   | 布尔属性；暴露 `as` 结果给模板层           |

详细说明 (见[query.md](query.md))。

## `map`

对数组或对象逐项处理，并输出新结果。

```html
<map from="product_page.items" as="products">
    <let from="id" as="id" ></let>
    <let from="title" as="title" ></let>
    <let from="featuredImage" as="featuredImage" ></let>
</map>
```

```html
<map from="filters" as="filter_keys">
    <let default="123" as="key" ></let>
</map>
```

```html
<map as="new_user">
    <let default="23" as="age" ></let>
    <let default="'jobs'" as="name" ></let>
</map>
```

| 属性   | 必填 | 说明             |
| ------ | ---- | ---------------- |
| `as`   | 是   | 输出变量名       |
| `from` | 否   | 源数组或对象变量,为空时返回新对象|

## `when` / `else-if` / `else`

条件分支结构。

```html
<when test="{mode:'published'}">
    <query ... ></query>
    <else-if test="{mode:'unpublished'}">
        <query ... ></query>
    </else-if>
    <else>
        <query ... ></query>
    </else>
</when>
```

| 属性   | 说明              |
| ------ |  ---------------- |
| `test` | JSON5条件表达式 （见[condition.md](condition.md)） |

注意：

- `when` / `else-if` / `else` 只在 `<k-data>` 内有效。
- 如果要控制模板层 DOM 是否渲染，请使用 `k-if` / `k-else-if` / `k-else`，不要把 `k-data` 的分支标签直接写到模板层。

## `try` / `catch`

异常处理结构。

```html
<try>
    <query ... ></query>
</try>
<catch>
    <let as="has_error" default="true"></let>
</catch>
```

## `redirect`

重定向。

```html
<redirect to="'/login'"></redirect>
```

| 属性   | 说明              |
| ------ |  ---------------- |
| `to` | 变量名或JSON5字符串 |

## `export` 属性

在需要由模板层使用的 `query` 或 `let` 上添加 `export`。它会暴露当前标签 `as` 指定的变量。

```html
<query as="products" source="commerce" resource="product" action="list" export></query>
<let as="keyword" source="queryString" from="keyword" default="''" export></let>
```

## 模板层属性绑定

链接 URL 使用原生 `href` 与 `{...}` 插值。其它模板层属性使用 `k-attribute`，并与 `k-for` / `k-if` / `k-content` 保持同一语法体系。
详细语法与完整示例见 [template-binding-syntax.md](../template-binding-syntax.md)。

常用示例：

```html
<!-- 链接 URL 使用原生 href -->
<a href="{linkHref}">About Us</a>
<a href="/detail/{itemId}">Detail</a>

<!-- 其它属性使用 k-attribute -->
<img k-attribute="src imageUrl">

<!-- 多属性用 ; 分隔 -->
<img k-attribute="src imageUrl; alt 'Logo'">

<!-- class 对象条件语法 -->
<div k-attribute="class {active: isActive; checked: isChecked}">Content</div>

<!-- 静态 class + 动态 class 共存，渲染时自动合并 -->
<a class="text-xs font-medium px-2 py-1"
   k-attribute="class {'text-white bg-blue-600': isActive, 'text-gray-500': !isActive}">
  Button
</a>
```

约定：

- 链接 URL 直接写原生 `href`；动态值使用 `{变量}` 或 `{对象.字段}`。
- 单个或多个非 `href` 属性使用 `k-attribute`，多个属性之间用 `;` 分隔。
- 字面量字符串请加引号（如 `alt 'logo'`），避免被解析为变量。
- 原生 `href` 或 `k-attribute` 的花括号插值应是变量名或字段路径，例如 `{product.id}`、`{product.title}`。
- 不要在原生 `href` 或 `k-attribute` 中直接写 `{pageIndex - 1}`、`{price * qty}` 这类运算；应先在 `<k-data>` 中通过 `<let use="add|subtract|multiply|divide|modulo">` 预计算，再绑定结果变量。
- 静态 `class="..."` 和 `k-attribute="class {...}"` 可共存，渲染时两者自动合并，互不覆盖。
- 新增文档与示例中，链接使用原生 `href`，其它动态属性使用 `k-attribute`。

---

## 5. 最小完整示例

```html
<k-data>
    <let as="keyword" source="queryString" from="keyword" default="''"></let>
    <let as="page_number" source="queryString" from="page_number" default="1"></let>
    <let as="page_size" source="queryString" from="page_size" default="10"></let>
    <query
        as="query_result"
        source="content"
        resource="user"
        action="paged"
        select="id,age,name,birthday"
        where="{age:{$gt:23},name:{$contains:'lily'}}"
        page-number="page_number"
        page-size="page_size"
        order-desc="create_time"
        export
    ></query>
</k-data>
<div>
    <div k-for="user in query_result.list">
        <p k-content="user.name"></p>
        <p k-content="user.birthday"></p>
    </div>
    <p>
        共 <span k-content="query_result.total"></span> 条，
        第 <span k-content="query_result.pageIndex"></span> / <span k-content="query_result.pageCount"></span> 页，
        每页 <span k-content="query_result.pageSize"></span> 条
    </p>
</div>
```
