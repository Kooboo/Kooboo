# Non-SQL JSON Condition

## 1. 目标

JSON5 条件表达式用于：

- `query` 的 `where`
- `when` / `else-if` 的 `test`

不写 SQL 字符串，便于：

- 结构化生成与校验
- 与 `k-data` 模板语法统一
- 稳定执行与跨数据源兼容

---

## 2. 操作符支持范围

当前仅支持以下操作符：

- `$and`
- `$contains`
- `$eq`
- `$gt`
- `$gte`
- `$lt`
- `$lte`
- `$ne`
- `$or`
- `$startswith`

除以上 10 个操作符外，其它操作符均不在当前支持范围内。

---

## 3. 基础结构

### 字段直写（等值匹配）

```json5
{ status: 'published' }
```

等价于：

```json5
{ status: { $eq: 'published' } }
```

### 字段 + 操作符

```json5
{ age: { $gte: 18 } }
```

### 组合条件

```json5
{
	$and: [
		{ status: { $eq: 'published' } },
		{ title: { $contains: keyword } }
	]
}
```

---

## 4. 操作符说明与示例

### `$eq`

等于。

```json5
{ id: { $eq: '1234' } }
```

### `$ne`

不等于。

```json5
{ status: { $ne: 'deleted' } }
```

### `$gt`

大于。

```json5
{ score: { $gt: 90 } }
```

### `$gte`

大于等于。

```json5
{ price: { $gte: 100 } }
```

### `$lt`

小于。

```json5
{ stock: { $lt: 10 } }
```

### `$lte`

小于等于。

```json5
{ level: { $lte: 3 } }
```

### `$contains`

包含匹配（通常用于字符串模糊包含）。

```json5
{ title: { $contains: 'hello' } }
```

### `$startswith`

前缀匹配。

```json5
{ code: { $startswith: 'SKU-' } }
```

### `$and`

所有条件同时成立。

```json5
{
	$and: [
		{ age: { $gte: 18 } },
		{ age: { $lte: 60 } },
		{ status: { $eq: 'active' } }
	]
}
```

### `$or`

任一条件成立。

```json5
{
	$or: [
		{ role: { $eq: 'admin' } },
		{ role: { $eq: 'editor' } }
	]
}
```

---

## 5. 在 `query` 与 `when` 中使用

```html
<let as="keyword" source="queryString" from="keyword" default='""' />
<let as="min_age" source="queryString" from="min_age" default="18" />

<query
		as="user_page"
		source="content"
		resource="user"
		action="paged"
		where="{
				$and:[
						{age:{$gte:min_age}},
						{$or:[
								{name:{$contains:keyword}},
								{email:{$startswith:keyword}}
						]},
						{status:{$ne:'disabled'}}
				]
		}"
		page-number="1"
		page-size="20"
></query>

<when test="{
		$and:[
				{status:{$eq:'published'}},
				{$or:[
						{title:{$contains:keyword}},
						{slug:{$startswith:keyword}}
				]}
		]
}">
		<let as="mode" default="'matched'" />
		<else-if test="{status:{$eq:'draft'}}">
				<let as="mode" default="'draft'" />
		</else-if>
		<else>
				<let as="mode" default="'other'" />
		</else>
</when>
```


