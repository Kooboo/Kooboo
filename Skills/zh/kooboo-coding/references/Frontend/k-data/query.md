# query 语法使用说明


## 公共 TypeScript 类型

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

分页结果计数直接读取 `query_result.total`。如果需要别名，使用 `<let as="total" from="query_result.total"></let>`；不要把整个 `query_result` 赋给 `total`，也不要在同一个 `<let>` 上重复声明 `from`，否则模板可能渲染出 CLR/Dictionary 类型文本。

查询动作按结果语义选择：

- 已知唯一标识、需要单对象详情时使用 `action="item"`，不要先执行 `list` 再读取 `[0]`。
- 只有业务语义确实是“有序结果中的第一项”时，才使用 `list` + `[0]`。
- `select` 是可选的字段投影；需要完整对象时省略。内容项通过名称 / ID 执行 `item` 查询时返回完整对象，通常不声明 `select`。
- `query` 当前通用定位属性是 `id` 和 `where`：内容项名称 / ID 使用 `id="变量名"`，自定义字段使用 `where="{field:{$eq:变量名}}"`。


## content

`content` 的 `resource` 对应contentFolder名称。

### item

例子：详情 Page 的 URL 为 `/blog/{slug}`，且内容集合有稳定的 `slug` 字段。
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

返回数据类型
```ts
type ContentItemResult = ContentRecord;
```

说明：

- `id` 的值可以引用内容项的名称 / ID 变量。
- Kooboo 页面路由 `/blog/{slug}` 中的 `slug` 已进入渲染数据上下文；当它就是内容项名称 / ID 时，使用 `<let as="slug" from="slug"></let>`，再传 `id="slug"`；当它是自定义字段时，在 `where="{slug:{$eq:slug}}"` 中使用同一变量。
- 当 `slug` 是普通内容字段而不是名称 / ID 时，仍使用 `action="item"`，并传 `where="{slug:{$eq:slug}}"`；`item` 会返回第一条匹配记录，无需改为 `list` 后再取 `[0]`。
- 列表和分页结果的链接应使用同一个路径参数，例如 `/blog/{item.slug}`；路径可表达时，不要把规范文章身份移到 `/blog?slug=...`。
- 详情需要完整字段时省略 `select`；只有明确需要部分字段投影时才声明它。

### list

例子
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

返回数据类型
```ts
type ContentListResult = ContentRecord[];
```

### paged

例子
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

返回数据类型
```ts
type ContentPagedResult = QueryPagedResult<ContentRecord>;
```

## commerce

`commerce` 的 `resource` 不是任意表名，而是固定值：`product` / `category` / `cart` / `order`。

### product / item

例子
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

返回数据类型
```ts
type CommerceProductItemResult = CommerceProduct;
```

说明：

- `id` 属性必须引用真实存在的变量，不能写文档占位名或省略。
- 该值既可为 Commerce product id，也可为 seoName。新建 PDP 时优先使用 Page URL `/product/{seoName}`，以 `<let as="seoName" from="seoName"></let>` 读取；列表详情链接使用 `/product/{item.seoName}`，并传给 `<query id="seoName">`。
- 只有需要兼容既有公开路由或业务明确要求时才使用 query-string 身份；新 PDP 不默认使用 `/product?seoName=...`。
- 详情链接和详情 View 完成后必须打开一个真实商品 URL，确认页面没有渲染 `Query commerce product item must provide id` 等错误文本。

### product / list

例子
```html
<k-data>
	<query export as="products" source="commerce" resource="product" action="list"
		where="{category:'category_id_or_seoname',title:{$contains:'shoe'}}"></query>
</k-data>
<div k-for="item in products">
	<h2 k-content="item.title"></h2>
</div>
```

返回数据类型
```ts
type CommerceProductListResult = CommerceProduct[];
```

说明

- `where` 当前支持按 product 顶层字段过滤，如 `id`、`title`、`featuredImage`、`images`、`active`、`seoName`、`tags`
- `where.category` 支持按分类 id 或 seoName 过滤
- `where.category` 默认始终包含子分类，无需额外指定 `include-sub-category`
- 商品列表/起售价直接使用 `item.price`；它是当前商品全部变体的最低价，不保证等于 `item.variants[0].price`
- 用户选中具体变体后，精确价格、库存和加购参数改用该 variant

### product / paged

例子
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

返回数据类型
```ts
type CommerceProductPagedResult = QueryPagedResult<CommerceProduct>;
```

### category / item

例子
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

返回数据类型
```ts
type CommerceCategoryItemResult = CommerceCategory;
```

### category / list

例子
```html
<k-data>
	<query export as="categories" source="commerce" resource="category" action="list"
		where="{title:{$contains:'summer'}}"></query>
</k-data>
<div k-for="item in categories">
	<span k-content="item.title"></span>
</div>
```

返回数据类型
```ts
type CommerceCategoryListResult = CommerceCategory[];
```

说明

- `where` 支持按 `id`、`title`、`seoName`、`description`、`image`、`active`、`tags`、`parentId` 过滤

### Products 分类选择器（All + active）

- 分类查询必须带 `where="{active:{$eq:true}}"`。
- 商品查询必须带 `active:{$eq:true}`。
- `All` 对应没有 category 条件，不要把 `all` 作为分类 id/seoName。
- 用同一个 queryString category 值驱动查询和 active 样式；分类变化时重置页码。
- 完整分支示例见 `references/Scenario/site-building/commerce-dynamicization.md`。

### category / paged

例子
```html
<k-data>
	<query export as="category_paged" source="commerce" resource="category" action="paged"
		where="{active:{$eq:true}}" page-number="1" page-size="20"></query>
</k-data>
<div k-for="item in category_paged.list">
	<span k-content="item.title"></span>
</div>
```

返回数据类型
```ts
type CommerceCategoryPagedResult = QueryPagedResult<CommerceCategory>;
```

### cart / item

例子
```html
<k-data>
	<query export as="cart_detail" source="commerce" resource="cart" action="item"></query>
</k-data>
<div>
	<span k-content="cart_detail.id"></span>
</div>
```

返回数据类型
```ts
type CommerceCartItemResult = CommerceCart;
```

说明

- `cart` 只支持 `item`
- 未传 `id` 时，会优先读取当前上下文中的购物车 id

### order / item

例子
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

返回数据类型
```ts
type CommerceOrderItemResult = CommerceOrder;
```

### order / list

例子
```html
<k-data>
	<query export as="orders" source="commerce" resource="order" action="list"></query>
</k-data>
<div k-for="item in orders">
	<span k-content="item.id"></span>
</div>
```

返回数据类型
```ts
type CommerceOrderListResult = CommerceOrder[];
```

说明

- 不支持 `where` 参数
- 不支持排序参数 `order` 和 `order-desc`，订单始终按创建时间倒序排列

### order / paged

例子
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

返回数据类型
```ts
type CommerceOrderPagedResult = QueryPagedResult<CommerceOrder>;
```

说明

- 不支持 `where` 参数
- 不支持排序参数 `order` 和 `order-desc`，订单始终按创建时间倒序排列

## sqlite

`sqlite` 的 `resource` 对应表名。

### item

例子
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

返回数据类型
```ts
type SqliteItemResult = DatabaseRecord;
```

### list

例子
```html
<k-data>
	<query export as="payment_records" source="sqlite" resource="payment_records" action="list"
		select="id,amount,status" where="{status:{$eq:'paid'}}" order-desc="id"></query>
</k-data>
<div k-for="item in payment_records">
	<span k-content="item.amount"></span>
</div>
```

返回数据类型
```ts
type SqliteListResult = DatabaseRecord[];
```

### paged

例子
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

返回数据类型
```ts
type SqlitePagedResult = QueryPagedResult<DatabaseRecord>;
```

## mysql

`mysql` 的参数结构与 `sqlite` 相同，`resource` 对应表名。

### item

例子
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

返回数据类型
```ts
type MysqlItemResult = DatabaseRecord;
```

### list

例子
```html
<k-data>
	<query export as="users" source="mysql" resource="users" action="list"
		select="id,name,email" where="{name:{$contains:'tom'}}" order="name"></query>
</k-data>
<div k-for="item in users">
	<span k-content="item.name"></span>
</div>
```

返回数据类型
```ts
type MysqlListResult = DatabaseRecord[];
```

### paged

例子
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

返回数据类型
```ts
type MysqlPagedResult = QueryPagedResult<DatabaseRecord>;
```

## sqlserver

`sqlserver` 的参数结构与 `sqlite`、`mysql` 相同，`resource` 对应表名。

### item

例子
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

返回数据类型
```ts
type SqlServerItemResult = DatabaseRecord;
```

### list

例子
```html
<k-data>
	<query export as="invoices" source="sqlserver" resource="invoices" action="list"
		select="id,code,status" where="{status:{$eq:'open'}}" order-desc="id"></query>
</k-data>
<div k-for="item in invoices">
	<span k-content="item.code"></span>
</div>
```

返回数据类型
```ts
type SqlServerListResult = DatabaseRecord[];
```

### paged

例子
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

返回数据类型
```ts
type SqlServerPagedResult = QueryPagedResult<DatabaseRecord>;
```

## indexeddb

`indexeddb` 的参数结构与数据库查询一致，`resource` 对应对象表名。

### item

例子
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

返回数据类型
```ts
type IndexedDbItemResult = DatabaseRecord;
```

### list

例子
```html
<k-data>
	<query export as="drafts" source="indexeddb" resource="drafts" action="list"
		select="id,title,status" where="{status:{$eq:'published'}}" order-desc="id"></query>
</k-data>
<div k-for="item in drafts">
	<span k-content="item.title"></span>
</div>
```

返回数据类型
```ts
type IndexedDbListResult = DatabaseRecord[];
```

### paged

例子
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

返回数据类型
```ts
type IndexedDbPagedResult = QueryPagedResult<DatabaseRecord>;
```

## culture

多语言查询在当前实现中使用的 source 是 `culture`。

### item

例子
```html
<k-data>
	<let as="lang" source="queryString" from="lang"></let>
	<query export as="current_culture_url" source="culture" resource="/products/detail" action="item"
		id="lang"></query>
</k-data>
<a href="{current_culture_url.url}" k-content="current_culture_url.name"></a>
```

返回数据类型
```ts
type CultureItemResult = CultureLink | null;
```

说明

- `resource` 表示要查询的相对地址；不传时默认使用当前请求地址
- `id` 表示 culture code；不传时默认使用当前上下文 culture

### list

例子
```html
<k-data>
	<query export as="culture_urls" source="culture" resource="/products/detail" action="list"></query>
</k-data>
<div k-for="item in culture_urls">
	<a href="{item.url}" k-content="item.name"></a>
</div>
```

返回数据类型
```ts
type CultureListResult = CultureLink[];
```
