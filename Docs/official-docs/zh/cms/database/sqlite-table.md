# SQLite 表

> 菜单：**数据库 → SQLite 表**  
> 深链：`/_Admin/database/sqlite-table?SiteId={站点GUID}`

**SQLite 表**管理当前站点内置 **SQLite 数据库**中的表：可在后台 **创建表**、维护列与数据，并支持 **CSV 导入 / 导出**。无需单独配置连接字符串；脚本侧使用 **`k.DB.sqlite`** 执行 SQL 或访问表对象。

::: tip 权限
与 [IndexedDB 表](./table.md) 相同：`database`（查看）、`database·edit`、`database·delete`。
:::

::: info 与 IndexedDB 表的区别
| | **SQLite 表** | **IndexedDB 表** |
|--|---------------|------------------|
| 引擎 | 站点 SQLite 文件 | 站点 IndexedDB 动态表 |
| 配置 | 开箱即用 | 开箱即用 |
| 列设置 | 无 **主键 / 唯一** 列（界面）、无数字 **自增** 项 | 可设主键、唯一、自增 |
| 数据页 | 有 **导入数据**（CSV） | 无 CSV 导入 |
| 脚本 | `k.DB.sqlite.query` / `execute` / `getTable` | `k.DB.indexedDb.{表名}` |
| 表关系 | 不支持 [表关系](./table-relation.md)（仅 IndexedDB） | 支持 |

列编辑、数据浏览与 IndexedDB 共用路由，但须带查询参数 **`dbType=Sqlite`**，左侧菜单高亮 **SQLite 表**。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **数据库 → SQLite 表**。
2. 左侧 **数据库 → SQLite 表**。

<DocImage src="/cms/database/sqlite-table-overview.png" alt="SQLite 表列表" width="1120" />

## 表列表

界面与 [IndexedDB 表](./table.md#表列表) 相同：**创建表**、点击 **名称** 进数据、**设置** 进列定义、多选删除。

<DocImage src="/cms/database/sqlite-table-toolbar.png" alt="SQLite 表工具栏" width="1120" />

<DocImage src="/cms/database/sqlite-table-list.png" alt="SQLite 表列表" width="1120" />

### 创建表弹窗

表名规则与 IndexedDB 相同：1～50 字符、字母或数字开头、仅字母数字、站点内唯一。创建后在 SQLite 中生成物理表，并登记后台列结构（Schema）。

<DocImage src="/cms/database/sqlite-table-create-dialog.png" alt="创建 SQLite 表弹窗" width="1120" />

## 列设置

路径示例：

```text
/_Admin/database/table/columns?SiteId={站点GUID}&table={表名}&dbType=Sqlite
```

从 SQLite 列表进入时，地址栏会自动带上 `dbType=Sqlite`。

| 与 IndexedDB 列设置差异 | 说明 |
|-------------------------|------|
| 列表列 | 显示 **列名**、**控件类型**、**索引**；**不显示**主键 / 唯一列 |
| 列编辑弹窗 | 无 **长度**（文本）、无 **自增**、无 **主键 / 唯一** 勾选项 |
| 保存 | 更新 SQLite 表结构及后台 Schema 映射 |

控件类型（文本框、多行文本、富文本、选项、布尔、日期时间、数字等）与 IndexedDB 一致，决定后台 **编辑数据** 表单样式。

<DocImage src="/cms/database/sqlite-table-columns.png" alt="SQLite 列设置" width="1120" />

<DocImage src="/cms/database/sqlite-table-column-dialog.png" alt="SQLite 列编辑弹窗" width="1120" />

## 表数据

路径示例：

```text
/_Admin/database/table/data?SiteId={站点GUID}&table={表名}&dbType=Sqlite
```

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **新建数据** | `database·edit` | 进入编辑页新建行 |
| **导出数据** | `database`（查看） | 导出 CSV（当前排序下；大量数据时导出会使用较大分页抓取） |
| **导入数据** | `database·edit` | 打开 [CSV 导入](#csv-导入) |
| **设置** | — | 跳转列设置 |

<DocImage src="/cms/database/sqlite-table-data-toolbar.png" alt="SQLite 表数据工具栏" width="1120" />

### 数据表格

分页默认 **30** 条/页，列头可排序（排序偏好保存在浏览器本地），行末 **编辑**、多选 **删除**。列表隐藏 **`_id`** 等内部列时的展示规则与 IndexedDB 数据页一致。

<DocImage src="/cms/database/sqlite-table-data.png" alt="SQLite 表数据" width="1120" />

### CSV 导入

| 步骤 / 字段 | 说明 |
|-------------|------|
| **跳过行数** | 文件开头跳过的行数（如 CSV 表头占 1 行则填 `1`） |
| **文件** | 选择 **`.csv`** 上传，服务端解析列与样例行 |
| **字段映射** | 将 **数据库字段** 映射到 **CSV 列**；可勾选 **必填**、**唯一** |
| **覆盖** | 当存在 **唯一** 映射且命中已有行时，可开启覆盖更新 |

确认 **开始** 后批量写入；完成后刷新数据表。

<DocImage src="/cms/database/sqlite-table-import-dialog.png" alt="SQLite CSV 导入" width="1120" />

## 编辑数据

路径示例：

```text
/_Admin/database/table/edit-data?SiteId={站点GUID}&table={表名}&dbType=Sqlite
```

按列 **控件类型** 渲染表单；**保存** / **取消** 后返回表数据列表。新建时不带 `id` 查询参数。

## 脚本访问（开发）

SQLite 以 **SQL** 与 **表对象** 为主，例如：

```typescript
k.DB.sqlite.execute(
  "CREATE TABLE IF NOT EXISTS logs (id INTEGER PRIMARY KEY, msg TEXT)"
)
const rows = k.DB.sqlite.query("SELECT * FROM logs WHERE id = @id", { id: 1 })
const table = k.DB.sqlite.getTable("logs")
```

详见 **[k.DB.sqlite](../../api/sqlite/index.md)**。在 Code 中 `execute` / `query` 创建的表，刷新 **SQLite 表** 列表后也可在后台维护（须符合表名规则）。

::: tip SQL 日志
执行 SQL 产生的记录可在 [SQL 日志](./sql-logs.md) 中查看（需站点开启相关日志）。
:::

## 典型工作流

1. **创建表** 或在脚本中 `CREATE TABLE` 后于本页 **同步** 看到表名。  
2. **设置** 中定义列（或依赖脚本建表时的 SQL 列）。  
3. **新建数据**、**导入 CSV** 或脚本 `INSERT` 写入。  
4. 排障时结合 [SQL 日志](./sql-logs.md) 与 `k.DB.sqlite.query`。

## 相关

| 文档 | 说明 |
|------|------|
| [IndexedDB 表](./table.md) | 动态表与对象 API |
| [k.DB.sqlite](../../api/sqlite/index.md) | SQL 与 `getTable` |
| [SQL 日志](./sql-logs.md) | 执行记录 |
| [数据库概述](./index.md) | 菜单总览 |
