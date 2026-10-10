# MySQL 表

> 菜单：**数据库 → MySQL 表**  
> 深链：`/_Admin/database/mysql-table?SiteId={站点GUID}`

**MySQL 表**用于在后台浏览与管理已接入的 **外部 MySQL** 库中的表：创建表、维护列与数据，并支持 **CSV 导入 / 导出**。须在 [服务集成](../navigation.md) 中先配置 **Mysql** 连接字符串；脚本侧使用 **`k.DB.mysql`** 执行 SQL 或访问表对象。子路由须带 **`dbType=MySql`**，左侧菜单高亮 **MySQL 表**。

::: tip 权限
与 [SQLite 表](./sqlite-table.md) 相同：`database`（查看）、`database·edit`、`database·delete`。  
配置连接字符串需 **服务集成** 的 `config·edit`。
:::

::: info 与 SQLite 表的区别
| | **MySQL 表** | **SQLite 表** |
|--|--------------|---------------|
| 数据源 | 外部 MySQL（连接字符串） | 站点内置 SQLite 文件 |
| 前置条件 | [服务集成](#配置-mysql-连接) **Database → Mysql** | 无需配置 |
| 未配置时 | 引导页 + **立即配置** | 直接列出表 |
| 列 / 数据 UI | 与 SQLite 相同（无主键/唯一列、有 CSV） | 同左 |
| 脚本 | `k.DB.mysql` | `k.DB.sqlite` |
| SQL 日志类型 | `mysql` | `sqlite` |

列编辑、数据浏览与 IndexedDB / SQLite 共用 `views/database/table/*` 路由。
:::

## 配置 MySQL 连接

1. 打开 **站点设置 → 服务集成**：`/_Admin/system/config?SiteId={站点GUID}`  
2. 展开 **Database** 分组，编辑 **Mysql**。详见 [服务集成 · Database](../settings/integrations/database.md)。
3. 填写 **连接字符串**（ConnectionString），保存。

示例格式（以 Kooboo 内置说明为准）：

```text
Server=myServerAddress;Port=3306;Database=myDataBase;Uid=myUsername;Pwd=myPassword;
```

也可从 **MySQL 表** 未配置引导页点击 **立即配置**，跳转到 `/_Admin/system/config?SiteId={站点GUID}&group=Database`（自动展开 Database 分组）。

::: warning 安全
连接字符串含账号密码，请勿提交到公开仓库；生产环境使用最小权限数据库用户。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **数据库 → MySQL 表**。  
2. 完成 [配置 MySQL 连接](#配置-mysql-连接)。  
3. 左侧 **数据库 → MySQL 表**。

未配置或连接无效时，页面显示错误说明与 **立即配置** 按钮，不展示表列表。

<DocImage src="/cms/database/mysql-table-unconfigured.png" alt="MySQL 未配置引导" width="1120" />

配置成功并连通后，列表、列、数据、CSV 导入等界面与 [SQLite 表](./sqlite-table.md) **共用同一套页面**（仅 `dbType=MySql`、菜单高亮不同）。下列配图以 SQLite 为例，MySQL 表配置完成后外观一致。

## 表列表

| 操作 | 权限 | 说明 |
|------|------|------|
| **创建表** | `database·edit` | 在 MySQL 中建表并登记后台 Schema |
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
/_Admin/database/table/columns?SiteId={站点GUID}&table={表名}&dbType=MySql
```

与 [SQLite 列设置](./sqlite-table.md#列设置) 相同：列表显示 **列名**、**控件类型**、**索引**（无主键/唯一列）；列编辑弹窗无 **长度**、**自增**、**主键/唯一**（仅 IndexedDB 有）。保存会更新 MySQL 表结构及后台控件映射。

<DocImage src="/cms/database/sqlite-table-columns.png" alt="列设置（与 SQLite 表界面相同）" width="1120" />

<DocImage src="/cms/database/sqlite-table-column-dialog.png" alt="列编辑弹窗" width="1120" />

## 表数据

路径示例：

```text
/_Admin/database/table/data?SiteId={站点GUID}&table={表名}&dbType=MySql
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
/_Admin/database/table/edit-data?SiteId={站点GUID}&table={表名}&dbType=MySql
```

按列 **控件类型** 渲染表单；**保存** / **取消** 返回数据列表。

## 脚本访问（开发）

```typescript
const rows = k.DB.mysql.query(
  "SELECT * FROM orders WHERE id = @id",
  { id: 1 }
)
k.DB.mysql.execute(
  "INSERT INTO orders (order_no, amount) VALUES (@no, @amt)",
  { no: "A1001", amt: 99 }
)
const table = k.DB.mysql.getTable("orders")
```

在 **站点设置 → 高级** 将 **默认数据库** 设为 **Mysql** 时，`k.DB.sql` 与 `k.DB.mysql` 指向同一实例。

详见 **[k.DB](../../api/database/index.md)**（`mysql` 与 `sqlite` 同为关系型接口：`query` / `execute` / `getTable` 等）。

::: tip SQL 日志
开启站点 **SQL 日志** 后，`k.DB.mysql` 执行的语句可在 [SQL 日志](./sql-logs.md) 中按类型 **MySQL** 筛选查看。
:::

## 典型工作流

1. 在 **服务集成** 配置 **Mysql** 连接并验证连通。  
2. 打开 **MySQL 表**，**创建表** 或同步已有库表（库中已存在的表会出现在列表）。  
3. **设置** 列与控件类型，**新建数据** 或 **导入 CSV**。  
4. Code 中用 `k.DB.mysql` 读写；排障时查 [SQL 日志](./sql-logs.md)。

## 相关

| 文档 | 说明 |
|------|------|
| [SQLite 表](./sqlite-table.md) | 内置库、无需连接 |
| [SQL Server 表](./sqlserver-table.md) | 同类外部库 |
| [k.DB](../../api/database/index.md) | 脚本 API |
| [SQL 日志](./sql-logs.md) | 执行记录 |
| [数据库概述](./index.md) | 菜单总览 |
