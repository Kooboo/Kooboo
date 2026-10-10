# 域名管理

> 菜单：**站点设置 → 域名管理**  
> 深链：`/_Admin/system/domains?SiteId={站点GUID}`

**域名管理**为当前站点配置 **访问绑定（Binding）**：将子域名或端口映射到本站，并可设置 **重定向**、**语言 culture** 与 **SSL**。列表中的主域通常与 [基础设置 · Base URL](./basic/general.md#站点信息) 一致；新增绑定后站点会重新加载绑定信息。

::: tip 权限
| 操作 | 权限 |
|------|------|
| 查看列表 | `domain` |
| **新建绑定**、启用 **SSL**、工具栏 **强制 SSL** | `domain·edit` |
| 多选 **删除** | `domain·delete` |

根域名来自组织/服务器已登记的 **可用域名**（`getAvailableDomain`）；本地/私有环境还可绑定 **端口**。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **站点设置 → 域名管理**。  
2. 左侧 **站点设置 → 域名管理**。

<DocImage src="/cms/settings/settings-domains-overview.png" alt="域名管理" width="1120" />

## 工具栏

| 控件 | 说明 |
|------|------|
| **新建绑定** | 打开 [新建绑定弹窗](#新建绑定) |
| **强制 SSL** | 站点级 `forceSSL`，切换后 **立即保存** 站点（与 [基础设置](./basic/general.md#站点信息) 中项相同）。CDN/代理场景下若出现重定向循环应关闭 |

<DocImage src="/cms/settings/settings-domains-toolbar.png" alt="域名管理工具栏" width="1120" />

## 绑定列表

| 列 | 说明 |
|----|------|
| **域名或端口** | 域名绑定显示 `fullName`（子域 + 根域）；**默认端口绑定** 显示端口号 |
| **重定向到** | 访问本绑定时 301/302 到的另一绑定域名（可为空） |
| **语言** | 绑定默认 culture（须在 [基础设置 · 多语言](./basic/general.md#多语言) 中已配置） |
| **SSL 已启用** | 域名绑定可开启 HTTPS 证书；已开启后开关 **禁用**（不可在后台关闭，仅可删除绑定） |

多选可 **删除** 绑定（`Binding/Deletes`）。

<DocImage src="/cms/settings/settings-domains-list.png" alt="域名绑定列表" width="1120" />

### 启用 SSL

对 **非端口** 绑定，在 **SSL 已启用** 列打开开关：

1. 调用 `Binding/verifySSL` 校验域名与证书环境  
2. 成功后 `Binding/SetSsl` 写入并刷新列表  

失败时行旁显示 **失败** 标签，悬停查看 `sslError` 详情。须 `domain·edit` 权限。

## 新建绑定

<DocImage src="/cms/settings/settings-domains-new-binding.png" alt="新建绑定弹窗" width="1120" />

### 绑定类型

| 类型 | 环境 | 说明 |
|------|------|------|
| **域名** | 全部 | 子域名 + 根域名下拉（来自可用域名列表） |
| **端口** | **非在线公有云**（`!isOnlineServer`） | 指定端口号绑定本站（`defaultBinding`） |

在线服务器环境弹窗中 **不显示**「绑定到」类型切换，仅域名模式。

### 域名字段

| 字段 | 规则 |
|------|------|
| **子域名** | 可选；须符合子域规则（长度 1～63、站点内唯一等） |
| **根域名** | 必选；下拉为组织可用根域，前缀 `.` 或 `-` 表示该根域与子域的连接方式（`sudDomainUseDash`） |

完整访问名为子域与根域组合后的 `fullName`。

### 高级设置

点击弹窗中部 **展开箭头** 显示：

| 字段 | 说明 |
|------|------|
| **重定向到** | 从已有绑定中选择目标 `fullName` |
| **语言** | 从站点 `culture` 字典选择 |

<DocImage src="/cms/settings/settings-domains-new-binding-advanced.png" alt="新建绑定高级设置" width="1120" />

确认后 `Binding/post` 创建；成功关闭弹窗并刷新列表与站点信息。

## 与基础设置的关系

| 项 | 关系 |
|----|------|
| **Base URL** | 应指向访客实际访问的主绑定域名（含协议） |
| **强制 SSL** | 本页与基础设置共享 `site.forceSSL`，两处修改等价 |
| **多语言** | 绑定的 **语言** 列使用基础设置中的 culture 列表 |

## 典型工作流

1. 在组织层确保根域名已登记可用。  
2. **新建绑定** 填写子域 + 根域（或本地环境绑端口）。  
3. 按需设 **重定向**、**语言**。  
4. DNS 指向 Kooboo 服务器后，在列表中 **启用 SSL**。  
5. 将 [基础设置](./basic/general.md) 的 **Base URL** 更新为主访问地址。

## 相关

| 文档 | 说明 |
|------|------|
| [站点设置概述](./index.md) | 菜单总览 |
| [基础设置](./basic/) | Base URL、多语言 |
| [服务集成](./integrations/) | 与域名无关的第三方配置 |
