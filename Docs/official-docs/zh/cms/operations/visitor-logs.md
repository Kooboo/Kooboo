# 访问统计

> 菜单：**运维中心 → 访问统计**  
> 深链：`/_Admin/system/visitor-logs?SiteId={站点GUID}`

查看站点 **访客与访问** 数据：浏览量、独立访客、机器人占比、趋势图、设备与地域分布、来源与浏览器排行，以及最新访问明细。数据来自访客日志采集（`enableVisitorLog`）；关闭 **启用** 开关会停止采集（立即写回站点设置）。

::: tip 权限
菜单权限：`visitorLog`（查看）。须在编辑菜单中勾选 **运维中心 → 访问统计**。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **运维中心 → 访问统计**。
2. 左侧 **运维中心 → 访问统计**。

进入后，主屏自上而下为指标卡片、趋势与排行、机器人情报、设备与地域分布等区块；下文按块配图说明。

<DocImage src="/cms/operations/visitor-logs-overview.png" alt="访问统计主屏总览：指标、趋势、排行与机器人情报等" width="1120" />

## 页头

路径：`/_Admin/system/visitor-logs?SiteId=...`

面包屑为 **访问统计**。右侧为全局控件，变更后刷新本页全部图表与子页数据。

| 控件 | 说明 |
|------|------|
| **启用** | `enableVisitorLog`；切换后立即 `saveSite`，开启/关闭采集 |
| **日期范围** | 全局 `daterange`（`visitorLogDateRange`），写入各子页 query（`startDate` / `endDate`） |

<DocImage src="/cms/operations/visitor-logs-header.png" alt="访问统计页头：启用开关与日期范围" width="1120" />

## 总览指标（四张卡片）

所选日期区间内，页顶横向四张卡片：

| 卡片 | 字段 | 说明 |
|------|------|------|
| **总访问量** | `pageViews` | 页面浏览次数合计 |
| **独立访客** | `uniqueVisitors` | 去重访客数 |
| **访客构成** | `machineRate` / `humanRate` | 机器人与真人占比（图标 + 百分比） |
| **错误率** | `errorRate` | 访问错误占比 |

<DocImage src="/cms/operations/visitor-logs-summary-cards.png" alt="访问统计：总访问量、独立访客、访客构成、错误率" width="1120" />

## 人类流量趋势与访客排行榜

同一行左右布局：**人类流量趋势** 为按日的 `dayInfo` 折线/柱图，可在图内切换 **访客数 / PV** 维度；**访客排行榜** 为 Tab 切换的条形排行，主屏各 Tab 仅显示前 **4** 条。

| Tab | 子页路由名 | 说明 |
|-----|------------|------|
| **页面访问排行** | `top-pages-panel` | URL 与访问次数 |
| **来源访问排行** | `top-referer-panel` | Referer 与访问次数 |

右下角 **查看更多** 会带上当前日期范围，跳转到当前 Tab 对应的完整列表子页。

<DocImage src="/cms/operations/visitor-logs-trend-ranking.png" alt="人类流量趋势图与访客排行榜" width="1120" />

## 机器人情报分析

展示 `botInfo` 摘要：常见爬虫/机器人访问概况（主屏约 5 条）。**查看更多** → [机器人情报子页](#机器人情报子页)。

<DocImage src="/cms/operations/visitor-logs-bot-intelligence.png" alt="机器人情报分析卡片" width="1120" />

## 像素密度与分辨率区间分布

左侧 **像素密度**（`dpr`）：按 1.0x / 2.0x 等 DPR 区间统计占比，并给出高清屏占比等摘要文案。右侧 **分辨率区间分布**（`viewportWidth`）：按视口宽度区间的访问分布。

<DocImage src="/cms/operations/visitor-logs-pixel-viewport.png" alt="像素密度与分辨率区间分布" width="1120" />

## 主题颜色使用分布与流量来源排名

左侧 **主题颜色使用分布**（`colorScheme`）：访客系统浅色/深色主题偏好（环形图）。右侧 **流量来源排名**（`topReferer` 前若干条）：Referer 条形排行。**查看更多** → [来源访问排行子页](#来源访问排行子页)。

<DocImage src="/cms/operations/visitor-logs-color-traffic-source.png" alt="主题颜色分布与流量来源排名" width="1120" />

## 热门国家访客

整行卡片：左侧 **世界地图**（`countryCH`），右侧列表。Tab 可切换排行维度：

| Tab | 说明 |
|-----|------|
| **访客数** | 按国家/地区访客量 |
| **RTT** | 往返时延相关指标 |
| **网速** | 网速相关指标 |

**查看更多** → [国家访客子页](#国家访客子页)。

<DocImage src="/cms/operations/visitor-logs-popular-countries.png" alt="热门国家访客：地图与排行列表" width="1120" />

## ECT 网络类型分布与浏览器访问排行

左侧 **ECT 网络类型分布**（`ectShare`）：有效连接类型（如 Wi‑Fi/4G/以太网等）占比环图。右侧 **浏览器 访问排行**（`topBrowsers` 前若干条）：各浏览器访问量条形图。**查看更多** → [浏览器排行子页](#浏览器排行子页)。

<DocImage src="/cms/operations/visitor-logs-ect-browser.png" alt="ECT 分布与浏览器访问排行" width="1120" />

## 最新访客信息

主屏嵌入 `AllPanel` 表格（约 **5** 条）：IP、国家、访问页面、开始时间、ECT、RTT 等。行内可对访客 **封禁**、**限速**（依赖站点访问限制配置）。**查看更多** → [最新访客子页](#最新访客子页)。

<DocImage src="/cms/operations/visitor-logs-latest-visitors.png" alt="最新访客信息主屏表格" width="1120" />

## 子页面（查看更多）

子路由均带当前 **日期范围**（`startDate` / `endDate`），面包屑为 **访问统计 → {子页标题}**。页头仍保留 **启用** 与 **日期范围**。

### 最新访客子页

路径：`/_Admin/system/visitor-logs/all-panel?SiteId=...&startDate=...&endDate=...`

分页完整列表：IP、国家、页面 URL、开始时间、ECT、RTT、User-Agent 等；支持封禁/限速。

<DocImage src="/cms/operations/visitor-logs-all-panel.png" alt="最新访客信息子页：分页表格" width="1120" />

### 机器人情报子页

路径：`/_Admin/system/visitor-logs/bot-panel?SiteId=...`

较主屏卡片展示完整 `botInfo` 列表（`RobotIntelligence` 组件，无条数上限）。

<DocImage src="/cms/operations/visitor-logs-bot-panel.png" alt="机器人情报子页" width="1120" />

### 页面访问排行子页

路径：`/_Admin/system/visitor-logs/top-pages-panel?SiteId=...`

表格列：**名称**（页面 URL）、**大小**、**浏览量**（`count`）。

<DocImage src="/cms/operations/visitor-logs-top-pages-panel.png" alt="页面访问排行子页" width="1120" />

### 来源访问排行子页

路径：`/_Admin/system/visitor-logs/top-referer-panel?SiteId=...`

完整 Referer 访问次数排行（与主屏「流量来源排名」同源字段）。

<DocImage src="/cms/operations/visitor-logs-top-referer-panel.png" alt="来源访问排行子页" width="1120" />

### 图片访问排行子页

路径：`/_Admin/system/visitor-logs/top-image-panel?SiteId=...`

图片资源 URL、缩略图与访问次数（站点有图片访问数据时才有内容）。

<DocImage src="/cms/operations/visitor-logs-top-image-panel.png" alt="图片访问排行子页" width="1120" />

### 国家访客子页

路径：`/_Admin/system/visitor-logs/top-countries-panel?SiteId=...`

国家/地区维度完整列表（访客数、RTT、网速等，以界面列为准）。

<DocImage src="/cms/operations/visitor-logs-top-countries-panel.png" alt="国家访客子页" width="1120" />

### 浏览器排行子页

路径：`/_Admin/system/visitor-logs/top-browsers-panel?SiteId=...`

各浏览器类型完整访问排行。

<DocImage src="/cms/operations/visitor-logs-top-browsers-panel.png" alt="浏览器排行子页" width="1120" />

::: tip 封禁与限速
子页与主屏表格中的封禁/限速与 **站点设置 → 基础设置** 中的访问限制、限速策略联动；详见 [基础设置 · 访问控制](../settings/basic/access.md)。
:::

## 相关

- [运维中心概述](./index.md)  
- [控制面板](../site/dashboard.md) — 概览 PV  
- 下一菜单项：**行为分析**
