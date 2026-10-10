# 商品管理（列表）

> 菜单：**电商 → 商品管理**  
> 深链：`/_Admin/commerce/product-management?SiteId={站点GUID}`

维护站点可售商品的 **列表、筛选与入口**；单条商品的表单与规格在 [商品详情（新建与编辑）](./product-management-detail.md) 维护。

::: tip 前置
须已有 [商品类型](./product-types.md)；分类可选，见 [商品分类](./product-categories.md)。全局开关与自定义字段见 [设置](./settings.md)。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **电商 → 商品管理**（及父级 **电商**）。
2. 左侧 **电商 → 商品管理**。

## 列表页

路径：`/_Admin/commerce/product-management?SiteId=...`

| 区域 | 说明 |
|------|------|
| 统计 | 缺货数、商品总数、库存合计等 |
| 筛选 | 关键词、上架状态、库存状态等；点击 **搜索** 刷新 |
| 表格列 | 由 [商品类型](./product-types.md) 与 [设置](./settings.md) 中的 **商品自定义字段** 决定；常见有标题、主图、规格数、库存、销量、标签、分类 |
| **创建商品** | 下拉：空白商品，或指定 **商品类型** 后进入 [新建页](./product-management-detail.md#如何进入) |
| **编辑** | 行内铅笔进入 [编辑页](./product-management-detail.md#如何进入) |
| **复制** | 勾选 **一行** 后，表格上方出现 **复制**，确认后生成副本 |
| **批量删除** | 勾选多行后，表格工具栏 **删除**；确认后移除所选商品 |

<DocImage src="/cms/commerce/product-management-list.png" alt="商品管理列表：统计、筛选、商品表格与创建商品下拉" width="1120" />

::: tip 规格（Variants）
若 [设置](./settings.md) 未开启「隐藏规格」，列表会显示规格数量；规格明细在 [商品详情](./product-management-detail.md) 页维护。
:::

## 数字商品

数字商品（电子书、软件授权、可下载资源等）须先具备 **数字商品** 类型的 [商品类型](./product-types.md#数字商品)，再在列表中创建或编辑具体商品。

| 步骤 | 在哪里做 |
|------|----------|
| 1. 定义类型 | [商品类型](./product-types.md) — 勾选 **数字商品**，可选下载次数/天数限制 |
| 2. 创建商品 | 本页 **创建商品** → 选择该类型（或编辑已有数字商品） |
| 3. 配置交付内容 | [商品详情](./product-management-detail.md#数字商品) — 在变体表格 **数字产品** 列，为 **每个 SKU** 添加文件 / 链接 / 文本 |

列表摘要区或表格中，数字商品常带 **数字产品** 小标签（`isDigital`），便于与实体商品区分。具体交付项不在列表维护，点 **编辑** 进入详情页操作。

若 [设置](./settings.md) 开启 **数字商品交付项必填**，未给任一变体配置交付内容的商品 **无法保存**。

## 相关

- [商品详情（新建与编辑）](./product-management-detail.md) — [数字商品配置](./product-management-detail.md#数字商品)  
- [商品类型](./product-types.md) — [数字商品类型](./product-types.md#数字商品)  
- [商品分类](./product-categories.md)  
- [电商概述](./index.md)
