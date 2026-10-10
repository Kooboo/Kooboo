# 操作 Hook

> 菜单：**站点设置 → 操作 Hook**（高级菜单）
> 列表：`/_Admin/system/backend-events?SiteId={站点GUID}`
> 编辑：`/_Admin/system/backend-events/edit?SiteId={站点GUID}&name={事件名}&display={显示名}`

**操作 Hook**在后台资源 **创建、更新、删除** 等操作前后触发规则，用于审计、同步、通知或在资源保存前做业务校验。对应服务端 `BackendRule` + `BackendEvent`；须同时开启站点级 **`enableBackendEvents`** 开关后规则才会执行。

::: tip 权限
| 操作 | 权限 |
|------|------|
| 查看 Hook 列表与编辑页 | `backendEvents` |
| 切换右上角 **Hook 开启 / 关闭** | `site·edit`（保存站点字段 `enableBackendEvents`） |
| **新建 Hook**、编辑规则并 **保存** | `backendEvents·edit` |
| 多选 **删除** 某事件下全部规则 | `backendEvents·edit` |

当前界面中 **新建 Hook** 与编辑页底部 **保存** 按钮仍检查 `frontEvents·edit`，实施角色权限时请同时核对该项。
:::

::: info 总开关
除本页右上角 **Hook 开启** 外，[基础设置 · 高级](./basic/advanced.md#约束与其它开关) 中 **操作 Hook**（`enableBackendEvents`）须为开；两处修改同一站点字段，任一处关闭则所有操作 Hook 不执行。
:::

::: warning 与请求 Hook 区分
| | [请求 Hook](./request-hooks.md) | **操作 Hook（本篇）** |
|--|-------------------------------|----------------------|
| 触发时机 | 前台 **HTTP 请求**（Route / Page / View） | 后台 **资源操作**（创建 / 更新 / 删除等） |
| 路径 | `/system/front-events` | `/system/backend-events` |
| 规则资源 | `BusinessRule` | `BackendRule` |
| 总开关 | `enableFrontEvents` | `enableBackendEvents` |
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **站点设置 → 操作 Hook**。
2. 左侧 **站点设置 → 操作 Hook**。

<DocImage src="/cms/settings/settings-action-hooks-overview.png" alt="操作 Hook 列表" width="1120" />

## 工具栏

| 控件 | 说明 |
|------|------|
| **新建 Hook** | 打开 [Hook 类型选择](#新建-hook)（已配置过的事件类型不再出现） |
| **Hook 开启 / 关闭** | `enableBackendEvents`；切换后 **立即保存站点**（`saveSite`） |
| 状态文案 | 开启时显示「Hook开启」（蓝色），关闭为灰色「Hook关闭」 |

<DocImage src="/cms/settings/settings-action-hooks-toolbar.png" alt="操作 Hook 工具栏" width="1120" />

## Hook 列表

| 列 | 说明 |
|----|------|
| **类型** | 事件 `name` 的本地化显示（如 PageCreated → 页面已创建，文案以登录语言为准） |
| **规则数量** | 该事件下已保存的规则条数 |
| **编辑** | 进入 [规则编辑页](#规则编辑页) |

多选 **删除** 会移除所选事件类型的 **全部** 规则（`BackendRule/DeleteEvents`），不是单条规则删除。

<DocImage src="/cms/settings/settings-action-hooks-list.png" alt="操作 Hook 列表" width="1120" />

## 新建 Hook

点击 **新建 Hook** 弹出 **Hook列表**，按资源类别分组展示可选事件；点击某一类型即进入编辑页（尚未保存规则前列表可能仍无该行，保存后出现）。

<DocImage src="/cms/settings/settings-action-hooks-event-picker.png" alt="Hook 类型选择" width="1120" />

### 可选事件类型

Hook 列表按资源分组。显示名由服务端 `Hardcoded` 本地化；以 `BackendRule/EventList` 返回为准。

#### 图片事件

| 时机 | 事件名（`name`） |
|------|------------------|
| 上传前 / 后 | `ImageUploading` / `ImageUpload` |
| 更新前 / 后 | `ImageUpdating` / `ImageUpdated` |
| 删除前 / 后 | `ImageDeleting` / `ImageDelete` |

#### 通用资源事件

下列资源都遵循 **资源前缀 + 时机后缀** 的命名规则，例如 `PageCreating`、`ContentUpdated`。

| 后台分组 | `name` 前缀 |
|----------|-------------|
| 页面 | `Page` |
| 组件 | `View` |
| 布局 | `Layout` |
| 代码 | `Code` |
| 菜单 | `Menu` |
| HTML 代码块 | `HtmlBlock` |
| 内容 | `Content` |
| 标签 | `Label` |
| 脚本 | `Script` |
| 样式 | `Style` |

| 操作时机 | `name` 后缀 | 组合示例 |
|----------|-------------|----------|
| 创建前 / 后 | `Creating` / `Created` | `PageCreating` / `PageCreated` |
| 更新前 / 后 | `Updating` / `Updated` | `ContentUpdating` / `ContentUpdated` |
| 删除前 / 后 | `Deleting` / `Deleted` | `ScriptDeleting` / `ScriptDeleted` |

#### 电商事件

| 后台分组 | 事件名（`name`） | 典型时机 |
|----------|------------------|----------|
| 商品 | `ProductCreating` / `ProductCreated` | 商品创建前 / 后 |
| 商品 | `ProductUpdating` / `ProductUpdated` | 商品更新前 / 后 |
| 商品 | `ProductDeleting` / `ProductDeleted` | 商品删除前 / 后 |
| 商品 | `ProductCopied` | 复制商品后 |
| 商品分类 | `ProductCategoryCreating` / `ProductCategoryCreated` | 分类创建前 / 后 |
| 商品分类 | `ProductCategoryUpdating` / `ProductCategoryUpdated` | 分类更新前 / 后 |
| 商品分类 | `ProductCategoryDeleting` / `ProductCategoryDeleted` | 分类删除前 / 后 |
| 订单 | `OrderConfirmed` | 订单已确认 |
| 订单 | `OrderDelivered` | 订单已发货 |
| 订单 | `OrderCancelled` | 订单已取消 |

## 规则编辑页

面包屑：**操作 Hook → {事件显示名}**。无侧栏菜单项，深链带 `name`（必填）与 `display`（面包屑用）。

<DocImage src="/cms/settings/settings-action-hooks-edit.png" alt="规则编辑页" width="1120" />

### 新建规则

工具栏 **新建规则** 下拉：

| 类型 | 值 | 结构 |
|------|-----|------|
| **IF-ELSE-THEN** | `IF-ELSE-THEN` | 条件 **IF** + 分支 **THEN** / **ELSE**（可嵌套子规则） |
| **DO** | `DO` | 顺序执行一段或多段 **Code** |

进入编辑页时若尚无规则，默认添加一条 **IF-ELSE-THEN** 规则。

### 规则卡片

- 拖动手柄在卡片 **标题栏**，可调整同一事件下规则 **执行顺序**。
- 标题栏 **删除** 移除整条规则。

#### IF-ELSE-THEN

| 区域 | 说明 |
|------|------|
| **IF** | 点击标签打开 [条件编辑](#编辑条件)；多条条件以 `&&` 连接展示 |
| **THEN** / **ELSE** | 嵌套子规则列表，可继续添加 IF-ELSE-THEN 或 DO |

#### DO

列表展示各 Code 片段摘要；**+** 添加 Code，铅笔编辑。Code 在 Monaco 中编写 KScript（`language: typescript`，KScript 模式）；具体脚本对象与 API 用法见文末相关链接。

<DocImage src="/cms/settings/settings-action-hooks-code-dialog.png" alt="Code 编辑弹窗" width="1120" />

### 编辑条件

<DocImage src="/cms/settings/settings-action-hooks-condition-dialog.png" alt="编辑条件" width="1120" />

| 列 | 说明 |
|----|------|
| **左值** | 当前事件可用字段（`BackendRule/ConditionOption`），例如资源 `Name`、`Body`、`Url`、内容 `FolderName`、商品 `Title` 等 |
| **运算符** | 随左值数据类型变化（如 `=`、`!=`、`Contains`、`>` 等） |
| **右值** | 比较常量 |

可 **+** 增加条件行；确认写回规则 `if` 数组。条件求值由 `ConditionManager.Evaluate` 在运行时执行。

### 保存与离开

底部 **KBottomBar**：**返回** 回列表；**保存** 调用 `BackendRule/post`（`eventName` + `rules`）。离开页面前若有未保存修改会提示（`useSaveTip`）。

## 执行顺序（概念）

1. 站点 `enableBackendEvents` 为真。
2. 后台资源操作触发对应 BackendEvent（如 `PageCreating`、`ContentUpdated`）。
3. 按列表顺序执行该事件下各规则；IF-ELSE-THEN 先判条件再走 THEN/ELSE；DO 顺序执行 Code。

具体执行由站点后台操作 Hook 引擎负责，按本页配置顺序运行。

## 典型工作流

1. 确认 [高级 Tab](./basic/advanced.md) 与本页 **Hook 开启**。
2. **新建 Hook** → 选择目标资源与时机，例如 **ContentUpdated**。
3. 添加 **IF-ELSE-THEN** 过滤资源字段，或添加 **DO** 执行后台动作。
4. **保存** 后回到对应后台资源做一次创建、更新或删除验证。
5. 协作导出时 Hook 对应站点包中的 **BackendRule** 资源。

## CMS 与 API 分工

| 场景 | 看 CMS | 看 API / 模板引擎 |
|------|--------|-------------------|
| 配置哪些后台操作会触发规则 | 本篇 Hook 类型选择与规则保存 | — |
| 编写规则里的脚本动作 | 只说明从 Code 弹窗进入 | [开发 → Code](../development/code.md)、[k.site Code API](../../api/site/code.md) |
| 内容保存后联动内容数据 | 本篇选择 Content 事件 | [k.content](../../api/content/index.md) |
| 商品、订单事件后联动电商数据 | 本篇选择 Product / Order 事件 | [k.commerce](../../api/commerce/commerce.md) |

## 相关

| 文档 | 说明 |
|------|------|
| [站点设置概述](./index.md) | 菜单与子路由 |
| [基础设置 · 高级](./basic/advanced.md) | `enableBackendEvents` |
| [请求 Hook](./request-hooks.md) | 前台请求生命周期事件 |
| [开发 → Code](../development/code.md) | KScript 代码资源入口 |
| [内容 API](../../api/content/index.md) | 内容数据读写能力 |
| [电商 API](../../api/commerce/commerce.md) | 商品、分类、订单相关 API |
