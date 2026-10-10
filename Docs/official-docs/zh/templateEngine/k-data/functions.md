# k-data 内置函数

> `<let use="函数名" params="...">` 可用函数

## 调用约定

每个 `<let>` 都在已有值上完成一次转换：

```html
<let from="create_time" as="createdDate" use="dateFormat" params="'yyyy-MM-dd'" />
<let from="title" as="shortTitle" use="truncate" params='{"length":20,"ellipsis":"..."}' />
<let from="amount" as="amountText" use="currencyFormat" params='{"currency":"USD"}' />
<let from="page_number" as="prev_page" use="subtract" params="1" />
```

| 属性 | 说明 |
|------|------|
| `from` | 输入值或字段路径 |
| `as` | 输出变量名 |
| `use` | 下表函数名 |
| `params` | JSON5：单值、对象，或格式字符串（外层可用单引号包裹） |

函数失败时多数返回 `null` 或原值，不抛到模板层。

## 函数总览

| 分类 | 函数 |
|------|------|
| 鉴权 | `jwtDecode` |
| 日期 | `dateFormat` |
| 货币 | `currencyFormat` |
| 数学 | `add`、`subtract`、`multiply`、`divide`、`modulo` |
| 文本 | `trim`、`lowercase`、`uppercase`、`truncate` |

## dateFormat

| `params` | 说明 |
|----------|------|
| `'"yyyy-MM-dd"'` | 格式模板 |
| `'{"format":"yyyy-MM-dd","fallback":"--"}'` | 带兜底 |

输入：`DateTime`、可解析日期字符串等。

## currencyFormat

| `params` | 说明 |
|----------|------|
| 省略 | 默认 `CNY` |
| `'"USD"'` 或 `'{"currency":"EUR"}'` | 指定货币代码 |

不可转数字时返回原输入。

## add / subtract / multiply / divide / modulo

| 函数 | `params` | 说明 |
|------|----------|------|
| 均可 | `'"3"'`、`'2'` 或 `'{"value":5}'` | 右操作数 |
| `divide`、`modulo` | 右操作数为 `0` | 返回 `null` |

分页链接示例（勿在 `k-attribute` 里写 `{pageIndex - 1}`）：

```html
<let as="page_number" source="queryString" from="page_number" default="1" />
<let from="page_number" as="prev_page_number" use="subtract" params="1" />
<let from="page_number" as="next_page_number" use="add" params="1" />
<a k-attribute="href /products?page_number={prev_page_number}">上一页</a>
```

## trim / lowercase / uppercase / truncate

```html
<let from="keyword" as="normalizedKeyword" use="trim" />
<let from="title" as="shortTitle" use="truncate" params='{"length":20,"ellipsis":"..."}' />
```

`truncate`：`length <= 0` 返回空字符串。

## jwtDecode

解析 JWT；可通过 `params` 提供密钥进行验签。解析失败时可结合条件分支执行重定向。

```html
<k-data>
    <let as="token" source="cookie" from="access_token" default="''" />
    <let from="token" as="payload" use="jwtDecode" params='{"secret":"your-secret"}' export />
    <when test="{payload:{$eq:null}}">
        <redirect to="'/login'" />
    </when>
</k-data>
```

- 失败（空 token、格式错误、验签失败）：`null`
- `when test` 使用 JSON5 条件，不使用 `payload == null` 这类普通表达式。

## 相关文档

- [标签语法](./tags.md#let)
- [条件表达式](./condition.md)
