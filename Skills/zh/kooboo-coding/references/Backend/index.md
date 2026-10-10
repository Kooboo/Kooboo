---
name: kooboo-coding-backend
description: Use when implementing Kooboo backend API/Service/Data layer tasks.
---

# Backend Task Guide 服务端任务入口

实现文件上传、导入、生成、替换或删除时，按 [图片与文件管理](../Frontend/images-and-files.md) 的保存与清理契约实施，优先复用已有文件服务。

## 三层架构

服务端任务分为三层，每层独立且有明确的职责：

| 层次 | 代码对象 | Name 规范 | 说明 |
|------|----------|-----------|------|
| API 层 | Code(CodeType=Api) | `product` | 定义对外接口，负责 import Service |
| Service 层 | Code(CodeType=CodeBlock) | `Services.xxx` / `Utils.xxx` | 业务逻辑，可被 API 或其他 Service import |
| Data 层 | 概念层 | — | 数据源路由自检，决定数据从哪读/往哪写 |

API 使用 `app.user` 等点分名称。每个业务模块优先对应一个包含数据访问的 `Services.xxx`，服务端只保留一个 `ENV`，内部按模块分组。复杂度足够高时再拆分，不强制 Models 层。包内 manifest 引用 `database.json`，真实使用前显式准备存储，不在请求中初始化表结构。

## 执行流程

实际可用参照、动态路由、参数处理和服务端 mock 移除规范见 [App Base 用户与通知服务](app-base-services.md)。

需要可选服务端能力时，读取[可选服务端模板](templates.md)：发现目录并按需安装归 App 所有的 CodeBlock 和 API。新 App Base 默认选择 response 和 notifications，显式选择时可跳过任意一个；已有接口仍遵循原有契约。

1. **判断层次**：先确定是 API 层还是 Service 层任务
2. **加载规范**：查阅对应的 layer guide
3. **调用 function 前**：**必须先阅读 [Backend 对象创建](../Functions/backend-create.md)**，确认 create_api / create_codeblock 的选择和参数格式
4. **自检**：用 [API 层自检清单](api-layer-checklist.md) 检查后再写入代码

::: warning 重要
**调用 create_api / create_codeblock 前必须先加载 [Backend 对象创建](../Functions/backend-create.md)**，否则可能导致 Name 格式错误或选择错误的创建函数。
:::

## API 如何调用 Service（勿臆造语法）

- **正确**：在 API 的 KScript 顶部书写 `import { getCurrentAppUser } from "./Services.user"`，在 `k.api.get/post/...` 的回调里直接调用 `getCurrentAppUser()`。Service CodeBlock 内对外的函数须使用 `export function`。
- **错误**：`k.req.services.getCurrentAppUser()`、`k.request.services.xxx()`，或任何「把 Service 挂在 request 上再按方法名调用」的写法。**Kooboo 不提供** `k.req.services` / `k.request.services` 这类 API；模型不得从其它 Web 框架迁移该习惯。
- 路由与参数请用 `k.request` / `k.api` 等文档已列 API；跨文件业务逻辑一律 **import + 调用导出函数**。详见 [API 层规范](api-layer-guide.md) 中「调用 Service」。

## 模块导航

详细规范见 references：

- [API 层规范](api-layer-guide.md) — 路由设计、参数获取、响应格式
- [Service 层规范](service-layer-guide.md) — 业务逻辑、import 模式、错误处理
- [数据层自检](data-layer-guide.md) — 数据源选择、三层协作
- [Backend 对象创建](../Functions/backend-create.md) — **create_api / create_codeblock 选择与参数**（必读）
- [API 层自检清单](api-layer-checklist.md) — 编写前自检
