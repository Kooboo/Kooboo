# 服务集成

> 菜单：**站点设置 → 服务集成**  
> 深链：`/_Admin/system/config?SiteId={站点GUID}`

**服务集成**集中管理站点的 **CoreSetting**：数据库连接、支付、短信、OAuth 登录、对象存储、JWT 等。页面按 **Group** 折叠分组；每项显示 **名称** 与 **当前值摘要**（列表中为脱敏/截断展示），点击 **编辑** 打开配置弹窗。

::: tip 权限
查看列表：`config`。编辑并保存任一集成项：`config·edit`。
:::

::: warning 安全
弹窗内常含 **密钥、连接字符串、证书** 等敏感信息。勿写入公开文档或提交到 Git；CMS 正文只说明 **后台路径与字段含义**，具体取值以厂商控制台为准。
:::

## 如何打开

1. 在 [编辑菜单](../../navigation.md#权限与编辑菜单) 中勾选 **站点设置 → 服务集成**。
2. 左侧 **站点设置 → 服务集成**。

URL 查询参数 **`group`** 可展开指定分组，例如从 [MySQL 表](../../database/mysql-table.md) 引导页跳转的 `group=Database`。

<DocImage src="/cms/settings/settings-integrations-overview.png" alt="服务集成总览" width="1120" />

## 页面结构

| 区域 | 说明 |
|------|------|
| **折叠分组** | 标题为 Group 名（如 `Database`、`Payment`）；可同时展开多个 |
| **行：名称** | 集成项 `name`，与脚本/后台 API 一致（如 `Mysql`、`Alipay`） |
| **行：值** | `value` 摘要（多为 JSON 或连接串片段），过长时省略 |
| **行：编辑** | 铅笔按钮，`data-cy` 为项名；须 `config·edit` |

<DocImage src="/cms/settings/settings-integrations-database.png" alt="Database 分组" width="1120" />

## 编辑弹窗

1. 点击某一行的 **编辑**。  
2. 顶部 **Alert**（若有）显示厂商回调地址、连接串示例等（来自服务端 `GetAlert`）。  
3. 表单字段由 `CoreSetting/GetFields` 动态生成：

| 控件类型 `type` | 界面 |
|-----------------|------|
| 默认 | 单行 **文本**（`el-input`） |
| `checkbox` | **开关** |
| `file` | **选择文件**（≤10KB，Base64 写入；可删已选文件） |

4. **确认** 调用 `CoreSetting/update`，保存后关闭弹窗并刷新列表。

<DocImage src="/cms/settings/settings-integrations-edit-dialog.png" alt="编辑集成项弹窗" width="1120" />

::: info CMS 与 API 的分工
- **本篇（CMS）**：在哪一组、点哪里、字段类型与保存方式。  
- **API 文档**：脚本调用、回调、`k.payment.*` 等。支付字段细节见各 [k.payment](../../../api/payment/index.md) 子篇，不在 CMS 重复密钥说明。
:::

## 分组与文档

| Group | 文档 | 典型集成项 |
|-------|------|------------|
| **Database** | [Database](./database.md) | Mysql、SqlServer、Mongo、Redis、Sqlite |
| **Payment** | [Payment](./payment.md) | Alipay、WeChatV3、Stripe、Paypal… |
| **SMS** | [SMS](./sms.md) | AliSMS、TencentSMSSetting、ChinaMobileSMSSetting |
| **OAuth2** | [OAuth2](./oauth2.md) | Google、Facebook、微信/微博/Apple 登录 |
| **Storage** | [Storage](./storage.md) | AmazonS3、AliyunOSS、AzureBlob |
| **Others** | [Others](./others.md) | JwtSetting、FirebaseSetting |

实际可见项随 Kooboo 版本与已安装模块略有差异，以登录后列表为准。

## 与其它菜单的关系

| 集成 | 配置后常用菜单 |
|------|----------------|
| Database · Mysql / SqlServer | [MySQL 表](../../database/mysql-table.md)、[SQL Server 表](../../database/sqlserver-table.md) |
| Payment | [电商](../../commerce/index.md)、Code 中 [k.payment](../../../api/payment/index.md) |
| OAuth2 | 前台登录回调；开发侧见 [身份验证](../../development/authentication.md)（JWT 网关规则） |
| Storage | [媒体库](../../site/media.md)、内容文件 |
| JwtSetting | [身份验证](../../development/authentication.md) 规则中的 JWT 校验 |

电商 **设置** 页不重复配置 Payment，以本页 **Payment** 分组为准。

## 典型工作流

1. 展开对应 **Group**，找到集成项。  
2. **编辑**，按 Alert 提示填写厂商参数并 **保存**。  
3. 到业务菜单验证（如绑 MySQL 后打开 MySQL 表列表、配 Alipay 后走测试支付）。  
4. 脚本侧查阅对应 API 文档。

## 相关

| 文档 | 说明 |
|------|------|
| [站点设置概述](../index.md) | 全部分组菜单 |
| [k.payment](../../../api/payment/index.md) | 支付 API |
| [k.DB](../../../api/database/index.md) | 数据库 API |
