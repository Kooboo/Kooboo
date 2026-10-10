# Backend 对象创建

创建前先检查同名对象；已有对象使用 [读取与更新](read-update.md)。

内置公共工具使用 `install_backend_templates` 按需引入，无需重新编写其源码。选择、App Base 默认行为及冲突处理见[可选服务端模板](../Backend/templates.md)。

## `create_api`

| 字段 | 说明 |
|---|---|
| `name` | 简洁小写 API 标识 |
| `url` | 以 `/` 开头的 API 路由 |
| `content` | KScript 内容 |

## `create_codeblock`

| 字段 | 说明 |
|---|---|
| `name` | `Services.xxx`、`Utils.xxx` 等模块名称 |
| `content` | KScript 内容 |

Service 必须使用 CodeBlock，不能用 API 代替。API 调用 Service 时，在文件顶部使用 `import { fn } from "./Services.xxx"` 或 `"./Utils.xxx"`，对应函数必须 `export`。

实现前继续读取 [Backend 入口](../Backend/index.md) 及实际使用的 KScript 模块；不要从 request 对象臆造 Service 调用方式。创建后读回并按当前阶段执行 Preflight 与运行验证。
