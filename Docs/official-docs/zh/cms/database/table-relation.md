# IndexedDB 表关系

> 菜单：**数据库 → IndexedDB 表关系**  
> 深链：`/_Admin/database/table-relation?SiteId={站点GUID}`

**表关系**用于声明两张 [IndexedDB 表](./table.md) 之间的关联：指定 **表 A / 字段 A**、**关系类型**、**表 B / 字段 B**，并赋予关系一个 **名称**。保存后，在 KScript 中读取表 A 某行时，可通过 **关系名称**（或部分场景下的关联表名）访问关联的表 B 数据。

::: tip 权限
列表与查看：`tableRelation`（查看）。**新建** 关系需 `tableRelation·edit`；多选 **删除** 需 `tableRelation·delete`。
:::

::: info 前置条件
两张表须已在 **IndexedDB 表** 中创建，且相关字段已存在于 **列设置** 中。下拉框中的表与字段来自当前站点 IndexedDB 表结构。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **数据库 → IndexedDB 表关系**。
2. 左侧 **数据库 → IndexedDB 表关系**。

<DocImage src="/cms/database/table-relation-overview.png" alt="表关系列表" width="1120" />

## 关系列表

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **新建** | `tableRelation·edit` | 打开 [新建关系弹窗](#新建关系弹窗) |

<DocImage src="/cms/database/table-relation-toolbar.png" alt="表关系工具栏" width="1120" />

### 列表表格

| 列 | 说明 |
|----|------|
| **名称** | 关系唯一名；脚本中常作为当前表记录上的 **属性名** 访问关联数据 |
| **表 A** / **字段 A** | 关系一侧的表与字段 |
| **关系** | 关系类型（一对一、一对多、多对多、多对一等，界面文案由系统本地化） |
| **表 B** / **字段 B** | 另一侧的表与字段 |

支持多选 **删除**。当前界面 **不提供行内编辑**；需修改时请删除后重新 **新建**（或通过 `TableRelation` API 更新）。

<DocImage src="/cms/database/table-relation-list.png" alt="表关系列表表格" width="1120" />

## 新建关系弹窗

| 字段 | 说明 |
|------|------|
| **名称** | 必填；1～50 字符；站点内唯一（冲突时提示关系名已存在） |
| **表 A** | 下拉选择 IndexedDB 表 |
| **字段 A** | 选定表 A 后的字段列表 |
| **关系** | 关系类型，可选：**OneOne**、**OneMany**、**ManyMany**、**ManyOne**（界面显示为对应中文/英文标签） |
| **表 B** | 另一张 IndexedDB 表 |
| **字段 B** | 表 B 上的关联字段 |

保存后调用 `TableRelation/post` 写入站点库，列表刷新。

<DocImage src="/cms/database/table-relation-create-dialog.png" alt="新建表关系弹窗" width="1120" />

### 关系类型含义（使用上）

| 类型 | 典型含义 | 脚本侧展开结果（从表 A 行访问时） |
|------|----------|----------------------------------|
| **一对一 / 多对一** | A 的一条记录对应 B 的一条 | 单个关联对象 |
| **一对多 / 多对多** | A 的一条记录对应 B 的多条 | 关联对象 **数组** |

实际联接以 **字段 A** 与 **字段 B** 的取值相等为准（例如 `users.id` 对应 `orders.userId`）。

## 在脚本中的效果

关系 **不** 单独提供 `k.*` API，而是在查询 [IndexedDB 表](./table.md) 得到动态对象后，用 **关系名称** 取关联数据：

```typescript
const user = k.DB.indexedDb.users.get(userId)
// 假定存在名为 userOrders 的关系：users.id → orders.userId，类型为一对多
const orders = user.userOrders
```

- 访问名通常与后台填写的 **名称** 一致。  
- 一对多 / 多对多时返回数组；一对一 / 多对一时返回单条对象。  
- 更多 CRUD 见 [k.DB.indexedDb](../../api/indexed-db/index.md)。

## 典型工作流

1. 在 **IndexedDB 表** 中准备好 **表 A**、**表 B** 及关联字段。  
2. 在本页 **新建** 关系，填齐名称、两端表字段与类型。  
3. 在 Code 中 `get` / `find` 主表记录后，通过 **名称** 读取关联集合。  
4. 关系定义变更后，若脚本仍用旧名称，需同步修改代码或删除重建关系。

## 相关

| 文档 | 说明 |
|------|------|
| [IndexedDB 表](./table.md) | 建表与列、数据维护 |
| [k.DB.indexedDb](../../api/indexed-db/index.md) | 动态对象与查询 |
| [数据库概述](./index.md) | 数据库分组菜单 |
