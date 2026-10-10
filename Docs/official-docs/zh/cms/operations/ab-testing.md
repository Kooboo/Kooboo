# AB 测试

> 菜单：**运维中心 → AB 测试**  
> 深链：`/_Admin/system/ab-testing?SiteId={站点GUID}`

为站点配置 **A/B 实验**：选定实验对象（页面、视图、商品等）作为 **对照组 A**，配置一个或多个 **变体 B/C…**（整对象替换或字段替换），按流量占比与可选用户条件分流访客，并以 **Goal** 作为获胜指标查看报告。

::: tip 权限
菜单权限：`abTesting`。须在编辑菜单中勾选 **运维中心 → AB 测试**（及父级 **运维中心**）。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **运维中心 → AB 测试**。
2. 左侧 **运维中心 → AB 测试**。

主屏为实验列表；**创建 A/B 测试** 进入四步向导；列表行内进入 **实验报告**。下文按块配图说明。

<DocImage src="/cms/operations/ab-testing-overview.png" alt="AB 测试列表总览" width="1120" />

## 页头与全局开关

路径：`/_Admin/system/ab-testing?SiteId=...`

| 控件 | 说明 |
|------|------|
| **启用** | 站点级 AB 测试总开关（`updateAbTestStatus`）；关闭后前台不再分流 |
| （列表区） | 见下节 |

<DocImage src="/cms/operations/ab-testing-header.png" alt="AB 测试页头与启用开关" width="1120" />

## 列表工具栏

| 控件 | 说明 |
|------|------|
| **创建 A/B 测试** | 跳转 `/_Admin/system/ab-testing/create?SiteId=...` |
| **状态** | 筛选：草稿、进行中、已暂停、已结束 |
| **搜索** | 按实验名称、描述关键字过滤 |

<DocImage src="/cms/operations/ab-testing-list-toolbar.png" alt="创建按钮与筛选" width="1120" />

## 实验列表

表格列说明：

| 列 | 说明 |
|----|------|
| **实验名称** | 名称 + 描述摘要 |
| **状态** | 草稿 `Draft`、进行中 `Running`、已暂停 `Paused`、已结束 `Completed` / `Archived` |
| **流量占比** | **对照组 A**（100% − 各变体权重之和）与各 **变体** 权重百分比 |
| **进行进度** | 进行中时显示已进行天数 / 总天数及起止时间 |
| **操作** | **查看报告**（已发布且已过开始时间）或进入详情；**删除** 实验 |

点击实验名称区域或 **查看报告** → [实验报告](#实验报告子页)。

<DocImage src="/cms/operations/ab-testing-list-table.png" alt="AB 测试实验列表" width="1120" />

## 创建 / 编辑实验（四步向导）

路径：`/_Admin/system/ab-testing/create?SiteId=...`  
编辑：同上并带 `name={实验名}` 查询参数，从列表预填数据。

底部：**上一步**、**下一步** / **保存**；**取消** 返回列表。

### 步骤 1：基本信息

| 字段 | 说明 |
|------|------|
| **实验名称** | 必填，唯一标识 |
| **实验描述** | 可选 |
| **开始日期 / 结束日期** | 实验时间窗；结束须晚于开始 |

<DocImage src="/cms/operations/ab-testing-create-step-basic.png" alt="创建向导：基本信息" width="1120" />

### 步骤 2：实验目标

| 字段 | 说明 |
|------|------|
| **对象类型** | 如 Page、View、Product、TextContent 等（`ObjectOptions`） |
| **测试对象** | 对照组 A 绑定的具体对象（可搜索） |
| **获胜指标** | 选择 **Goal**（来自行为分析流量设置）；可 **编辑 Goal** 打开 Goal 管理弹窗 |

<DocImage src="/cms/operations/ab-testing-create-step-goal.png" alt="创建向导：实验目标与获胜指标" width="1120" />

### 步骤 3：实验配置

为每个变体配置：

| 模式 | 说明 |
|------|------|
| **替换整个对象** | 选择另一对象作为变体内容 |
| **替换字段** | 仅当对象类型支持 `fieldOverWrites` 时可用；按字段 + 多语言填写替换值 |

可 **添加变体**、删除变体、修改变体名称。对照组 A 为步骤 2 所选对象，不在此重复选择。

<DocImage src="/cms/operations/ab-testing-create-step-config.png" alt="创建向导：变体配置" width="1120" />

### 步骤 4：流量与用户分配

| 区块 | 说明 |
|------|------|
| **流量分配** | 对照组 A 占比 = 100% − 所有变体权重；各变体设置 0–100% 权重，总和不得超过 100% |
| **用户分组条件** | 可选：属性 + 比较器 + 值（如国家、设备等）；可 **添加条件** |

<DocImage src="/cms/operations/ab-testing-create-step-traffic.png" alt="创建向导：流量与用户分配" width="1120" />

## 实验报告子页

路径：`/_Admin/system/ab-testing/report?SiteId=...&name={实验名}`

顶栏：返回、实验名、状态标签；按状态显示 **编辑**、**开始实验**、**暂停实验**、**重新开始**、**提前结束** 等按钮。

### 实验详情 Tab

包含：

- **基本信息**：名称、描述、目标用户、获胜指标、实验时长与时间范围  
- **实验配置**：对照组与各变体配置摘要  
- **流量分配**：各组占比  

<DocImage src="/cms/operations/ab-testing-report-detail.png" alt="实验报告：实验详情 Tab" width="1120" />

### 实验报告 Tab

实验 **进行中或已结束后** 展示：

- **实验结论**  
- **实验组数据**（各变体表现）  
- **置信度统计**  

草稿或未开始时显示空状态提示（尚无报告数据）。

<DocImage src="/cms/operations/ab-testing-report-report.png" alt="实验报告：实验报告 Tab" width="1120" />

::: tip Goal 与行为分析
**获胜指标** 依赖行为分析中的 Goal 配置。若下拉为空，请先在 **行为分析**（或相关流量设置）中维护 Goal，再在创建向导中 **编辑 Goal** 添加。
:::

## 相关

- [运维中心概述](./index.md)  
- [页面交互](./page-interaction.md) — 锚点点击统计  
- 行为分析 — Goal、用户分群
