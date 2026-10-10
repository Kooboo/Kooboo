# 页面交互

> 菜单：**运维中心 → 页面交互**  
> 深链：`/_Admin/system/page-interaction?SiteId={站点GUID}`

统计站点 **页面锚点**（`k-track`）的点击：各页面触发次数、访问次数、触发率，以及全站锚点排行与单页锚点分布。可在 **页面详情** 中预览页面、查看锚点数据并编辑锚点；**全屏锚点编辑器** 用于在真实页面上添加、修改或删除锚点。

::: tip 采集开关
页头 **启用** 对应站点字段 `enablePageInteraction`；关闭后不再采集锚点点击。保存站点设置后立即生效。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **运维中心 → 页面交互**。
2. 左侧 **运维中心 → 页面交互**。

进入后为主屏统计区；点击 **编辑锚点** 或表格中的页面链接会在 **新标签页** 打开 [页面详情](#页面详情子页)。下文按块配图说明。

<DocImage src="/cms/operations/page-interaction-overview.png" alt="页面交互主屏总览" width="1120" />

## 页头

路径：`/_Admin/system/page-interaction?SiteId=...`

| 控件 | 说明 |
|------|------|
| **启用** | `enablePageInteraction`；`saveSite` 写回站点配置 |
| **日期范围** | 全局 `userBehaviorDateRange`（默认近 7 日），变更后请求 `PageInteraction/GetStats` |

<DocImage src="/cms/operations/page-interaction-header.png" alt="页面交互页头：启用与日期范围" width="1120" />

## 编辑锚点入口

**编辑锚点** 按钮：按站点 **首页** 路径打开 [页面详情](#页面详情子页)（新标签页），便于从首页开始配置锚点。

<DocImage src="/cms/operations/page-interaction-edit-entry.png" alt="编辑锚点按钮" width="1120" />

## 页面锚点点击总览

区块标题：**页面锚点点击总览**。左侧为 **页面点击排行** 条形图（`Ranking`，按触发次数）；右侧表格列：

| 列 | 说明 |
|----|------|
| **页面** | 页面 URL；点击在新标签页打开该页详情 |
| **触发次数** | `count` |
| **访问次数** | `userCount` |
| **触发率** | `rate`（百分比） |

<DocImage src="/cms/operations/page-interaction-overview-block.png" alt="页面锚点点击总览：排行与表格" width="1120" />

## 锚点点击总数与页面锚点

下方左右两栏：

| 区块 | 说明 |
|------|------|
| **锚点点击总数** | 全站各锚点 `name`、所属 **页面**、触发次数、触发率 |
| **页面锚点** | 下拉选择页面；左侧 **预览锚点** 打开该页详情；右侧为当前页锚点点击排行 |

<DocImage src="/cms/operations/page-interaction-anchor-sections.png" alt="锚点点击总数与页面锚点排行" width="1120" />

## 页面详情子页

路径：`/_Admin/system/page-interaction/detail?SiteId=...&path=...&startDate=...&endDate=...&type=...&id=...`  
（新标签页打开，菜单仍高亮 **页面交互**。）

面包屑：**页面交互 → 详情**。右侧 **日期范围** 与主屏共用，变更后刷新当前页锚点统计。

### 页面预览区

| 控件 | 说明 |
|------|------|
| **路径** | 下拉切换站点内页面路径，确认后刷新预览 |
| **视口宽度** | 切换手机/平板/桌面等预览宽度 |
| **全屏** | 打开 [全屏锚点编辑器](#全屏锚点编辑器)（`/_Admin/visual-anchor-editor`） |

中间为页面 iframe 预览，已标注锚点元素；点击元素可在右侧查看数据。

### 右侧锚点列表

- **页面锚点** 列表：显示锚点名称与触发次数；**编辑** 进入批量删除模式（勾选后删除 `k-track` 绑定）。
- 点击某一锚点：底部展开 **「{名称} 锚点数据」**：总点击、访问次数、触发率。

<DocImage src="/cms/operations/page-interaction-detail.png" alt="页面详情：预览与锚点列表" width="1120" />

<DocImage src="/cms/operations/page-interaction-detail-anchor-stats.png" alt="页面详情：选中锚点的统计数据" width="1120" />

## 全屏锚点编辑器

路径：`/_Admin/visual-anchor-editor?SiteId=...&path=...&startDate=...&endDate=...`  
（通常从页面详情 **全屏** 进入。）

整页嵌入站点页面，用于可视化维护锚点：

- 点击页面元素打开 **上下文菜单**（编辑锚点、移除锚点等，依赖元素是否具备 Kooboo 绑定）。
- 浮动 **全局菜单**：切换预览宽度、查看锚点统计面板、批量删除锚点等。
- 锚点保存在 DOM 属性 **`k-track`**（锚点名称）上，与 Inline Design 绑定信息一并提交 API。

<DocImage src="/cms/operations/page-interaction-visual-editor.png" alt="全屏锚点编辑器" width="1120" />

::: tip 无数据时
若主屏表格为空，请确认已 **启用** 页面交互、所选日期内有访问，且前台页面已配置 `k-track` 锚点。可先从 **编辑锚点** 在首页添加锚点后再观察统计。
:::

## 相关

- [运维中心概述](./index.md)  
- [访问统计](./visitor-logs.md) — 全站访客 PV/UV  
- [页面](../site/pages.md) — 页面路径与内容编辑
