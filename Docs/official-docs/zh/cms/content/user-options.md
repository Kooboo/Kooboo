# 参数配置

> 菜单：**内容 → 参数配置**（路由 `useroptions`；须在 **编辑菜单** 中勾选显示）  
> 深链：`/_Admin/content/useroptions?SiteId={站点GUID}`

**参数配置**用于为站点定义一组**可结构化的键值参数**：先在后台设计 **Schema**（字段名、显示名、类型），再在另一页填写 **数据**；模板与 KScript 中通过 **`k.paramConfig`** 读取，适合「站点级开关、页眉电话、社交链接对象」等不必写进 Page 源码、又希望运营在后台维护的配置。API 说明见 [k.paramConfig](../../api/param-config/index.md)。

::: tip 与站点「自定义设置」、元素属性的区别
| | **参数配置**（本页） | 站点设置中的自定义设置 | [元素属性](./tag-attributes.md) |
|--|---------------------|------------------------|--------------------------------|
| 后台 | 内容 → 参数配置 | 站点 → 设置等 | 内容 → 元素属性 |
| 数据 | 按配置名分组的 JSON | `k.site.info.setting` 等站点信息 | KConfig：HTML 属性 binding |
| 模板 | `k.paramConfig.配置名.字段` | 视站点设置而定 | `k-config="键名"` |
| 来源 | 本页新建 + 填表 | 非本菜单 | 模板标记自动生成条目 |
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **参数配置**。
2. 左侧 **内容 → 参数配置**。

<DocImage src="/cms/content/user-options-list.png" alt="参数配置列表" width="1120" />

## 列表页

| 列 / 操作 | 说明 |
|-----------|------|
| 名称 | 显示名或内部 **name**；点击进入 **编辑数据** 页（`useroptions/edit?id=...`） |
| 最后修改 | 更新时间 |
| **设置**（齿轮） | 编辑该配置的 **Schema**（字段结构）；需权限 `userOptions` · `setting` |
| 批量删除 | 勾选多行后删除（`userOptions` · `delete`） |

工具栏 **新建** 仅在有 **setting** 权限时显示，进入 **新建** 页（同时填写名称与 Schema）。

## 新建配置

路径：`/_Admin/content/useroptions/create?SiteId=...`

| 字段 | 说明 |
|------|------|
| **名称** | 内部标识，1–50 字符，须以字母或数字开头，站点内唯一；模板中用 `k.paramConfig.{名称}` |
| **显示名称** | 后台展示用，可选 |
| **自定义设置** | Schema 编辑器：为每条字段填 **名称**、**显示**、**类型** |

Schema 支持的 **类型**：文本（`string`）、数字（`number`）、布尔（`boolean`）、对象（`object`）、数组（`array`）。数组需再选元素类型；对象与「对象数组」可嵌套子字段（左侧缩进层级）。

保存后返回列表；此时 **数据** 为空对象，需在列表点名称进入 **编辑数据** 页填写。

<DocImage src="/cms/content/user-options-create.png" alt="新建参数配置：名称与 Schema" width="1120" />

## 编辑 Schema（设置页）

从列表行内 **齿轮** 进入：`/_Admin/content/useroptions/setting?id=...`

- **名称** 不可改（避免破坏模板中的 `k.paramConfig.{名称}` 引用）。
- 可改 **显示名称** 与 **Schema**（增删字段、改类型）。
- 保存 Schema 时，引擎会将该配置的 **数据重置为空对象** `{}`；若仅调整结构且需保留已有值，请先导出 JSON（见下）或在改 Schema 后重新填值。

<DocImage src="/cms/content/user-options-setting.png" alt="参数配置：编辑 Schema" width="1120" />

## 编辑数据

从列表 **名称** 链接进入：`/_Admin/content/useroptions/edit?id=...`

根据 Schema 渲染表单（文本框、数字、开关、嵌套对象、数组等）。右上角 **编辑代码** 可用 Monaco 以 **JSON** 批量编辑整份数据，确认后写回表单再保存。

保存调用 `updateOptions`，仅更新 **data**，不改 Schema。

<DocImage src="/cms/content/user-options-edit.png" alt="参数配置：按 Schema 填写数据" width="1120" />

## 模板与 KScript

读取当前站点已保存的数据（`data` 字段 JSON）：

```javascript
// 假设存在名为 siteContact 的配置，Schema 含 phone、showBanner
var phone = k.paramConfig.siteContact.phone;
var show = k.paramConfig.siteContact.showBanner;

```

- 配置 **名称** 不存在或未填数据时，对应属性为 **undefined**。
- 引擎会为各配置生成 **KDefine** 类型提示（`k.paramConfig` 下按配置名、字段名补全），便于在脚本编辑器中编写。

## 相关

- [k.paramConfig](../../api/param-config/index.md) — KScript API  
- [内容概述](./index.md)  
- [编辑菜单](../navigation.md#权限与编辑菜单)  
- [元素属性](./tag-attributes.md)（模板 `k-config`，非本页）
