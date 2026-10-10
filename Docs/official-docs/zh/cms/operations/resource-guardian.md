# 资源守护

> 菜单：**运维中心 → 资源守护**  
> 深链：`/_Admin/system/resource-guardian?SiteId={站点GUID}`

监控站点 **静态资源** 的请求与流量：按文件类型统计带宽、资源构成与访问排行、实时请求清单、机器人访问，以及 **防盗链 / 未授权采集** 防护。须先 **启用** 资源日志后，统计卡片与图表才会加载；**网站防护** 开关控制是否按防护规则拦截站外引用。

::: tip 权限
菜单权限与访问统计相同：`visitorLog`（查看）。须在编辑菜单中勾选 **运维中心 → 资源守护**。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **运维中心 → 资源守护**。
2. 左侧 **运维中心 → 资源守护**。

进入后，主屏自上而下为页头控件、防护入口、指标卡片、带宽分析、资源构成与排行、实时日志、机器人情报、外部站点列表；下文按块配图说明。

<DocImage src="/cms/operations/resource-guardian-overview.png" alt="资源守护主屏总览" width="1120" />

## 页头

路径：`/_Admin/system/resource-guardian?SiteId=...`

| 控件 | 说明 |
|------|------|
| **启用** | `enableResourceLog`；关闭后清空统计，仅保留防护配置入口 |
| **日期范围** | 全局 `userBehaviorDateRange`（与行为分析等模块共用），变更后刷新 `ResourceLog/Info` |

<DocImage src="/cms/operations/resource-guardian-header.png" alt="资源守护页头：启用与日期范围" width="1120" />

## 防护入口

| 控件 | 说明 |
|------|------|
| **配置防护规则** | 跳转 [防护规则配置](#防护规则配置子页)（`resource-guardian/protection-rules`） |
| **网站防护** | `enableGuardian`；开启后按规则对站外请求执行禁止 / 重定向 / 水印等处理 |

<DocImage src="/cms/operations/resource-guardian-toolbar.png" alt="配置防护规则与网站防护开关" width="1120" />

## 总览指标（四张卡片）

启用资源日志且选定日期范围后显示：

| 卡片 | 说明 |
|------|------|
| **总资源流量** | 区间内资源请求带宽合计 |
| **无效资源** | 404 等资源数 + **来源** 引用页数；点击卡片打开明细弹窗（资源 / 来源 Tab） |
| **防护拦截次数** | 被防护规则拦截的请求次数；点击卡片查看被拦资源列表 |
| **流量构成** | 机器人与真人访问占比（`machineRate` / `humanRate`） |

<DocImage src="/cms/operations/resource-guardian-stat-cards.png" alt="总资源流量、无效资源、防护拦截、流量构成" width="1120" />

### 无效资源弹窗

点击 **无效资源** 卡片后，可切换 **无效资源** / **来源** 查看 Top 列表：资源 URL、使用位置、请求数、引用次数等。

<DocImage src="/cms/operations/resource-guardian-invalid-dialog.png" alt="无效资源明细弹窗" width="1120" />

### 防护拦截弹窗

点击 **防护拦截次数** 卡片后，表格列出被拦 **资源**、**请求数** 及主要 **来源** 域名。

<DocImage src="/cms/operations/resource-guardian-protected-dialog.png" alt="防护拦截资源弹窗" width="1120" />

## 带宽分析区块

按 **文件类型** 展示水平堆叠条与图例：各类型带宽占比及实际大小（`bandwidthByFileType`）。无数据时不显示该区块。

<DocImage src="/cms/operations/resource-guardian-bandwidth.png" alt="带宽分析：按文件类型堆叠条" width="1120" />

## 站点资源构成与资源排行榜

同一行左右两栏：

| 区块 | 说明 |
|------|------|
| **站点资源构成** | 图片、脚本、样式、文件、页面等类型数量与占比条 |
| **资源排行榜** | Tab：**资源** / **来源** / **系统资源**；条形图按请求数排序，可打开资源链接 |

<DocImage src="/cms/operations/resource-guardian-composition-ranking.png" alt="站点资源构成与资源排行榜" width="1120" />

## 实时资源清单和使用情况跟踪

主屏表格约 **5** 条实时记录：**资源 URL**、**引用位置**、**请求位置**、**使用情况分析**（带宽、请求数）；可展开查看与页面/视图等的关联。**查看实时数据** → [资源日志子页](#资源日志子页)。

<DocImage src="/cms/operations/resource-guardian-live-log.png" alt="实时资源清单主屏表格" width="1120" />

## 机器人情报分析

与 [访问统计](./visitor-logs.md) 相同组件：展示资源请求场景下的 `botInfo` 摘要（主屏约 5 条）。

<DocImage src="/cms/operations/resource-guardian-bot-intelligence.png" alt="机器人情报分析" width="1120" />

## 正在使用你资源的外部站点

表格外链域名：**站点域名**、**请求数**、**带宽用量**、**状态**（已阻止 / 已放行）。用于发现盗链或异常站外引用。

<DocImage src="/cms/operations/resource-guardian-external-domains.png" alt="外部站点使用本站点资源列表" width="1120" />

## 子页面

### 防护规则配置子页

路径：`/_Admin/system/resource-guardian/protection-rules?SiteId=...`

面包屑：**资源守护 → 防护规则配置**。规则表字段：**优先级**、**起始路径**、**受保护的扩展名**、**阻断类型**（强力阻断 403 / 重定向 / 水印）；行内 **编辑**、**删除**。**添加规则** 打开弹窗。底部 **保存** 写回 `ResourceLog/UpdateSetting`。

<DocImage src="/cms/operations/resource-guardian-protection-rules.png" alt="防护规则配置列表" width="1120" />

#### 添加 / 编辑规则弹窗

可配置：起始路径、阻断类型、重定向 URL（重定向时）、水印文案/透明度/颜色/字号/位置、受保护扩展名、黑名单域名等。

<DocImage src="/cms/operations/resource-guardian-rule-dialog.png" alt="添加防护规则弹窗" width="1120" />

### 资源日志子页

路径：`/_Admin/system/resource-guardian/log-list?SiteId=...`

面包屑：**资源守护 → 实时资源清单和使用情况跟踪**。完整分页/列表：**状态**、**资源 URL**、**类型**、**带宽** 等（以界面列为准）。

<DocImage src="/cms/operations/resource-guardian-log-list.png" alt="资源日志完整列表子页" width="1120" />

::: tip 未启用资源日志
关闭页头 **启用** 时，主屏统计与图表为空，并提示「未启用资源日志」；开启后可查看请求日志与流量统计。防护规则仍可提前配置，需同时打开 **网站防护** 才会生效。
:::

## 相关

- [运维中心概述](./index.md)  
- [访问统计](./visitor-logs.md) — 全站访客与页面访问  
- [站点设置](../navigation.md#站点设置) — 资源 CDN、缓存等（待补充专题文档）
