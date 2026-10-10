# 键值存储

> 菜单：**数据库 → 键值存储**  
> 深链：`/_Admin/database/key-value?SiteId={站点GUID}`

**键值存储**提供站点级简单的 **键 / 值** 对：适合缓存标记、轻量配置、临时状态等不需要建表结构的场景。值以字符串保存；脚本通过 **`k.DB.keyValue`** 读写（与后台同一数据源）。

::: tip 权限
列表与查看：`keyValue`（查看）。**新建**、编辑值需 `keyValue·edit`；多选 **删除** 需 `keyValue·delete`。
:::

::: info 与 IndexedDB 表的区别
| | **键值存储** | **IndexedDB 表** |
|--|--------------|------------------|
| 模型 | 扁平 key → string value | 多列、类型、索引 |
| 后台 | 本页键值列表 | [IndexedDB 表](./table.md) |
| 脚本 | `k.DB.keyValue.set` / `get` | `k.DB.indexedDb.{表名}` |

需要结构化查询或关联时，请用 IndexedDB 表；仅需少量字符串配置时用键值存储即可。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **数据库 → 键值存储**。
2. 左侧 **数据库 → 键值存储**。

<DocImage src="/cms/database/key-value-overview.png" alt="键值存储列表" width="1120" />

## 键值列表

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **新建** | `keyValue·edit` | 打开 [新建/编辑弹窗](#新建与编辑弹窗)（空键） |

<DocImage src="/cms/database/key-value-toolbar.png" alt="键值存储工具栏" width="1120" />

### 列表表格

| 列 | 说明 |
|----|------|
| **键** | 唯一标识；列表按键名排序 |
| **值** | 当前保存的字符串（列表中为摘要展示） |
| 行末 **编辑** | 打开弹窗修改 **值**（键名不可改） |

支持多选 **删除**（按 **键** 删除）。

<DocImage src="/cms/database/key-value-list.png" alt="键值列表" width="1120" />

## 新建与编辑弹窗

| 字段 | 说明 |
|------|------|
| **键** | 新建必填；须 **字母或数字开头**；站点内唯一（占用时提示键已被使用）。**编辑** 时键名禁用，仅可改值 |
| **值** | 必填；多行文本。单条值的 UTF-8 长度上限约 **4096 字节**（超出保存会失败） |

保存调用 `KeyValue/Update`（新建与更新同一接口）。新建成功后列表刷新。

<DocImage src="/cms/database/key-value-edit-dialog.png" alt="新建或编辑键值弹窗" width="1120" />

## 脚本访问（开发）

```typescript
k.DB.keyValue.set("lastSync", "2026-05-27T12:00:00Z")
const v = k.DB.keyValue.get("lastSync")
// 或属性访问（与 get 等价）
const v2 = k.DB.keyValue.lastSync
```

也可使用顶层 **`k.keyValue`**（与 `k.DB.keyValue` 相同入口）。详见 [k.DB.keyValue](../../api/key-value/index.md)。

::: warning 非内容「参数配置」
站点 **内容 → 参数配置**（UserOptions）是另一套结构化 JSON 配置，对应 `k.paramConfig` 等 API。键值存储与之独立，勿混用菜单。
:::

## 典型工作流

1. **新建** 键值，填写键名与初始值。  
2. 在 Code / 定时任务中 `get` / `set` 读写。  
3. 需要调整时列表 **编辑** 或脚本 `set` 覆盖。  
4. 不再使用时多选 **删除**。

## 相关

| 文档 | 说明 |
|------|------|
| [数据库概述](./index.md) | 数据库分组 |
| [k.DB.keyValue](../../api/key-value/index.md) | 脚本 API |
| [IndexedDB 表](./table.md) | 结构化数据 |
| [参数配置](../content/user-options.md) | 结构化站点参数（不同功能） |
