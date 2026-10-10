# IndexedDB 表

> 菜单：**数据库 → IndexedDB 表**  
> 深链：`/_Admin/database/table?SiteId={站点GUID}`

**IndexedDB 表**是站点自带的 **动态表** 存储：在后台定义表名与字段（列）后，可在本页维护数据，并在 KScript 中通过 **`k.DB.indexedDb.{表名}`** 读写。数据按站点隔离，无需单独配置连接字符串。

::: tip 权限
列表与浏览数据：`database`（查看）。**创建表**、改列、增删改数据需 `database·edit`；多选删除表或删除数据行需 `database·delete`。
:::

::: info 与 SQLite / MySQL 等菜单的区别
| | **IndexedDB 表** | **SQLite 表** 等 |
|--|------------------|------------------|
| 存储 | 站点 IndexedDB 动态表 | 站点 SQLite 文件或外部数据库连接 |
| 建表 | 后台填表名即可 | SQLite 开箱即用；MySQL/SQL Server 须先配连接 |
| 改结构 | **列设置** 页维护字段 | 多数为已有物理表，能力以界面为准 |
| 脚本 | `k.DB.indexedDb.表名` | `k.DB.sqlite` 等 |

详见 [数据库概述](./index.md)。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **数据库 → IndexedDB 表**。
2. 左侧 **数据库 → IndexedDB 表**。

<DocImage src="/cms/database/table-overview.png" alt="IndexedDB 表列表" width="1120" />

## 表列表

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **创建表** | `database·edit` | 打开 [创建表弹窗](#创建表弹窗) |

<DocImage src="/cms/database/table-toolbar.png" alt="表列表工具栏" width="1120" />

### 列表

| 列 / 操作 | 说明 |
|-----------|------|
| **名称** | 点击表名进入 [表数据](#表数据) |
| 行末 **设置**（齿轮） | 进入 [列设置](#列设置) |
| 多选 **删除** | 删除整张表及其数据（`database·delete`） |

列表中不显示以 `_sys_`、`_koobootemp` 开头的系统内部表名。

<DocImage src="/cms/database/table-list.png" alt="表列表" width="1120" />

### 创建表弹窗

| 字段 | 规则 |
|------|------|
| **表名** | 必填；1～50 字符；字母或数字开头；仅允许字母与数字；站点内唯一 |

确认后注册表定义，即可进入 **列设置** 添加字段，或点击表名直接维护数据（空表也可先写入数据，列会随 **列设置** 保存生效）。

<DocImage src="/cms/database/table-create-dialog.png" alt="创建表弹窗" width="1120" />

## 列设置

路径：`/_Admin/database/table/columns?SiteId=...&table={表名}`

从列表 **设置** 进入，或在 **表数据** 页工具栏 **设置** 进入。页头显示当前表名。

### 工具栏

| 按钮 | 说明 |
|------|------|
| **新建列** | 打开 [列编辑弹窗](#列编辑弹窗) |

### 列列表

| 列 | 说明 |
|----|------|
| **列名** | 字段名 |
| **控件类型** | 见下表 |
| **主键** / **唯一** | 仅 IndexedDB 表显示 |
| **索引** | 是否建索引（主键列视为已索引） |
| **编辑** / **删除** | 改列定义；从列表移除列（保存后生效） |

底部 **保存** / **取消**：**保存** 将整表列定义写回站点；离开页面前若有未保存修改会提示。内部字段 **`_id`**（主键）、**`_version`** 不在列表中展示。

<DocImage src="/cms/database/table-columns.png" alt="列设置列表" width="1120" />

### 列编辑弹窗

| 字段 | 说明 |
|------|------|
| **名称** | 新建必填；保存后不可改名 |
| **控件类型** | 决定数据类型与录入控件（见下） |
| **长度** | 文本类控件可设最大长度 |
| **选项** | **下拉 / 复选 / 单选** 需配置显示名与值 |
| **自增** | 仅 **数字** 类型：勾选后按种子、步长递增；编辑数据时不可改 |
| **主键** | 业务主键标记（与系统 `_id` 并存，按表设计使用） |
| **唯一** | 列值唯一 |
| **索引** | 查询索引 |

| 控件类型（界面） | 典型用途 |
|------------------|----------|
| 文本框 | 短文本 |
| 多行文本 | 长文本 |
| 富文本 | HTML 内容 |
| 下拉 / 复选框 / 单选 | 枚举选项 |
| 布尔 | 是/否 |
| 日期时间 | 日期时间 |
| 数字 | 数值、可自增 |

<DocImage src="/cms/database/table-column-dialog.png" alt="列编辑弹窗" width="1120" />

## 表数据

路径：`/_Admin/database/table/data?SiteId=...&table={表名}`

点击列表中的 **表名** 进入。

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **新建数据** | `database·edit` | 进入 [编辑数据](#编辑数据)（新建） |
| **导出数据** | `database`（查看） | 导出当前筛选排序下的数据为 **CSV** |
| **设置** | — | 跳转 **列设置** |

::: warning 无 CSV 导入
**IndexedDB 表** 数据页 **不提供**「导入数据」按钮（该功能在 SQLite / MySQL / SQL Server 表菜单中提供）。批量写入请用脚本 `k.DB.indexedDb` 或自行开发导入逻辑。
:::

<DocImage src="/cms/database/table-data-toolbar.png" alt="表数据工具栏" width="1120" />

### 数据表格

- 列：除 **`_id`**、**`_version`** 外的业务字段；表头可 **排序**（排序状态会记住到浏览器本地）。
- 分页：默认每页 **30** 条，可翻页。
- 行末 **编辑**：进入 [编辑数据](#编辑数据)。
- 多选 **删除**：按主键删除行（`database·delete`）。

日期时间列在列表中会格式化为本地时间显示。

<DocImage src="/cms/database/table-data.png" alt="表数据列表" width="1120" />

## 编辑数据

路径：`/_Admin/database/table/edit-data?SiteId=...&table={表名}`（新建无 `id`；编辑带 `id`）

按 **列设置** 中的控件类型渲染表单项：文本、富文本、选项、布尔、日期时间、数字等。**自增** 字段仅展示当前值，不可修改。

**保存** 后返回 **表数据** 列表；**取消** 放弃并返回。

## 脚本访问（开发）

在 Code / 模块等服务端脚本中，表名作为 `k.DB.indexedDb` 的属性访问，例如：

```typescript
const row = k.DB.indexedDb.orders.find("orderNo", "A1001")
k.DB.indexedDb.orders.add({ orderNo: "A1002", amount: 99 })
```

链式 API（`find` / `findAll` / `add` / `update` / `delete` / `pagination` 等）见 **[k.DB.indexedDb](../../api/indexed-db/index.md)**。后台在本页所做的列与数据变更，与脚本读写同一张表。

::: tip SQL 日志
IndexedDB 操作 **不会** 写入 SQL 日志。仅 `k.DB.sqlite` / MySQL / SQL Server 的 SQL 在开启站点 **SQL 日志** 后可于 [SQL 日志](./sql-logs.md) 查看。
:::

## 典型工作流

1. **创建表**，填写合法表名。  
2. 打开 **设置**，**新建列** 并 **保存** 列定义。  
3. 点击表名，**新建数据** 录入或通过脚本写入。  
4. 需要关联多表时，配置 [IndexedDB 表关系](./table-relation.md)。  
5. 排障或审计时查看 [SQL 日志](./sql-logs.md)。

## 相关

| 文档 | 说明 |
|------|------|
| [数据库概述](./index.md) | 子菜单与路由 |
| [IndexedDB 表关系](./table-relation.md) | 表间关联 |
| [k.DB.indexedDb](../../api/indexed-db/index.md) | 脚本 API 全文 |
| [开发 · 代码](../development/code.md) | 编写调用表的 Code |
