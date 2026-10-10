# 设置

> 菜单：**电商 → 设置**  
> 深链：`/_Admin/commerce/settings?SiteId={站点GUID}`

站点级 **电商全局配置**（Commerce Store `settings`）：计量单位、结账可用支付方式、商品/分类/订单的字段模型，以及规格与数字商品的显示规则。保存后影响 [商品类型](./product-types.md)、[商品管理](./product-management.md)、[商品分类](./product-categories.md)、[订单](./orders.md) 等模块；脚本侧通过 `k.commerce.settings` 读取，见 [k.commerce 设置](../../api/commerce/settings.md)。

::: tip 与「货币」菜单
前台默认 **货币代码 / 符号** 多在 **电商 → 货币** 单独维护；本页首屏 **基础** 区当前为 **重量单位** 与 **支付方式**（若后续界面开放货币下拉，以实际后台为准）。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **电商 → 设置**（及父级 **电商**）。
2. 左侧 **电商 → 设置**。

## 页面结构

单页表单，自上而下三块白底卡片 + 底部固定 **保存**（`KBottomBar`，无 **取消**）。支持快捷键保存（与站点其它编辑页一致）。

<DocImage src="/cms/commerce/commerce-settings-overview.png" alt="电商设置：基础、自定义字段、显示开关三块区域" width="1120" />

| 区块 | 内容 |
|------|------|
| **基础** | 重量单位、启用的支付方式 |
| **自定义字段** | 商品字段、分类字段、订单扩展字段 |
| **显示开关** | 隐藏属性 / 隐藏规格 / 数字商品交付项必填（条件显示） |

## 基础

| 项 | 说明 |
|----|------|
| **重量单位** | `kg` 或 `g`；用于商品变体重量及 [配送](./shippings.md) 等运费相关计算 |
| **启用的支付方式** | 多选下拉；选项来自站点已安装的支付模块（须先在 [服务集成](../navigation.md) `/_Admin/system/config` 配置支付能力，见 [k.payment](../../api/payment/index.md)）。保存为 `payments` 数组，结账仅展示已启用项。Tooltip：为 `k.commerce.settings.payments` 配置 |

## 自定义字段

三个 **SortableList** 列表，结构类似 [内容 → 数据类型](../content/content-types.md) 的字段表，但数据写入电商 `settings` 而非内容类型。

<DocImage src="/cms/commerce/commerce-settings-custom-fields.png" alt="电商设置：商品字段、分类字段、订单扩展字段列表" width="1120" />

### 商品字段

控制 [商品详情](./product-management-detail.md) 表单与 [商品管理](./product-management.md) 列表 **摘要列**。

| 操作 | 说明 |
|------|------|
| **添加** | 打开 **自定义字段** 弹窗，配置新字段 |
| **编辑** | 修改类型、显示名、验证、是否摘要列等 |
| **删除** | 非 **系统字段** 可删；系统字段（如 Title、Images）不可删除 |
| **排序** | 拖拽调整字段顺序 |

Tooltip：**摘要字段将显示在列表页面上**。

### 分类字段

控制 [商品分类](./product-categories.md) 编辑页与列表动态列，规则同商品字段。

### 订单扩展字段

为 [订单](./orders.md) 增加扩展数据定义；字段类型偏订单场景（如是否可筛选 `filterable`、是否可导出 `exportable`、列宽等），与商品自定义字段弹窗略有不同。

### 自定义字段弹窗（商品 / 分类）

点击商品或分类字段列表的 **添加** 或行内 **编辑** 打开 `CustomFieldDialog`。

<DocImage src="/cms/commerce/commerce-settings-custom-field-dialog.png" alt="自定义字段弹窗：基础、高级、验证等 Tab" width="960" />

常见配置（与内容字段编辑器同类控件）：

| Tab / 项 | 说明 |
|----------|------|
| **基础** | 名称、显示名、控件类型（文本、富文本、图片、选择等）、是否多值、是否 **摘要字段**、是否多语言等 |
| **高级** | 动态选项、关联内容夹等（视控件类型显示） |
| **验证** | 必填、长度等规则 |
| 系统字段 **Images / Image** | 允许编辑图片 **meta** 模板（商品主图/分类图元数据） |

保存弹窗后字段进入对应列表；须再点页面底部 **保存** 才写入站点。

## 显示开关

<DocImage src="/cms/commerce/commerce-settings-display.png" alt="电商设置：隐藏属性、隐藏规格、数字商品交付项必填" width="1120" />

| 开关 | 说明 |
|------|------|
| **隐藏属性** | 开启后，[商品类型](./product-types.md) 创建弹窗与列表 **属性** 列隐藏；[商品详情](./product-management-detail.md) 基本信息区不显示 **属性** 键值对 |
| **隐藏规格** | 开启后，商品按单规格处理；类型与详情无 **规格选项 / 变体** 区块，列表不显示规格数 |
| **产品数字内容不能为空** | 仅当存在 **至少一种** 勾选了 **数字商品** 的 [商品类型](./product-types.md) 时显示；开启后，保存数字商品时每个变体须配置 [数字产品](./product-management-detail.md#数字商品) 交付项 |

## 相关

- [电商概述](./index.md) — 建议实施顺序  
- [商品类型](./product-types.md) · [商品管理](./product-management.md) · [商品分类](./product-categories.md)  
- [货币](./currencies.md) · [配送](./shippings.md) · [优惠折扣](./discounts.md)  
- [k.commerce 设置](../../api/commerce/settings.md)
