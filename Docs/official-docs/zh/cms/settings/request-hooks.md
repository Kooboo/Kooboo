# 请求 Hook

> 菜单：**站点设置 → 请求 Hook**（高级菜单）  
> 列表：`/_Admin/system/front-events?SiteId={站点GUID}`  
> 编辑：`/_Admin/system/front-events/edit?SiteId={站点GUID}&name={事件名}&display={显示名}`

**请求 Hook**在访客 **HTTP 请求** 生命周期的固定节点（路由 / 页面 / 视图解析前后）执行 KScript 规则，用于 URL 重写、条件跳转、动态替换页面或视图等。对应服务端 `BusinessRule` + `FrontEvent`；须同时开启站点级 **`enableFrontEvents`** 开关后规则才会执行。

::: tip 权限
| 操作 | 权限 |
|------|------|
| 查看 Hook 列表与编辑页 | `frontEvents` |
| **新建 Hook**、编辑规则并 **保存**、列表 **Hook 开关** | `frontEvents·edit` |
| 多选 **删除** 某事件下全部规则 | 列表删除（移除该事件类型所有规则） |
:::

::: info 总开关
除本页右上角 **Hook 开启** 外，[基础设置 · 高级](./basic/advanced.md#约束与其它开关) 中 **请求 Hook**（`enableFrontEvents`）须为开；两处修改同一站点字段，任一处关闭则所有请求 Hook 不执行。
:::

::: warning 与操作 Hook 区分
| | **请求 Hook（本篇）** | [操作 Hook](./action-hooks.md) |
|--|----------------------|--------------------------------|
| 触发时机 | 前台 **HTTP 请求**（Route / Page / View） | 后台 **保存、删除** 等资源操作 |
| 路径 | `/system/front-events` | `/system/backend-events` |
| 总开关 | `enableFrontEvents` | `enableBackendEvents` |
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **站点设置 → 请求 Hook**。  
2. 左侧 **站点设置 → 请求 Hook**。

<DocImage src="/cms/settings/settings-request-hooks-overview.png" alt="请求 Hook 列表" width="1120" />

## 工具栏

| 控件 | 说明 |
|------|------|
| **新建 Hook** | 打开 [Hook 类型选择](#新建-hook)（已配置过的事件类型不再出现） |
| **Hook 开启 / 关闭** | `enableFrontEvents`；切换后 **立即保存站点**（`saveSite`） |
| 状态文案 | 开启时显示「Hook开启」（蓝色），关闭为灰色「Hook关闭」 |

<DocImage src="/cms/settings/settings-request-hooks-toolbar.png" alt="请求 Hook 工具栏" width="1120" />

## Hook 列表

| 列 | 说明 |
|----|------|
| **类型** | 事件 `name` 的本地化显示（如 RouteFinding → 界面文案以登录语言为准） |
| **规则数量** | 该事件下已保存的规则条数 |
| **编辑** | 进入 [规则编辑页](#规则编辑页) |

多选 **删除** 会移除所选事件类型的 **全部** 规则（`BusinessRule/DeleteEvents`），不是单条规则删除。

<DocImage src="/cms/settings/settings-request-hooks-list.png" alt="请求 Hook 列表" width="1120" />

## 新建 Hook

点击 **新建 Hook** 弹出 **Hook列表**，按 **分类**（Route / View / Page）分组展示可选事件；点击某一类型即进入编辑页（尚未保存规则前列表可能仍无该行，保存后出现）。

<DocImage src="/cms/settings/settings-request-hooks-event-picker.png" alt="Hook 类型选择" width="1120" />

### 可选事件类型

| 分类 | 事件名（`name`） | 典型时机 |
|------|------------------|----------|
| **Route** | `RouteFinding` | 解析路由 **之前**（可改 `k.event.url` 等） |
| | `RouteFound` | 已匹配到路由 |
| | `RouteNotFound` | 未匹配到路由 |
| **View** | `ViewFinding` | 查找视图组件 **之前** |
| | `ViewFound` | 已找到视图 |
| | `ViewNotFound` | 未找到视图 |
| **Page** | `PageFinding` | 查找页面 **之前** |
| | `PageFound` | 已找到页面 |

显示名由服务端 `Hardcoded` 本地化；以 `BusinessRule/EventList` 返回为准。

## 规则编辑页

面包屑：**请求 Hook → {事件显示名}**。无侧栏菜单项，深链带 `name`（必填）与 `display`（面包屑用）。

<DocImage src="/cms/settings/settings-request-hooks-edit.png" alt="规则编辑页" width="1120" />

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

列表展示各 Code 片段摘要；**+** 添加 Code，铅笔编辑。Code 在 Monaco 中编写 KScript（`language: typescript`，KScript 模式）。

<DocImage src="/cms/settings/settings-request-hooks-code-dialog.png" alt="Code 编辑弹窗" width="1120" />

编辑器内注释示例变量（因事件而异）：

- 通用：`k.event.url`、`k.event.userAgent`、`k.event.culture`  
- 路由相关：`k.event.page`、`k.event.view`、`k.event.route`  

例如 **RouteFinding** 中可按 URL 片段重写：`k.event.url = "/pageone"`（仅在该事件有效）。

### 编辑条件

<DocImage src="/cms/settings/settings-request-hooks-condition-dialog.png" alt="编辑条件" width="1120" />

| 列 | 说明 |
|----|------|
| **左值** | 当前事件可用字段（`BusinessRule/ConditionOption`） |
| **运算符** | 随左值变化（如 `==`、`contains` 等） |
| **右值** | 比较常量或表达式片段 |

可 **+** 增加条件行；确认写回规则 `if` 数组。条件求值由 `ConditionManager.Evaluate` 在运行时执行。

### 保存与离开

底部 **KBottomBar**：**返回** 回列表；**保存** 调用 `BusinessRule/post`（`eventName` + `rules`）。离开页面前若有未保存修改会提示（`useSaveTip`）。

## 执行顺序（概念）

1. 站点 `enableFrontEvents` 为真。  
2. 请求进入对应 FrontEvent（如 `RouteFinding`）。  
3. 按列表顺序执行该事件下各规则；IF-ELSE-THEN 先判条件再走 THEN/ELSE；DO 顺序执行 Code。  

具体执行由站点前台请求 Hook 引擎负责，按本页配置顺序运行。

## 典型工作流

1. 确认 [高级 Tab](./basic/advanced.md) 与本页 **Hook 开启**。  
2. **新建 Hook** → 选 **RouteFinding**（或目标事件）。  
3. 添加 **IF-ELSE-THEN** 或 **DO** 规则，编写条件与 Code。  
4. **保存** 后在前台用目标 URL 验证行为。  
5. 协作导出时 Hook 对应站点包中的 **BusinessRule** 资源。

## 相关

| 文档 | 说明 |
|------|------|
| [站点设置概述](./index.md) | 菜单与子路由 |
| [基础设置 · 高级](./basic/advanced.md) | `enableFrontEvents` |
| [操作 Hook](./action-hooks.md) | 后台操作事件 |
| [开发 → Code](../development/code.md) | KScript 代码资源 |
