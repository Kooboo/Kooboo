# Commerce 内置函数指南

> 指导 AI 通过 **Kooboo 内置函数**（`BuiltIn/Commerce`）创建、查询、更新和删除商品与商品分类。  
> 在运行时用 KScript 读取电商对象（如 `k.product`、`k.category`）请配合同目录 [index.md](index.md)。

## 何时阅读本文

- 需要通过 function call 直接维护商城商品或分类数据（而非只读展示）。
- 需要调用 `create_product`、`update_product_category`、`get_product_detail` 等函数，但不确定参数契约与调用顺序。
- 需要区分哪些操作会写数据（需审批）与哪些仅查询（无需审批）。

## 与 `index.md`（KScript Commerce）分工

| 场景 | 使用 |
| --- | --- |
| 通过 AI 工具函数做商品/分类的增删改查 | 本文 + 对应内置函数 |
| 在页面/API/SSR KScript 中读取并渲染电商数据 | [index.md](index.md) |

## 推荐调用顺序

1. `list_product_category` / `list_product` —— 先了解现有数据，避免重名或误删。
2. 创建场景：`create_product_category`（可选）-> `create_product`。
3. 更新场景：先 `get_product_detail` / `get_product_category_detail` 确认目标对象，再执行 `update_*`。
4. 删除场景：删除前先用详情函数做一次确认，再调用 `delete_product` 或 `delete_product_category`。
5. 删除后可再次 `list_*` 复核结果。

商城前台动态化时，Product / Category / Variant 默认属于 Commerce。不要为了“后台可维护”而创建同名 ContentType/ContentFolder；完整边界见 `../../Scenario/site-building/commerce-dynamicization.md`。

## 审批（Approval）

以下函数 `ApprovalRequired == true`，执行前通常需用户确认：

- `create_product`
- `update_product`
- `delete_product`
- `create_product_category`
- `update_product_category`
- `delete_product_category`

以下函数 `ApprovalRequired == false`：

- `list_product`
- `get_product_detail`
- `list_product_category`
- `get_product_category_detail`

## 商品（Product）函数

## `create_product`

创建商品，并自动创建一个默认变体（variant）。

| 参数 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| Title | string | 是 | 商品标题 |
| SeoName | string | 是 | SEO 友好名称（URL 标识） |
| Description | string | 否 | 商品描述 |
| FeaturedImage | string | 否 | 主图 URL；AI storefront 创建时必传 |
| Price | double | 是 | 默认变体价格 |
| Inventory | int | 是 | 默认变体库存 |
| CategoryIds | string[] | 是 | 已存在的手工 Commerce 分类 ID，可来自查询结果或 `create_product_category`；明确需要未分类商品时传 `[]` |
| Tags | string[] | 否 | 标签数组 |

返回：结构化保存回执，包含 `message`、商品/默认变体标识、`categoryIds` 和关键字段。写入后仍需用 `get_product_detail` 读回并确认 `categories`。

## `update_product`

按商品 Id 更新基础信息；未传字段保持不变。

| 参数 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| Id | string | 是 | 商品 Id |
| Title | string | 否 | 商品标题 |
| SeoName | string | 否 | SEO 名称 |
| Description | string | 否 | 商品描述 |
| FeaturedImage | string | 否 | 主图 URL |
| Tags | string[] | 否 | 标签数组 |
| Active | bool | 否 | 是否启用 |
| CategoryIds | string[] | 否 | 手工 Commerce 分类 ID 的替换集合；不传表示保持现有关联，传 `[]` 表示移除全部关联 |

返回：`"Update success"`（字符串）。

## `delete_product`

按商品 Id 删除商品；实现会同时移除该商品的关联变体与分类关联。

| 参数 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| Id | string | 是 | 商品 Id |

返回：`"Success deleted"`（字符串）。

## `list_product`

列出所有商品的基础信息（扁平列表）。

| 参数 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| （无） | - | - | 无输入参数 |

返回列表每项包含：`Id`、`SeoName`、`Title`。

## `get_product_detail`

按 `Id` 或 `SeoName` 获取商品详情（含变体和分类等扩展信息）。

| 参数 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| SeoNameOrId | string | 是 | 商品 Id 或 SeoName |

返回：商品详情对象（找不到会抛错）。

## 分类（Category）函数

## `create_product_category`

创建商品分类。

| 参数 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| Title | string | 是 | 分类标题 |
| SeoName | string | 是 | SEO 友好名称 |
| Description | string | 否 | 分类描述 |
| Image | string | 否 | 分类图片 URL；AI storefront 创建时必传 |
| Tags | string[] | 否 | 标签数组 |
| ParentId | string | 否 | 父分类 Id；为空表示根分类 |

返回：结构化保存回执，包含 `message`、分类标识和关键字段。写入后仍需用 `get_product_category_detail` 读回确认。

## `update_product_category`

按分类 Id 更新分类信息；未传字段保持不变。

| 参数 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| Id | string | 是 | 分类 Id |
| Title | string | 否 | 分类标题 |
| SeoName | string | 否 | SEO 名称 |
| Description | string | 否 | 分类描述 |
| Image | string | 否 | 分类图片 URL |
| Tags | string[] | 否 | 标签数组 |
| ParentId | string | 否 | 父分类 Id；传空字符串可移到根级 |
| Active | bool | 否 | 是否启用 |

返回：`"Update success"`（字符串）。

## `delete_product_category`

按分类 Id 删除分类；实现会移除该分类的商品-分类关联。

| 参数 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| Id | string | 是 | 分类 Id |

返回：`"Success deleted"`（字符串）。

## `list_product_category`

列出所有商品分类的基础信息（扁平列表）。

| 参数 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| （无） | - | - | 无输入参数 |

返回列表每项包含：`Id`、`SeoName`、`Title`。

## `get_product_category_detail`

按 `Id` 或 `SeoName` 获取分类详情，返回当前分类、祖先路径和直接子分类。

| 参数 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| SeoNameOrId | string | 是 | 分类 Id 或 SeoName |

返回：`CategoryDetail` 对象（找不到会抛错）。

## 常见错误

1. **用标题当唯一标识更新/删除**：`update_*` 与 `delete_*` 基本都要求 `Id`，建议先 `list_*` 或 `get_*_detail` 获取准确 Id。
2. **误以为查询函数可分页/过滤**：当前 `list_product` 与 `list_product_category` 为全量基础列表，不支持传筛选参数。
3. **层级分类更新遗漏 ParentId 语义**：`update_product_category.ParentId` 传空字符串是“移到根级”，与“不传该字段”含义不同。
4. **误解 `update_product.CategoryIds`**：不传表示保持现有关联，传 `[]` 表示移除全部关联，传非空数组则替换为指定的手工 Commerce 分类 ID。
5. **删前未确认引用关系**：删除函数会级联清理关联（商品变体/商品分类关系），建议先查询详情确认影响面。
6. **把 `product.price` 当作首个变体别名**：顶层价格是变体最低价；选定变体后的精确购买价格仍用该 variant。
7. **storefront 数据缺主图或 seoName**：AI 建站写入必须同时提供稳定 URL 标识与可加载主图，不能只传标题和价格。
8. **只创建分类和商品但没建立关系**：把对应的手工分类 id 放入 `create_product.categoryIds`，创建后读回 `product.categories`，不能只比较两份列表是否存在。

## 实现溯源

逻辑以源码为准：`Kooboo.Sites/AI/Functions/BuiltIn/Commerce/Product/*.cs`、`Kooboo.Sites/AI/Functions/BuiltIn/Commerce/Category/*.cs`。
