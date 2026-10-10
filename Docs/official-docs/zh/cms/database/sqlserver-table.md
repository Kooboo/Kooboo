# SQL Server 表

> 菜单：**数据库 → SQL Server 表**  
> 深链：`/_Admin/database/sqlserver-table?SiteId={站点GUID}`

**SQL Server 表**用于在后台浏览与管理已接入的 **外部 SQL Server** 库中的表：创建表、维护列与数据，并支持 **CSV 导入 / 导出**。须在 [服务集成](../navigation.md) 中先配置 **SqlServer** 连接字符串；脚本侧使用 **`k.DB.sqlServer`** 执行 SQL 或访问表对象。子路由须带 **`dbType=SqlServer`**，左侧菜单高亮 **SQL Server 表**。

::: tip 权限
与 [SQLite 表](./sqlite-table.md) 相同：`database`（查看）、`database·edit`、`database·delete`。  
配置连接字符串需 **服务集成** 的 `config·edit`。
:::

::: info 与 MySQL / SQLite 的区别
| | **SQL Server 表** | **MySQL 表** | **SQLite 表** |
|--|-------------------|--------------|---------------|
| 数据源 | 外部 SQL Server | 外部 MySQL | 站点内置 SQLite |
| 服务集成项 | **Database → SqlServer** | **Database → Mysql** | 无需 |
| `dbType` | `SqlServer` | `MySql` | `Sqlite` |
| 脚本 | `k.DB.sqlServer` | `k.DB.mysql` | `k.DB.sqlite` |
| SQL 日志类型 | `sqlserver` | `mysql` | `sqlite` |

列编辑、数据浏览与 [MySQL 表](./mysql-table.md)、[SQLite 表](./sqlite-table.md) **共用同一套页面**（关系型库 UI 一致，仅 IndexedDB 列设置含主键/唯一/自增）。
:::

## 配置 SQL Server 连接

1. 打开 **站点设置 → 服务集成**：`/_Admin/system/config?SiteId={站点GUID}`  
2. 展开 **Database** 分组，编辑 **SqlServer**。详见 [服务集成 · Database](../settings/integrations/database.md)。  
3. 填写 **连接字符串**（ConnectionString），保存。

示例格式（以 Kooboo 内置说明为准）：

```text
Data Source=127.0.0.1;Initial Catalog=mydb;User ID=sa;Pwd=123
```

也可从 **SQL Server 表** 未配置引导页点击 **立即配置**，跳转到 `/_Admin/system/config?SiteId={站点GUID}&group=Database`。

::: warning 安全
连接字符串含账号密码，请勿提交到公开仓库；生产环境使用最小权限数据库用户。Kooboo 连接时会自动处理常见 TLS 证书选项（如 `TrustServerCertificate`），具体以部署环境驱动为准。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **数据库 → SQL Server 表**。  
2. 完成 [配置 SQL Server 连接](#配置-sql-server-连接)。  
3. 左侧 **数据库 → SQL Server 表**。

未配置或连接无效时，页面显示错误说明与 **立即配置** 按钮，不展示表列表。

<DocImage src="/cms/database/sqlserver-table-unconfigured.png" alt="SQL Server 未配置引导" width="1120" />

配置成功并连通后，列表、列、数据、CSV 导入等界面与 [SQLite 表](./sqlite-table.md) **共用同一套页面**（仅 `dbType=SqlServer`、菜单高亮不同）。下列配图以 SQLite 为例，SQL Server 表配置完成后外观一致。

## 表列表

| 操作 | 权限 | 说明 |
|------|------|------|
| **创建表** | `database·edit` | 在 SQL Server 中建表并登记后台 Schema |
| 点击 **名称** | `database` | 进入 [表数据](#表数据) |
| **设置**（齿轮） | `database` | 进入 [列设置](#列设置) |
| 多选 **删除** | `database·delete` | 删除选中物理表（谨慎） |

表名规则与 IndexedDB / SQLite 相同：1～50 字符、字母或数字开头、仅字母数字。

<DocImage src="/cms/database/sqlite-table-overview.png" alt="表列表（与 SQLite 表界面相同）" width="1120" />

<DocImage src="/cms/database/sqlite-table-toolbar.png" alt="创建表工具栏" width="1120" />

<DocImage src="/cms/database/sqlite-table-list.png" alt="表名列表示例" width="1120" />

### 创建表弹窗

<DocImage src="/cms/database/sqlite-table-create-dialog.png" alt="创建表弹窗" width="1120" />

## 列设置

路径示例：

```text
/_Admin/database/table/columns?SiteId={站点GUID}&table={表名}&dbType=SqlServer
```

与 [SQLite 列设置](./sqlite-table.md#列设置) 相同：列表显示 **列名**、**控件类型**、**索引**；列编辑弹窗无 **长度**、**自增**、**主键/唯一**（仅 IndexedDB 有）。保存会更新 SQL Server 表结构及后台控件映射。

<DocImage src="/cms/database/sqlite-table-columns.png" alt="列设置（与 SQLite 表界面相同）" width="1120" />

<DocImage src="/cms/database/sqlite-table-column-dialog.png" alt="列编辑弹窗" width="1120" />

## 表数据

路径示例：

```text
/_Admin/database/table/data?SiteId={站点GUID}&table={表名}&dbType=SqlServer
```

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **新建数据** | `database·edit` | 进入编辑页 |
| **导出数据** | `database` | 导出 CSV |
| **导入数据** | `database·edit` | [CSV 导入](#csv-导入) |
| **设置** | — | 跳转列设置 |

<DocImage src="/cms/database/sqlite-table-data-toolbar.png" alt="表数据工具栏" width="1120" />

<DocImage src="/cms/database/sqlite-table-data.png" alt="表数据列表" width="1120" />

### CSV 导入

字段与流程同 [SQLite CSV 导入](./sqlite-table.md#csv-导入)：跳过行数、上传 `.csv`、字段映射（必填/唯一）、可选覆盖。

<DocImage src="/cms/database/sqlite-table-import-dialog.png" alt="CSV 导入弹窗" width="1120" />

## 编辑数据

路径示例：

```text
/_Admin/database/table/edit-data?SiteId={站点GUID}&table={表名}&dbType=SqlServer
```

按列 **控件类型** 渲染表单；**保存** / **取消** 返回数据列表。

## 脚本访问（开发）

```typescript
const rows = k.DB.sqlServer.query(
  "SELECT * FROM orders WHERE id = @id",
  { id: 1 }
)
k.DB.sqlServer.execute(
  "INSERT INTO orders (order_no, amount) VALUES (@no, @amt)",
  { no: "A1001", amt: 99 }
)
const table = k.DB.sqlServer.getTable("orders")
```

在 **站点设置 → 高级** 将 **默认数据库** 设为 **SqlServer** 时，`k.DB.sql` 与 `k.DB.sqlServer` 指向同一实例。

详见 **[k.DB](../../api/database/index.md)**（`sqlServer` 与 `mysql` / `sqlite` 同为关系型接口：`query` / `execute` / `getTable` / `procedure` 等）。

::: tip SQL 日志
开启站点 **SQL 日志** 后，`k.DB.sqlServer` 执行的语句可在 [SQL 日志](./sql-logs.md) 中按类型 **SQLServer** 筛选查看。
:::

## 典型工作流

1. 在 **服务集成** 配置 **SqlServer** 连接并验证连通。  
2. 打开 **SQL Server 表**，**创建表** 或管理库中已有表。  
3. **设置** 列与控件类型，**新建数据** 或 **导入 CSV**。  
4. Code 中用 `k.DB.sqlServer` 读写；排障时查 [SQL 日志](./sql-logs.md)。

## 相关

| 文档 | 说明 |
|------|------|
| [MySQL 表](./mysql-table.md) | 同类外部 MySQL |
| [SQLite 表](./sqlite-table.md) | 内置库、无需连接 |
| [k.DB](../../api/database/index.md) | 脚本 API |
| [SQL 日志](./sql-logs.md) | 执行记录 |
| [数据库概述](./index.md) | 菜单总览 |
