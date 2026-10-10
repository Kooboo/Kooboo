# AI 服务

> 菜单：**站点设置 → AI 服务**（高级菜单）
> 同页入口：**运维中心 → AI 服务**
> 深链：`/_Admin/system/ai-service?SiteId={站点GUID}`
> Tab 深链：`name=settings` / `functions` / `vectorSearch` / `llms` / `markdown` / `jsonld`

**AI 服务**用于配置站点级 AI 能力：向量搜索、MCP 服务、AI 函数、`llms.txt`、自动 Markdown 输出与全局 JSON-LD 映射。它在 **站点设置** 与 **运维中心** 各有一个菜单项，打开的是同一页面；本文只维护一份操作说明。

::: tip 权限
| 操作 | 权限 |
|------|------|
| 打开 **AI 服务** 页面 | `ai` |
| 查看自定义 AI 函数列表 | `ai·view` |
| 新增、编辑、删除 AI 函数 | 界面按钮检查 `mcp·edit`；后端 `mcp/PostTool`、`mcp/DeleteTools` 检查 `ai·edit` |
| 运行自定义 AI 函数 | 界面按钮检查 `mcp·view` |
| 保存 **MCP 服务** 与 **向量搜索** 设置 | 页面底部保存按钮检查 `mcp·edit`，并保存站点 AI 设置 |
| 保存 `llms.txt`、Markdown、JSON-LD 配置 | 需能进入本页；其中 Markdown / JSON-LD 开关会保存站点 AI 设置 |
:::

::: info 支持范围
当前后台页面已开放并有服务端接口的 Tab 包括 **MCP / AI 函数**、**llms.txt 生成器**、**自动 Markdown 引擎**、**全局 JSON-LD 映射**。Pay-per-crawl 相关能力当前界面未开放，本篇不写。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **站点设置 → AI 服务**，或勾选 **运维中心 → AI 服务**。
2. 左侧打开对应菜单。两处入口都会进入 `/_Admin/system/ai-service`。

<DocImage src="/cms/settings/settings-ai-service-overview.png" alt="AI 服务设置页" width="1120" />

## 页面结构

页面按 Tab 组织，切换时 URL 写入 `name` 参数。

<DocImage src="/cms/settings/settings-ai-service-tabs.png" alt="AI 服务 Tab" width="1120" />

| Tab | 用途 | 备注 |
|-----|------|------|
| **设置** | 开启向量搜索、选择 Embedding Provider / Model、开启 MCP 服务 | 默认 Tab |
| **AI 函数** | 管理站点自定义函数，查看内置函数，并可试运行自定义函数 | 写入站点 `AIFunction` 仓库 |
| **向量搜索** | 查看各类对象索引状态，执行同步或重建 | 仅在 **设置** 中启用向量搜索后显示 |
| **llms.txt 生成器** | 生成、编辑并保存站点根路径 `/llms.txt` | 读取在线页面生成索引 |
| **自动 Markdown 引擎** | 开启 `.md` 路由输出，并维护覆盖规则 | 保存站点 `AI.MarkdownEngine` |
| **全局 JSON-LD 映射** | 将内容 / 数据库字段映射到 Schema.org 属性 | 保存站点 `AI.JsonLD` 与映射 |

## 设置

设置 Tab 先配底层能力，再去其它 Tab 同步或编写函数。

<DocImage src="/cms/settings/settings-ai-service-settings.png" alt="AI 服务设置 Tab" width="1120" />

### 向量搜索

| 项 | 说明 |
|----|------|
| **启用向量搜索** | 开启后站点保存 `ai.vectorSearch.enable`，页面会出现 **向量搜索** Tab |
| **Provider** | 从支持 Embedding Models 的 AI Provider 中选择；若下拉为空，先确认组织级 AI Provider 是否可用 |
| **Model** | 选择该 Provider 下的 Embedding 模型；切换 Provider 时会自动带出第一个可用模型 |

保存后，新建或更新的内容会按向量搜索逻辑进入索引；已有对象建议到 [向量搜索](#向量搜索) Tab 执行一次同步或重建。

### MCP 服务

| 项 | 说明 |
|----|------|
| **启用 MCP 服务** | 开启站点 MCP Server，使外部 MCP Client 可以连接并调用站点工具 |
| **Development Connection Information** | 非在线服务器会显示开发连接信息：`Endpoint` 与 `Authorization Header` |
| **禁用内置 AI 工具** | 关闭系统内置工具，只暴露用户创建的 AI 函数 |

`Endpoint` 由站点 [Base URL](./basic/general.md#站点信息) 推导，并追加 `_mcp`；若 Base URL 仍是 `http`，页面显示时会转为 `https`。开发连接里的 Bearer Token 是当前登录用户凭据，只用于本机调试，不应写入文档、截图或代码仓库。

## AI 函数

AI 函数 Tab 分为 **自定义** 与 **内置**。自定义函数可被 MCP Server 暴露，也可被站点内 AI 调用；内置函数由系统提供，只在列表中查看名称与描述。

<DocImage src="/cms/settings/settings-ai-service-functions.png" alt="AI 函数自定义列表" width="1120" />

若当前账号没有 `mcp·edit`，**添加工具** 按钮会显示为禁用态；请先在 [角色权限](./roles.md) 中补齐权限。

### 自定义列表

| 控件 / 列 | 说明 |
|-----------|------|
| **添加工具** | 打开 [AI 工具弹窗](#ai-工具弹窗) |
| **自定义 / 内置** | 右上角切换当前列表 |
| **名称** | 自定义工具名称；新建保存后不建议再改 |
| **标签** | 从 `mcp/FunctionTags` 返回的标签中选择，用于分类和发现 |
| **描述** | 给 AI 或调用方理解用途的自然语言说明 |
| **运行** | 多选中仅选一条时显示，打开参数与结果弹窗 |
| **编辑 / 删除** | 编辑当前工具或多选删除 |

### AI 工具弹窗

| 字段 | 说明 |
|------|------|
| **名称** | 新建必填、唯一，长度 1～50；编辑已有工具时禁用 |
| **描述** | 说明函数解决什么问题、何时调用、返回什么结果 |
| **标签** | 多选标签，便于内置发现逻辑筛选 |
| **执行代码** | Monaco TypeScript 编辑器，启用 KScript 模式；这里只说明入口，脚本 API 见文末相关链接 |
| **Input Schema** | JSON Schema，用于声明运行参数；运行弹窗会据此生成示例输入 |

描述与 Input Schema 比代码更影响 AI 调用质量。实施时建议先把 **函数边界、必填参数、返回结构** 写清楚，再填执行代码；不要把密钥、外部 Token 或长期凭证硬编码到函数里。

### 运行检查

点击 **运行** 后，系统读取 Input Schema 并生成一份 JSON 参数。确认或调整参数后点 **运行**，后台调用 `mcp/RunTool` 并显示返回结果。若 Schema 没有属性，弹窗会直接运行并进入结果页。

### 内置工具

<DocImage src="/cms/settings/settings-ai-service-built-in-tools.png" alt="AI 函数内置工具列表" width="1120" />

内置列表只展示 **名称** 与 **描述**。若不希望 MCP Client 调用内置工具，回到 [设置](#mcp-服务) 打开 **禁用内置 AI 工具**。

## 向量搜索

向量搜索 Tab 只在 **设置 → 启用向量搜索** 并保存后出现。列表按对象类型展示索引状态。

| 列 / 操作 | 说明 |
|-----------|------|
| **名称** | 对象类型，如商品、页面、视图、内容等 |
| **总数** | 当前可索引对象数量 |
| **未同步** | 尚未生成向量，或对象更新时间 / 模型与索引记录不一致 |
| **排队中** | 已提交但未完成的索引任务 |
| **状态** | 显示运行中、错误、待同步或已完成 |
| **同步** | 对所选对象类型只处理未同步项 |
| **重建** | 删除所选类型已有向量后重新索引全部对象 |

切换 Embedding Provider 或 Model 后，旧索引模型与当前模型不一致，通常需要对关键对象执行 **重建**。

## llms.txt 生成器

`llms.txt` 用于给 AI 爬虫提供站点内容索引。后台读取并保存的是站点文件 `/llms.txt`。

<DocImage src="/cms/settings/settings-ai-service-llms.png" alt="llms.txt 生成器" width="1120" />

| 控件 | 说明 |
|------|------|
| **生成** | 根据当前站点在线页面生成一份 `llms.txt` 草稿；每个页面链接到对应 `.md` 地址 |
| **文本框** | 可手动编辑最终内容 |
| **保存** | 将文本框内容写入根路径 `/llms.txt` |

生成前建议先确认页面已上线、路由名称正确，并已按需开启 [自动 Markdown 引擎](#自动-markdown-引擎)，这样 `llms.txt` 指向的 `.md` 链接更容易被 AI 客户端读取。

## 自动 Markdown 引擎

自动 Markdown 引擎开启后，访问页面路径追加 `.md` 或 `index.html.md` 时，系统会找到原页面路由并把 HTML 渲染结果转换为 Markdown。

<DocImage src="/cms/settings/settings-ai-service-markdown.png" alt="自动 Markdown 引擎" width="1120" />

### 启用开关

右上角 **启用** 开关会立即保存站点 `AI.MarkdownEngine.Enable`。关闭后，`.md` 路由不会按 Markdown 渲染。

### 覆盖规则

覆盖规则用于为部分路径指定适用对象或禁用规则。

<DocImage src="/cms/settings/settings-ai-service-markdown-rule-dialog.png" alt="Markdown 覆盖规则弹窗" width="1120" />

| 字段 | 说明 |
|------|------|
| **匹配路径** | 需要覆盖的访问路径；建议使用真实前台路径，方便和页面路由核对 |
| **适用对象** | 该路径要使用的对象标识 |
| **状态** | 启用或禁用该覆盖规则 |

新增、编辑、删除规则只改变当前表单状态，最后仍需点击底部 **保存** 写入站点。

## 全局 JSON-LD 映射

全局 JSON-LD 映射把 Kooboo 内容或数据库字段映射到 Schema.org 类型与属性。开启后，页面渲染时会输出 `application/ld+json` 结构化数据。

<DocImage src="/cms/settings/settings-ai-service-jsonld.png" alt="全局 JSON-LD 映射" width="1120" />

### 启用开关

右上角 **启用** 开关保存站点 `AI.JsonLD.Enable`。建议先完成至少一个数据源映射，再开启到线上站点。

### 数据源与字段映射

| 区域 | 说明 |
|------|------|
| **Kooboo 数据源** | 可选 `content`、`indexeddb`、`sqlite`、`mysql`、`sqlserver`、`mongo` 等当前站点可访问的数据源 |
| **数据** | 选择具体内容文件夹或表 |
| **架构类型** | 从 Schema.org 类型树中选择，如 Article、Product、Organization 等 |
| **Kooboo 变量/字段** | 当前数据源返回的字段列表 |
| **Schema 属性** | 为每个字段选择要映射的 Schema.org 属性 |

内容数据源的字段来自 [内容类型](../content/content-types.md)；数据库数据源的字段来自对应表结构。若下拉里没有预期字段，先回到内容或数据库模块检查模型是否已经保存。

## 典型工作流

1. 先确认站点 [Base URL](./basic/general.md#站点信息) 与需要的 AI Provider / Embedding 模型可用。
2. 在 **设置** 中按需开启 **MCP 服务**、**向量搜索**，保存。
3. 在 **AI 函数** 中创建自定义工具：写清名称、描述、标签、Input Schema，再补执行代码并用 **运行** 验证。
4. 若启用向量搜索，进入 **向量搜索** Tab 对已有对象执行 **同步** 或 **重建**。
5. 若希望 AI 爬虫理解站点结构，开启 **自动 Markdown 引擎**，再用 **llms.txt 生成器** 生成并保存 `/llms.txt`。
6. 若页面需要结构化数据，先在内容类型或数据库表中整理字段，再到 **全局 JSON-LD 映射** 选择 Schema.org 类型并映射字段。

## CMS 与 API / 模板分工

| 场景 | 看 CMS | 看 API / 模板引擎 |
|------|--------|-------------------|
| 开启 MCP 服务、复制开发连接信息 | 本篇 **设置** | — |
| 创建 AI 函数、填写描述与 Input Schema | 本篇 **AI 函数** | [开发 → 代码](../development/code.md)、[k.site Code API](../../api/site/code.md)、[KScript API](../../api/index.md) |
| 函数里读取内容、数据库或电商数据 | 本篇只说明函数入口 | [k.content](../../api/content/index.md)、[k.DB](../../api/database/index.md)、[k.commerce](../../api/commerce/commerce.md) |
| 页面输出 Markdown 或 `llms.txt` | 本篇开关与保存路径 | [Page](../../templateEngine/page/index.md)、[View](../../templateEngine/view/index.md)、[模板绑定](../../templateEngine/binding/index.md) |
| JSON-LD 字段来自内容或表 | 本篇选择数据源与字段映射 | [内容类型](../content/content-types.md)、[数据库](../database/index.md)、[k-data](../../templateEngine/k-data/index.md) |

## 相关

| 文档 | 说明 |
|------|------|
| [站点设置概述](./index.md) | 站点设置菜单与子路由 |
| [运维中心](../operations/index.md) | AI 服务的同页入口 |
| [基础设置 · 基础](./basic/general.md) | Base URL 与站点信息 |
| [开发 → 代码](../development/code.md) | KScript / TypeScript 代码资源 |
| [内容类型](../content/content-types.md) | JSON-LD 内容字段来源 |
| [数据库](../database/index.md) | JSON-LD 数据库字段来源 |
| [KScript API](../../api/index.md) | AI 函数执行代码可用 API |
