# Non-SQL JSON Condition

## 1. Goal

JSON5 condition expressions are used for:

- `query` `where`
- `when` / `else-if` `test`

They avoid SQL strings and support:

- structured generation and validation;
- consistency with `k-data` template syntax;
- stable execution and compatibility across data sources.

---

## 2. Supported Operator Range

Only these operators are currently supported:

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

All operators other than these 10 are outside the current supported range.

---

## 3. Basic Structure

### Direct Field Form (Equality Match)

```json5
{ status: 'published' }
```

Equivalent to:

```json5
{ status: { $eq: 'published' } }
```

### Field + Operator

```json5
{ age: { $gte: 18 } }
```

### Combined Conditions

```json5
{
	$and: [
		{ status: { $eq: 'published' } },
		{ title: { $contains: keyword } }
	]
}
```

---

## 4. Operator Descriptions and Examples

### `$eq`

Equal to.

```json5
{ id: { $eq: '1234' } }
```

### `$ne`

Not equal to.

```json5
{ status: { $ne: 'deleted' } }
```

### `$gt`

Greater than.

```json5
{ score: { $gt: 90 } }
```

### `$gte`

Greater than or equal to.

```json5
{ price: { $gte: 100 } }
```

### `$lt`

Less than.

```json5
{ stock: { $lt: 10 } }
```

### `$lte`

Less than or equal to.

```json5
{ level: { $lte: 3 } }
```

### `$contains`

Contains match (normally used for fuzzy string containment).

```json5
{ title: { $contains: 'hello' } }
```

### `$startswith`

Prefix match.

```json5
{ code: { $startswith: 'SKU-' } }
```

### `$and`

All conditions must be true.

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

At least one condition is true.

```json5
{
	$or: [
		{ role: { $eq: 'admin' } },
		{ role: { $eq: 'editor' } }
	]
}
```

---

## 5. Use in `query` and `when`

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
