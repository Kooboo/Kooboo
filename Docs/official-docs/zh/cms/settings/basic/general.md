# 基础 · 基础 Tab

> Tab：**基础** · `name=basic`  
> 深链：`/_Admin/system/settings?SiteId={站点GUID}&name=basic`

本 Tab 配置站点 **对外标识**、**预览 Base URL**、HTTPS/SPA 行为、**多语言** culture，以及 **导入 / 导出** 整站包。

<DocImage src="/cms/settings/settings-basic-general.png" alt="基础 Tab" width="1120" />

## 站点信息

| 字段 | 说明 |
|------|------|
| **显示名称** | 后台与控制面板展示的站点名（`displayName`） |
| **Base URL** | 站点预览与生成绝对链接的基准地址（`previewUrl`），须与 [域名管理](../domains.md) 中主访问域一致或互为别名 |
| **强制 SSL** | 开启后 HTTP 请求重定向 HTTPS。若前置 CDN/代理已处理 HTTPS 且出现 **过多重定向**，应关闭此项 |
| **SPA** | 开启后将 **404 重定向到主页**，适合单页应用路由由前端接管 |

## 多语言

通过 **多语言** 分组面板开关 `enableMultilingual`。关闭后保存时会收起为仅保留 **默认语言** 一条 culture，并关闭路径模式相关项。

<DocImage src="/cms/settings/settings-basic-multilingual.png" alt="多语言配置" width="1120" />

### 语言列表

| 操作 | 说明 |
|------|------|
| **添加** | 从内置 culture 列表选择，或 **输入自定义** 两位小写字母代码（如 `en`、`zh`）；非法格式会提示 |
| **显示名** | 每种语言的展示名称（如「简体中文」） |
| **删除** | 至少保留一种语言时可删 |

### 默认语言

**默认语言** 下拉仅列出当前 culture 列表中的项；删除默认语系时会自动改选列表第一项。

### 路径模式

| 模式 | 说明 |
|------|------|
| **参数模式** | 语言通过查询参数等方式传递（`enableSitePath=false`） |
| **路由模式** | URL 路径前缀区分语言（`enableSitePath=true`） |

路由模式下可配置 **默认语言前缀**：是否在默认语言 URL 上也带语言前缀。

### 其它开关

| 开关 | 说明 |
|------|------|
| **自动检测语言** | 按访客 `Accept-Language` 等自动选择 culture |
| **HrefLang** | 输出 `hreflang` 链接利于搜索引擎多语言 |

::: warning 与 SPA 多语言的区别
本页配置的是 **Kooboo 站点 culture**（页面/内容多语言、URL 规则）。[开发 → SPA 多语言](../../development/spa-multilingual.md) 管理的是 **前端 SPA 语言 JSON**，二者数据源不同，勿混用。
:::

脚本与模板侧常用 [k.label](../../../api/label/index.md) 按 culture 取文案。

## 导入 / 导出站点包

| 按钮 | 权限 | 说明 |
|------|------|------|
| **导入包** | `site·edit` | 上传 Kooboo 站点包（扩展名以产品 `importAccept` 为准），覆盖合并当前站点内容 |
| **导出站点** | `site·export` | 打开导出对话框，选择范围后下载站点包 |

导入成功后页面会 **重新加载站点** 配置。导出与 [协作同步](../../operations/sync.md) 的推送/拉取是不同机制：本处为单文件包导入导出。

## 相关

- [基础设置概述](./index.md)  
- [访问控制](./access.md)  
- [域名管理](../domains.md)
