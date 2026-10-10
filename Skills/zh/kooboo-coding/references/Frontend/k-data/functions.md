# k-data functions

本文档定义 `k-data` 中 `let use="..."` 可调用的内置函数语义。
目标是覆盖 SSR 常见展示场景（鉴权解析、日期格式化、金额与文本处理、基础数学运算），**优先在 k-data 内完成，避免把可声明式表达的逻辑降级到 `<script env="server">`**（见 [basic.md](basic.md)「取数路线决策」）。

---

## 调用方式

```html
<let as="token" source="cookie" from="access_token" default='""' />
<let from="token" as="payload" use="jwtDecode" />
<let from="payload.userId" as="userId" />
<let from="create_time" as="createDateText" use="dateFormat" params='"yyyy-MM-dd"' />
<let from="create_time" as="displayText" use="dateFormat" params='{"format":"yyyy-MM-dd","fallback":"--"}' />
<let from="amount" as="amountText" use="currencyFormat" params='{"currency":"USD"}' />
<let from="title" as="shortTitle" use="truncate" params='{"length":20,"ellipsis":"..."}' />
<let from="amount" as="total" use="add" params='"12.5"' />
<let from="total" as="discounted" use="subtract" params='{"value":2.5}' />
<let from="discounted" as="taxed" use="multiply" params='{"value":1.06}' />
<let from="taxed" as="avg" use="divide" params='2' />
<let from="avg" as="remainder" use="modulo" params='3' />
```

约定：

- `from`：输入变量名、字段路径，或请求参数名 / cookie 名
- `source`：外部输入来源，如 `querystring` / `cookie` / `header` / `path` / `form`
- `as`：输出变量名
- `use`：函数名（必须在本文件定义）
- `params`：函数参数，传 JSON 序列化内容；字符串、数组、对象等需要内层双引号时，外层再使用单引号包裹
- 单参数场景可直接传 JSON 值；多参数场景传 JSON 对象，key 为参数名，value 为参数值

---

## 函数总览

| 分类 | 函数 |
| --- | --- |
| 鉴权 | `jwtDecode` |
| 日期 | `dateFormat` |
| 货币/数字 | `currencyFormat` |
| 数学 | `add`、`subtract`、`multiply`、`divide`、`modulo` |
| 文本 | `trim`、`lowercase`、`uppercase`、`truncate` |

---

## 鉴权类函数

## `jwtDecode`

用途：

- 从 JWT token 中解析用户标识，用于后续 sqlite / 业务查询。

输入：

- `from` 指向的 token 字符串。

参数：

- `params` 可选。
- `params` 为字符串时，作为密钥进行验签。
- `params` 为对象时，支持 `secret` 或 `key` 作为密钥字段。

输出：

- 成功时将解析结果对象写入 `as` 变量（示例中为 `payload`）。

失败行为：

- token 为空、格式非法、验签失败或解析失败时，返回 `null`，不抛异常。

示例：

```html
<k-data>
    <let as="token" source="cookie" from="access_token" default='""' />
    <let from="token" as="payload" use="jwtDecode" params='{"secret":"your-256-bit-secret"}' />
    <when test="{payload:{$eq:null}}">
        <redirect to="'/login'" />
    </when>
</k-data>
```

---

## 日期类函数

## `dateFormat`

用途：

- 将时间值格式化为指定展示格式。

输入：

- `from` 指向 `DateTime`、`DateTimeOffset`、可解析日期字符串，或可转日期值。

参数：

- `params` 传格式模板字符串，例如 `'"yyyy-MM-dd"'`、`'"yyyy-MM-dd HH:mm"'`、`'"yyyy-MM-dd HH:mm:ss"'`。
- 也可传对象参数：`params='{"format":"yyyy-MM-dd","fallback":"--"}'`。

输出：

- 正常时返回格式化后的日期字符串。
- `format` 无效或值不可转换时，返回 `fallback`（若提供）或原始输入。

示例：

```html
<let from="create_time" as="createdDate" use="dateFormat" params='"yyyy-MM-dd"' />
<let from="update_time" as="updatedAtText" use="dateFormat" params='"yyyy-MM-dd HH:mm:ss"' />
<let from="publish_time" as="publishDateText" use="dateFormat" params='{"format":"yyyy-MM-dd","fallback":"--"}' />
```

---

## 货币/数字类函数

## `currencyFormat`

输入：

- 数字或可转数字字符串。

参数：

- `params` 为字符串时，作为货币代码（如 `"USD"`）。
- `params` 为对象时，支持 `currency` 或 `code` 字段（如 `{"currency":"EUR"}`）。
- 未传货币代码时，默认 `CNY`。

输出：

- 使用货币默认小数位进行格式化。
- 已知币种输出符号（如 `$1,234.50`、`¥1,234.50`）。
- 未知币种输出代码前缀（如 `ABC 1,234.50`）。
- 输入不可转数字时返回原始输入。

示例：

```html
<let from="amount" as="amountText" use="currencyFormat" />
<let from="amount" as="amountUsd" use="currencyFormat" params='"USD"' />
<let from="amount" as="amountEur" use="currencyFormat" params='{"currency":"EUR"}' />
```

---

## 数学类函数

适用函数：`add`、`subtract`、`multiply`、`divide`、`modulo`。

用途：

- 对 `from` 输入值与第二操作数进行基础数学运算。

输入：

- `from` 为左操作数，支持数字或可转换数字字符串。

参数：

- `params` 直接传单值时，作为右操作数，例如 `params='"3"'`、`params='2'`。
- `params` 传对象时，支持 `value` / `right` / `operand` 作为右操作数字段。

输出：

- 运算成功返回 `decimal` 结果。
- 任一操作数无法转换为数字时，返回原始 `from`。
- `divide` 与 `modulo` 在右操作数为 `0` 时返回 `null`，不抛异常。

示例：

```html
<let from="price" as="plusFee" use="add" params='{"value":5}' />
<let from="plusFee" as="minusCoupon" use="subtract" params='2' />
<let from="minusCoupon" as="withTax" use="multiply" params='"1.13"' />
<let from="withTax" as="avg" use="divide" params='3' />
<let from="orderNo" as="bucket" use="modulo" params='10' />
```

分页、上一页 / 下一页等场景也应先在 `k-data` 内预计算，再交给模板层绑定：

```html
<let as="page_number" source="queryString" from="page_number" default="1" />
<let from="page_number" as="prev_page_number" use="subtract" params="1" />
<let from="page_number" as="next_page_number" use="add" params="1" />
<a href="/products?page_number={prev_page_number}">上一页</a>
<a href="/products?page_number={next_page_number}">下一页</a>
```

不要把 `product_paged.pageIndex - 1`、`total * rate` 这类表达式直接写进原生 `href` 或 `k-attribute` 的 `{...}` 中；模板插值只负责绑定已有值，不负责执行数学运算。

---

## 文本类函数

## `trim`

- 去除首尾空白字符。

## `lowercase`

- 转小写。

## `uppercase`

- 转大写。

## `truncate`

用途：

- 将文本裁剪到指定长度。

参数：

- 支持 `params='3'` 或 `params='"3"'` 直接指定长度。
- 对象参数仅支持：
    - `length`：最大长度。
    - `ellipsis`：可选后缀文本。

输出：

- 长度未提供时返回原值。
- `length <= 0` 时返回空字符串。
- `ellipsis` 长度大于等于 `length` 时，返回被截断的 `ellipsis`。

示例：

```html
<let from="keyword" as="normalizedKeyword" use="trim" />
<let from="status" as="statusCode" use="lowercase" />
<let from="title" as="titleUpper" use="uppercase" />
<let from="title" as="shortTitle" use="truncate" params='{"length":20,"ellipsis":"..."}' />
```
