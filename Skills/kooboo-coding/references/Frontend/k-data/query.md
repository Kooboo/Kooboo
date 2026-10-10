# query Syntax Guide


## Common TypeScript Types

```ts
type QueryPagedResult<T> = {
	list: T[];
	total: number;
	pageIndex: number;
	pageSize: number;
	pageCount: number;
};

type ContentRecord = {
	id: string;
	[key: string]: unknown;
};

type DatabaseRecord = {
	_id?: string;
	[key: string]: unknown;
};

type CommerceAttribute = {
	key: string;
	value: string;
};

type CommerceSelectedOption = {
	name: string;
	value: string;
};

type CommerceVariantOption = {
	name?: string;
	values?: string[];
};

type CommerceVariant = {
	id: string;
	createdAt?: string;
	updatedAt?: string;
	productId?: string;
	barcode?: string;
	sku?: string;
	image?: string;
	imageMeta?: string;
	price?: number;
	active?: boolean;
	selectedOptions?: CommerceSelectedOption[];
	weight?: number;
	inventory?: number;
	order?: number;
	sales?: number;
	tags?: string[];
};

type CommerceCurrency = {
	code?: string;
	symbol?: string;
	rate?: number;
};

type CommerceProduct = {
	id: string;
	title?: string;
	description?: string;
	featuredImage?: string;
	images?: string[];
	imageMetas?: string;
	active?: boolean;
	isDigital?: boolean;
	seoName?: string;
	tags?: string[];
	/** 当前商品全部变体的最低价。 */
	price?: number;
	attributes?: CommerceAttribute[];
	variants?: CommerceVariant[];
	variantOptions?: CommerceVariantOption[];
	currency?: CommerceCurrency;
};

type CommerceCategory = {
	id: string;
	title?: string;
	seoName?: string;
	description?: string;
	image?: string;
	active?: boolean;
	tags?: string[];
	parentId?: string;
};

type CommerceDiscountAllocation = {
	discountId?: string;
	amount?: number;
	code?: string;
	title?: string;
};

type CommerceCartLine = {
	variantId?: string;
	productId?: string;
	title?: string;
	options?: CommerceSelectedOption[];
	sku?: string;
	image?: string;
	originalPrice?: number;
	price?: number;
	discountAllocations?: CommerceDiscountAllocation[];
	quantity?: number;
	totalQuantity?: number;
	amount?: number;
	taxAmount?: number;
	inventory?: number;
	note?: string;
};

type CommerceCart = {
	id: string;
	contact?: string;
	country?: string;
	currency?: CommerceCurrency;
	discountCodes?: string[];
	activeDiscountCodes?: string[];
	note?: string;
	discountAllocations?: CommerceDiscountAllocation[];
	shippingAmount?: number;
	insuranceAmount?: number;
	subtotalAmount?: number;
	totalAmount?: number;
	taxAmount?: number;
	totalQuantity?: number;
	pointsDeductionAmount?: number;
	lines?: CommerceCartLine[];
};

type CommerceAddress = {
	address1?: string;
	address2?: string;
	city?: string;
	country?: string;
	firstName?: string;
	lastName?: string;
	phone?: string;
	province?: string;
	zip?: string;
};

type CommerceOrderLine = {
	id: string;
	totalAmount: number;
	taxAmount: number;
	originalAmount: number;
	quantity: number;
	totalQuantity: number;
	price: number;
	originalPrice: number;
	title: string;
	image: string;
	sku: string;
	note: string;
};

type CommerceOrder = {
	id: string;
	customer?: {
		id?: string;
		email?: string;
		firstName?: string;
		lastName?: string;
		phone?: string;
	};
	totalAmount?: number;
	taxAmount?: number;
	originalAmount?: number;
	shippingAmount?: number;
	insuranceAmount?: number;
	subtotalAmount?: number;
	originalSubtotalAmount?: number;
	totalQuantity?: number;
	currency?: string;
	paid?: boolean;
	paidAt?: string | null;
	paymentMethod?: string;
	delivered?: boolean;
	canceled?: boolean;
	cancelReason?: string;
	cancelAt?: string | null;
	createdAt?: string;
	updatedAt?: string;
	shippingAddress?: CommerceAddress;
	note?: string;
	lines?: CommerceOrderLine[];
	pointsDeductionAmount?: number;
};

type CultureLink = {
	key: string;
	name: string;
	url: string;
	isActive: boolean;
};
```

Read the paged-result count directly from `query_result.total`. If an alias is needed, use `<let as="total" from="query_result.total"></let>`; do not assign the entire `query_result` to `total`, and do not declare `from` twice on the same `<let>`, or the template may render CLR/Dictionary type text.

Choose the query action according to the result semantics:

- When a unique identifier is known and one detail object is needed, use `action="item"`; do not run `list` and then read `[0]`.
- Use `list` + `[0]` only when the business meaning is specifically “the first item in an ordered result”.
- `select` is optional field projection; omit it when a complete object is needed. An `item` query by content name / ID returns the complete object, so `select` is usually unnecessary.
- The general `query` location attributes are currently `id` and `where`: use `id="变量名"` for a content name / ID and `where="{field:{$eq:变量名}}"` for a custom field.


## content

For `content`, `resource` corresponds to the content-folder name.

### item

Example: a content detail Page whose URL is `/blog/{slug}` and whose content collection has a stable `slug` field.
```html
<k-data>
	<let as="slug" from="slug"></let>
	<query export as="article_detail" source="content" resource="article" action="item"
		where="{slug:{$eq:slug}}"></query>
</k-data>
<div>
	<a href="/blog/{article_detail.slug}">
		<h1 k-content="article_detail.title"></h1>
	</a>
	<div k-content="article_detail.body"></div>
</div>
```

Return type
```ts
type ContentItemResult = ContentRecord;
```

Notes:

- The `id` value may reference a content item's name / ID variable.
- The `slug` in a Kooboo page route such as `/blog/{slug}` is already in the rendering data context. When it is the content item's name / ID, use `<let as="slug" from="slug"></let>` and then pass `id="slug"`; when it is a custom field, use the same variable in `where="{slug:{$eq:slug}}"`.
- When `slug` is an ordinary content field rather than a name / ID, still use `action="item"` with `where="{slug:{$eq:slug}}"`; `item` returns the first matching record, so there is no need to switch to `list` and read `[0]`.
- List and paged result links should use the same path parameter, for example `/blog/{item.slug}`; do not move the canonical article identity into `/blog?slug=...` when a path route is available.
- Omit `select` when the detail needs complete fields; declare it only when a partial field projection is explicitly required.

### list

Example
```html
<k-data>
	<query export as="articles" source="content" resource="article" action="list" select="id,slug,title,body"
		where="{title:{$contains:'article1'}}"></query>
</k-data>
<div k-for="item in articles">
	<a href="/blog/{item.slug}">
		<h1 k-content="item.title"></h1>
	</a>
	<div k-content="item.body"></div>
</div>
```

Return type
```ts
type ContentListResult = ContentRecord[];
```

### paged

Example
```html
<k-data>
	<query export as="article_paged" source="content" resource="article" action="paged" select="id,slug,title,body"
		where="{title:{$contains:'article1'}}" page-number="1" page-size="10"></query>
</k-data>
<div k-for="item in article_paged.list">
	<a href="/blog/{item.slug}">
		<h1 k-content="item.title"></h1>
	</a>
	<div k-content="item.body"></div>
</div>
<p>
    共 <span k-content="article_paged.total"></span> 条，
    第 <span k-content="article_paged.pageIndex"></span> / <span k-content="article_paged.pageCount"></span> 页，
    每页 <span k-content="article_paged.pageSize"></span> 条
</p>
```

Return type
```ts
type ContentPagedResult = QueryPagedResult<ContentRecord>;
```

## commerce

For `commerce`, `resource` is not an arbitrary table name; it must be one of `product` / `category` / `cart` / `order`.

### product / item

Example
```html
<k-data>
	<let as="seoName" from="seoName"></let>
	<query export as="product_detail" source="commerce" resource="product" action="item"
		id="seoName"></query>
</k-data>
<div>
	<h1 k-content="product_detail.title"></h1>
</div>
```

Return type
```ts
type CommerceProductItemResult = CommerceProduct;
```

Notes:

- The `id` attribute must reference a real variable; do not use a documentation placeholder or omit it.
- The value can be a Commerce product id or a seoName. For a new PDP, prefer the Page URL `/product/{seoName}` and use `<let as="seoName" from="seoName"></let>`; list-detail links should use `/product/{item.seoName}` and `<query id="seoName">`.
- Use a query-string identity only when an existing public route must be preserved or the business explicitly requires it; do not default new PDPs to `/product?seoName=...`.
- After the detail link and detail View are complete, open a real product URL and confirm that the page does not render errors such as `Query commerce product item must provide id`.

### product / list

Example
```html
<k-data>
	<query export as="products" source="commerce" resource="product" action="list"
		where="{category:'category_id_or_seoname',title:{$contains:'shoe'}}"></query>
</k-data>
<div k-for="item in products">
	<h2 k-content="item.title"></h2>
</div>
```

Return type
```ts
type CommerceProductListResult = CommerceProduct[];
```

Notes

- `where` currently supports filtering by top-level product fields such as `id`, `title`, `featuredImage`, `images`, `active`, `seoName`, and `tags`.
- `where.category` supports filtering by category id or seoName.
- `where.category` always includes child categories by default; no additional `include-sub-category` setting is required.
- Use `item.price` directly for product lists and starting prices; it is the lowest price among all variants of the current product and is not guaranteed to equal `item.variants[0].price`.
- After the user selects a specific variant, use that variant for the exact price, inventory, and add-to-cart parameters.

### product / paged

Example
```html
<k-data>
	<query export as="product_paged" source="commerce" resource="product" action="paged"
		where="{category:'category_id_or_seoname',active:{$eq:true}}"
		page-number="1" page-size="12"></query>
</k-data>
<div k-for="item in product_paged.list">
	<h2 k-content="item.title"></h2>
</div>
```

Return type
```ts
type CommerceProductPagedResult = QueryPagedResult<CommerceProduct>;
```

### category / item

Example
```html
<k-data>
	<let as="category_id" source="queryString" from="id"></let>
	<query export as="category_detail" source="commerce" resource="category" action="item"
		id="category_id_or_seoname"></query>
</k-data>
<div>
	<h1 k-content="category_detail.title"></h1>
</div>
```

Return type
```ts
type CommerceCategoryItemResult = CommerceCategory;
```

### category / list

Example
```html
<k-data>
	<query export as="categories" source="commerce" resource="category" action="list"
		where="{title:{$contains:'summer'}}"></query>
</k-data>
<div k-for="item in categories">
	<span k-content="item.title"></span>
</div>
```

Return type
```ts
type CommerceCategoryListResult = CommerceCategory[];
```

Notes

- `where` supports filtering by `id`, `title`, `seoName`, `description`, `image`, `active`, `tags`, and `parentId`.

### Products Category Selector (All + active)

- Category queries must include `where="{active:{$eq:true}}"`.
- Product queries must include `active:{$eq:true}`.
- `All` means that no category condition is applied; do not use `all` as a category id/seoName.
- Use the same queryString category value to drive the query and active styling; reset the page number when the category changes.
- For the complete branching example, see `references/Scenario/site-building/commerce-dynamicization.md`.

### category / paged

Example
```html
<k-data>
	<query export as="category_paged" source="commerce" resource="category" action="paged"
		where="{active:{$eq:true}}" page-number="1" page-size="20"></query>
</k-data>
<div k-for="item in category_paged.list">
	<span k-content="item.title"></span>
</div>
```

Return type
```ts
type CommerceCategoryPagedResult = QueryPagedResult<CommerceCategory>;
```

### cart / item

Example
```html
<k-data>
	<query export as="cart_detail" source="commerce" resource="cart" action="item"></query>
</k-data>
<div>
	<span k-content="cart_detail.id"></span>
</div>
```

Return type
```ts
type CommerceCartItemResult = CommerceCart;
```

Notes

- `cart` supports only `item`.
- When `id` is omitted, the cart id is read from the current context first.

### order / item

Example
```html
<k-data>
	<let as="order_id" source="queryString" from="id"></let>
	<query export as="order_detail" source="commerce" resource="order" action="item"
		id="order_id"></query>
</k-data>
<div>
	<span k-content="order_detail.id"></span>
</div>
```

Return type
```ts
type CommerceOrderItemResult = CommerceOrder;
```

### order / list

Example
```html
<k-data>
	<query export as="orders" source="commerce" resource="order" action="list"></query>
</k-data>
<div k-for="item in orders">
	<span k-content="item.id"></span>
</div>
```

Return type
```ts
type CommerceOrderListResult = CommerceOrder[];
```

Notes

- The `where` parameter is not supported.
- The `order` and `order-desc` sorting parameters are not supported; orders are always sorted by creation time descending.

### order / paged

Example
```html
<k-data>
	<query export as="order_paged" source="commerce" resource="order" action="paged"
		page-number="1" page-size="20"></query>
</k-data>
<div k-for="item in order_paged.list">
	<span k-content="item.id"></span>
</div>
<p>
    共 <span k-content="order_paged.total"></span> 条，
    第 <span k-content="order_paged.pageIndex"></span> / <span k-content="order_paged.pageCount"></span> 页，
    每页 <span k-content="order_paged.pageSize"></span> 条
</p>
```

Return type
```ts
type CommerceOrderPagedResult = QueryPagedResult<CommerceOrder>;
```

Notes

- The `where` parameter is not supported.
- The `order` and `order-desc` sorting parameters are not supported; orders are always sorted by creation time descending.

## sqlite

For `sqlite`, `resource` corresponds to the table name.

### item

Example
```html
<k-data>
	<let as="record_id" source="queryString" from="id"></let>
	<query export as="payment_record" source="sqlite" resource="payment_records" action="item"
		id="record_id" select="id,amount,status"></query>
</k-data>
<div>
	<span k-content="payment_record.status"></span>
</div>
```

Return type
```ts
type SqliteItemResult = DatabaseRecord;
```

### list

Example
```html
<k-data>
	<query export as="payment_records" source="sqlite" resource="payment_records" action="list"
		select="id,amount,status" where="{status:{$eq:'paid'}}" order-desc="id"></query>
</k-data>
<div k-for="item in payment_records">
	<span k-content="item.amount"></span>
</div>
```

Return type
```ts
type SqliteListResult = DatabaseRecord[];
```

### paged

Example
```html
<k-data>
	<query export as="payment_record_paged" source="sqlite" resource="payment_records" action="paged"
		select="id,amount,status" where="{status:{$eq:'paid'}}"
		page-number="1" page-size="10" order-desc="id"></query>
</k-data>
<div k-for="item in payment_record_paged.list">
	<span k-content="item.amount"></span>
</div>
```

Return type
```ts
type SqlitePagedResult = QueryPagedResult<DatabaseRecord>;
```

## mysql

`mysql` uses the same parameter structure as `sqlite`, and `resource` corresponds to the table name.

### item

Example
```html
<k-data>
	<let as="record_id" source="queryString" from="id"></let>
	<query export as="user_detail" source="mysql" resource="users" action="item"
		id="record_id" select="id,name,email"></query>
</k-data>
<div>
	<span k-content="user_detail.email"></span>
</div>
```

Return type
```ts
type MysqlItemResult = DatabaseRecord;
```

### list

Example
```html
<k-data>
	<query export as="users" source="mysql" resource="users" action="list"
		select="id,name,email" where="{name:{$contains:'tom'}}" order="name"></query>
</k-data>
<div k-for="item in users">
	<span k-content="item.name"></span>
</div>
```

Return type
```ts
type MysqlListResult = DatabaseRecord[];
```

### paged

Example
```html
<k-data>
	<query export as="user_paged" source="mysql" resource="users" action="paged"
		select="id,name,email" where="{name:{$contains:'tom'}}"
		page-number="1" page-size="10" order="name"></query>
</k-data>
<div k-for="item in user_paged.list">
	<span k-content="item.name"></span>
</div>
```

Return type
```ts
type MysqlPagedResult = QueryPagedResult<DatabaseRecord>;
```

## sqlserver

`sqlserver` uses the same parameter structure as `sqlite` and `mysql`, and `resource` corresponds to the table name.

### item

Example
```html
<k-data>
	<let as="record_id" source="queryString" from="id"></let>
	<query export as="invoice_detail" source="sqlserver" resource="invoices" action="item"
		id="record_id" select="id,code,status"></query>
</k-data>
<div>
	<span k-content="invoice_detail.code"></span>
</div>
```

Return type
```ts
type SqlServerItemResult = DatabaseRecord;
```

### list

Example
```html
<k-data>
	<query export as="invoices" source="sqlserver" resource="invoices" action="list"
		select="id,code,status" where="{status:{$eq:'open'}}" order-desc="id"></query>
</k-data>
<div k-for="item in invoices">
	<span k-content="item.code"></span>
</div>
```

Return type
```ts
type SqlServerListResult = DatabaseRecord[];
```

### paged

Example
```html
<k-data>
	<query export as="invoice_paged" source="sqlserver" resource="invoices" action="paged"
		select="id,code,status" where="{status:{$eq:'open'}}"
		page-number="1" page-size="10" order-desc="id"></query>
</k-data>
<div k-for="item in invoice_paged.list">
	<span k-content="item.code"></span>
</div>
```

Return type
```ts
type SqlServerPagedResult = QueryPagedResult<DatabaseRecord>;
```

## indexeddb

`indexeddb` uses the same parameter structure as database queries, and `resource` corresponds to the object-store name.

### item

Example
```html
<k-data>
	<let as="record_id" source="queryString" from="id"></let>
	<query export as="draft_detail" source="indexeddb" resource="drafts" action="item"
		id="record_id" select="id,title,status"></query>
</k-data>
<div>
	<span k-content="draft_detail.title"></span>
</div>
```

Return type
```ts
type IndexedDbItemResult = DatabaseRecord;
```

### list

Example
```html
<k-data>
	<query export as="drafts" source="indexeddb" resource="drafts" action="list"
		select="id,title,status" where="{status:{$eq:'published'}}" order-desc="id"></query>
</k-data>
<div k-for="item in drafts">
	<span k-content="item.title"></span>
</div>
```

Return type
```ts
type IndexedDbListResult = DatabaseRecord[];
```

### paged

Example
```html
<k-data>
	<query export as="draft_paged" source="indexeddb" resource="drafts" action="paged"
		select="id,title,status" where="{status:{$eq:'published'}}"
		page-number="1" page-size="10" order-desc="id"></query>
</k-data>
<div k-for="item in draft_paged.list">
	<span k-content="item.title"></span>
</div>
```

Return type
```ts
type IndexedDbPagedResult = QueryPagedResult<DatabaseRecord>;
```

## culture

The current implementation uses `culture` as the source for multilingual queries.

### item

Example
```html
<k-data>
	<let as="lang" source="queryString" from="lang"></let>
	<query export as="current_culture_url" source="culture" resource="/products/detail" action="item"
		id="lang"></query>
</k-data>
<a href="{current_culture_url.url}" k-content="current_culture_url.name"></a>
```

Return type
```ts
type CultureItemResult = CultureLink | null;
```

Notes

- `resource` is the relative address to query; when omitted, the current request address is used.
- `id` is the culture code; when omitted, the current context culture is used.

### list

Example
```html
<k-data>
	<query export as="culture_urls" source="culture" resource="/products/detail" action="list"></query>
</k-data>
<div k-for="item in culture_urls">
	<a href="{item.url}" k-content="item.name"></a>
</div>
```

Return type
```ts
type CultureListResult = CultureLink[];
```
