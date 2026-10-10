# DB Guide 数据库模块

## 模块导航

根据数据类型选择：

| 用户目标 | 使用内容 |
|----------|----------|
| 使用站点内置 SQLite 存储/查询业务数据 | [SQLite Guide](sqlite.md) |

## 使用流程

1. 判断数据是否适合 `k.DB.sqlite`（非 `k.commerce`、非 `k.content`）
2. 设计表结构并初始化建表 SQL
3. 在 Service/Utils 中封装 `query/execute/transaction`
4. 在 API 层调用 Service，不直接拼接复杂 SQL

## 边界说明

- 本入口只负责 SQLite 路由与导航，不展开其他数据库类型
