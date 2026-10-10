# SPA 多语言

> 菜单：**开发 → SPA 多语言**  
> 深链：`/_Admin/development/spamultilingual?SiteId={站点GUID}`

为 **单页应用（SPA）** 维护键值型多语言文案：在后台编辑各语言的翻译，前台通过 HTTP 接口 `/_spa/lang/{语言代码}` 拉取 JSON 词典。适用于 Vue/React 等前端自行管理界面语言、而不走 Kooboo 服务端模板渲染的场景。

::: tip 权限
查看列表：`spaMultilingual`。**导入**、**设置**（语言列）、行内 **编辑** 需 `spaMultilingual·edit`；多选 **删除** 需 `spaMultilingual·delete`。
:::

::: info 与「内容 → 标签」的区别
| | **SPA 多语言**（本页） | **[内容 → 标签](../content/labels.md)** |
|--|------------------------|--------------------------------------|
| 消费方式 | 前端请求 `/_spa/lang/{lang}` 取整表 JSON | 模板 / KScript：`k.label()`、`k.t()` |
| 典型场景 | SPA、静态前端打包进站点 | Layout / Page / View 服务端渲染 |
| 脚本 API | 无 `k.*` 读写；见 [SPA 多语言 API](../../api/spa-multilingual/index.md) | [k.site.labels](../../api/label/index.md) |

站点 **页面级多语言**（路由、文化切换）仍在 **站点设置 → 基础设置** 配置，见 [页面](../site/pages.md) 与 [k-data · culture](../../templateEngine/k-data/query.md#culture)。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → SPA 多语言**。
2. 左侧 **开发 → SPA 多语言**。

<DocImage src="/cms/development/spamultilingual-overview.png" alt="SPA 多语言列表总览" width="1120" />

## 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **导入** | `spaMultilingual·edit` | 打开 [导入弹窗](#导入弹窗)，从 JSON 批量写入词条 |
| **导出全部** | 无额外限制 | 下载当前全部词条为 `translate-YYYYMMDD.json` |
| **设置** | `spaMultilingual·edit` | 打开 [语言设置弹窗](#语言设置弹窗)，勾选站点启用的语言列 |

列表勾选后，工具栏出现 **导出选中项**（仅导出勾选的词条）。

<DocImage src="/cms/development/spamultilingual-toolbar.png" alt="SPA 多语言工具栏" width="1120" />

## 列表表格

表格列由 [语言设置](#语言设置弹窗) 决定：第一列语言表头带 **（默认）** 标记，对应该词条的 `defaultLang`；其余列为各语言代码（如 `en`、`zh-cn`）。

| 列 | 说明 |
|----|------|
| **名称** | 词条键名（唯一），对应 JSON 中的 key；最长 50 字符（导入时校验） |
| **各语言列** | 该 key 在该语言下的文案；空单元格表示未翻译 |
| **操作** | **编辑** 打开 [编辑词条弹窗](#编辑词条弹窗) |

支持多选 **删除**词条。

<DocImage src="/cms/development/spamultilingual-table.png" alt="SPA 多语言列表" width="1120" />

::: tip 新建词条
列表无单独「新建」按钮；**首次**或批量增加 key 请用 **导入**。导入后可对单行 **编辑** 补全各语言文案。
:::

## 语言设置弹窗

**设置** 打开。以标签形式勾选站点要启用的 **语言代码**（与 Element Plus 等 locale 代码一致，如 `en`、`zh-cn`、`zh-tw`）。

- **列表中排在首位的语言**（表格列标题带「默认」）不可在此取消勾选，即全站默认语言列。
- 保存后调用 `SetLang`：为**所有**词条统一增删语言键；新加语言值为空，需在列表或导入中补译文。

<DocImage src="/cms/development/spamultilingual-setting-dialog.png" alt="语言设置弹窗" width="1120" />

## 导入弹窗

| 区域 | 说明 |
|------|------|
| **提交方式** | **替换**：同 key 已有译文会被覆盖；**合并**：仅填充当前为空的语言字段 |
| **内容类型** | **文件**（`.json`）或 **代码**（Monaco 粘贴 JSON） |
| **示例** | 可下载 `example.json` |

JSON 结构为「词条名 → 语言代码 → 文案」：

```json
{
  "welcome": {
    "en": "Welcome",
    "zh-cn": "欢迎"
  },
  "nav.home": {
    "en": "Home",
    "zh-cn": "首页"
  }
}
```

- 每个 **key**（词条名）长度 ≤ 50。  
- 语言代码大小写会规范为小写（如 `zh-cn`）。  
- 选择 **替换** 且列表已有数据时，会二次确认以免误覆盖。

<DocImage src="/cms/development/spamultilingual-import-dialog.png" alt="导入弹窗" width="1120" />

## 编辑词条弹窗

行末 **编辑** 打开（界面标题可能显示为「设置」）。按当前启用的语言列出多行 **textarea**，修改该 key 各语言译文后保存（`SpaMultilingual/post`）。

## 前台如何使用

保存后，SPA 在运行时请求（与站点同域）：

```http
GET /_spa/lang/{语言代码}
```

例如 `GET /_spa/lang/zh-cn` 返回：

```json
{
  "welcome": "欢迎",
  "nav.home": "首页"
}
```

- 某 key 在当前语言无译文时，使用该项的 **默认语言** 文案。  
- 响应 `Content-Type: application/json`；支持 CORS 头（便于本地调试）。  
- 服务端对整表结果有约 **3 秒** 内存缓存（按站点 Id）。

接入示例（Vue i18n、react-i18next 等）见 **[SPA 多语言 API](../../api/spa-multilingual/index.md)**。

## 相关

| 文档 | 说明 |
|------|------|
| [SPA 多语言 API](../../api/spa-multilingual/index.md) | `/_spa/lang/{lang}` 响应与 fallback |
| [标签](../content/labels.md) | 模板用 `k.label()` |
| [k.label API](../../api/label/index.md) | 服务端多语言文案 |
| [开发概述](./index.md) | 开发分组索引 |
