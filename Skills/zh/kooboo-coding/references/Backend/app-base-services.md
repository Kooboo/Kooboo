# App Base 用户与通知服务

以已安装的 App Base 代码作为业务 API 的实际参照，扩展前按需读取相关 API 和 Service。这些是实际基础功能，不另存 example 副本。

## 职责边界

App Base 负责启动、身份、用户数据和能力接入；可选服务端模板拥有自己的 API、业务 Service 和数据库定义；服务端配置统一放在 ENV。每项能力只保留一份实现，App Base 负责选择安装并接入，不另存副本。

- `assets/app-base/API`、`CodeBlock`：用户、身份 API 和 Service；`assets/app-base/database.json` 定义 AppUsers。
- `assets/backend-templates/notifications/files`：通知 API 和一个包含当前接收人解析的 Service；模板的 `database.json` 定义 AppNotifications。
- `assets/app-base/Script/service.notification.js`：由 `appENV.notificationsEnabled` 控制的前端接入。
- `/service.bootstrap.js`：启动组装；`/service.auth.js` 仅负责认证和用户操作。

首次创建默认安装 response 和 notifications。`backendTemplates: []` 跳过两者，用户 API 使用局部响应函数，启动不请求通知。后续安装不会修改前端配置，接入和移除步骤见[通知模板](../../assets/backend-templates/notifications/README.md)。

## 对象组织与路由

| 对象 | 路由 / 职责 |
|---|---|
| API `app.user` | `/api/app/user/{action}`：基础路径 GET 当前用户、PUT 修改自己的显示名称；POST `logout` |
| API `app.notifications` | `/api/app/notifications/{action}`：基础路径 GET 列表；GET `unread-count`；PUT `mark-read`、`mark-all-read` |
| `ENV` | 唯一服务端配置：身份模式、语言和通知设置；源码位于 assets/backend-templates/ENV.ts |
| `Services.user` | 同步账号身份，读取和更新 App 自有用户资料 |
| `Services.notification` | 可选通知模板：按接收人限定的操作 |
| 包内 `database.json` | manifest 引用的初始表结构定义，不安装为运行时代码 |

`assets/app-base/API`、`CodeBlock` 是模板源码目录；安装后的名称是 Kooboo 代码对象，不是文件系统路径。使用 `./Services.user`、`./ENV` 等 import。按业务域组织 API；动态 `{action}` 路由的基础路径读写使用 `k.api.get("/", ...)`、`k.api.put("/", ...)`，不要绑定或检查查询参数 `action` 来判断路径。有独立含义的操作使用具名回调；最后只加 `k.api.get(() => k.api.notFound())` 兜底未知 GET 路径，POST/PUT/DELETE 不加通配兜底。

API 对象名和文件名使用点分隔，例如 `app.user` / `app.user.ts`。一个业务模块优先对应一个 `Services.xxx`，包含参数化数据访问；复杂度足够高时再拆分。不强制 Models 层，不在请求处理中初始化表。服务端统一使用一个 `ENV`，内部按模块分组配置。

API 负责 query/body 读取、解析、校验及 HTTP 响应。Service 接收经过校验的普通参数和可信用户上下文，不读取请求参数，不写 HTTP 响应。当前身份只能来自服务端，不能相信调用方提交的当前用户 ID。SQL 值使用参数化绑定；更新必须限制归属并明确选择可写字段。

## 参数与响应

- User GET 返回 `successResponse(user)`；未登录的 GET、PUT 返回 HTTP 401 及 `{ success: false, code: "UNAUTHENTICATED", message: "请先登录", data: null }`。
- User PUT 使用回调的 `body` 参数，仅接受去除首尾空格后长 1–80 字符的 `displayName`，返回更新后的用户。此接口不能修改账号 ID、账号邮箱和管理员标识。
- 通知 GET 在回调中声明 `page`、`pageSize`、`unreadOnly` 为 `string | null`，由 Kooboo 原生绑定；分页字符串转换为 `pageNumber`、`pageSizeNumber` 后再校验。常规参数优先通过回调声明，JSON 请求体使用 `body: unknown`；需要明确指定参数来源时再显式读取 `k.request`。不手工解析 URL 或添加备用解析逻辑。默认值和上限来自 `ENV.notifications`；安装默认值为 page 1、pageSize 20、max page 100000、max pageSize 100。`unreadOnly` 只接受 `true`/`false`。非法值返回 HTTP 400 / `INVALID_ARGUMENT`。
- 请求体结构、可写字段、字段类型和值范围分步校验；确认字符串类型后只做一次规范化处理。不要把不相关的校验规则拼成超长条件表达式。
- 在路由检查、身份校验、参数校验、业务调用和响应返回等逻辑步骤之间留一行空行。紧密相关的语句放在一起，不要每条语句后都加空行。
- `mark-read` 接收 JSON body `{ id }`，写入同时限定通知 ID 和当前接收人。重复标记安全，不暴露他人的通知是否存在。
- 业务调用与响应构造分开：先用局部变量接收 Service 结果，再 `return successResponse(result)`，不要在响应 helper 中嵌套业务调用。已有 `user` 等语义明确的变量可直接返回。错误使用 `return errorResponse(httpStatus, code, message, data?)`；helper 返回带约定 JSON body 的 `k.api.httpCode(...)`。Service 不依赖响应工具。
- 默认显式 import 响应模板；跳过服务端模板时，App Base API 内生成同契约的局部 helper，不强制通用请求工具或运行时绑定该结构。
- 用户同步使用原子 upsert，保留 App 自己的显示名称。`LastSeenAt` 表示观察到账号的时间，不冒充登录事件；`UpdatedAt` 表示 App 资料修改时间。

启用的前端能力在 mock 预览时也请求实际 HTTP 接口。明确关闭通知时不发通知请求，不能因 HTTP 错误而自动关闭模块。User service 仅把 401 / UNAUTHENTICATED 视为未登录；网络错误与 500 仍按失败处理。`/service.bootstrap.js` 中的 `loadAppBootstrap` 只在前端组合独立接口的结果供 Shell 使用。通用 `requestJson` 原样返回 payload，user/notification service 各自适配响应。

## 数据库准备与 mock 切换

新 App 默认先做 mock 预览，用户要求真实持久化时接入真实存储；保留已有数据策略。说明 mock 写入不保存数据。替换 mock API 内部实现时尽量保持前端 HTTP 契约稳定。

`create_app_base` 和 `install_backend_templates` 返回 `DatabaseDefinitions`，每项包含 `Resource` 和 `Status: "not-checked"`。安装不检查或创建表，即使选择 `authenticationMode="kooboo"` 也一样。`FoundationTables` 仅列出预期表名，不代表数据库已就绪。`authenticationMode="mock"` 的预览不需要表，不为将来可能的持久化提前读取定义或建表。`authenticationMode="kooboo"` 时，用户接口在每次已登录请求中写入 `AppUsers`（已安装的通知也会读取 `AppNotifications`），因此即使业务数据仍用 mock，也要在首次登录前创建缺失的 FoundationTables，否则登录后页面无法加载。

启用真实存储或调用已登录的真实 API 前：

1. 使用 `read_skill_resource(skillName="kooboo-coding", resourceName=...)` 读取相关的 `DatabaseDefinitions[].Resource`。App Base 使用[自己的 manifest](../../assets/app-base/manifest.json) 和[数据库定义](../../assets/app-base/database.json)；通知使用[对应 manifest](../../assets/backend-templates/notifications/manifest.json) 和[定义](../../assets/backend-templates/notifications/database.json)。后续切换缺少安装回执时，沿这些路径读取，不必重装。
2. 加载 SQLite Functions，通过 `list_database_table`、`get_database_table` 检查现状。每个 `tables[]` 项都对应 `create_database_table` 的参数对象（`name`、`columns`）；只创建缺失表。保留主键和唯一约束，特别是 upsert 依赖的 AppUsers.KoobooUserId 唯一约束。通知初始不创建普通查询索引。
3. 定制后以当前表结构和应用代码为准。先比较，再通过 `update_database_table` 执行授权范围内必要的变更。不要用模板覆盖已有表、重建表或删除数据，也不能将读取失败当成不存在。列类型及约束变化需要显式迁移，当前 update Function 不支持这些修改。
4. 读回表结构，再移除相关 mock 分支并启用真实使用。通过实际 API 验证跨请求写入和读取，以及适用的事务回滚、用户隔离。KScript 数据库检查不能证明 HTTP 权限正确；安装或结构检查不能证明数据库就绪。

数据库定义保留在模板包内，不是 CodeBlock，也不是 App 的运行时依赖。正常 Service 请求只操作数据；缺表属于初始化未完成，不能静默建表或回退 mock。重装模板不会执行迁移或升级。

`create_app_base(authenticationMode="mock")` 在服务端 CodeBlock 中安装明确的 `/* mock:start */` ... `/* mock:end */` 区块。`Services.user` 提供固定模拟身份，user 和 notification service 根据此可信身份一致分支。请求参数不能开启 mock。模拟资料写入仅返回预览结果，不跨请求持久化；通知为空，已读操作返回成功且不访问 SQLite。不创建真实登录会话。

**接入真实数据、权限敏感操作或交付前，删除 Services.user、API app.user 及已安装的 Services.notification 中所有标记的 mock 区块。** 读回 `ENV` 和上述已安装对象，确认无 mock 兜底，并验证匿名访问用户和通知接口均返回约定的 401。同时移除业务开发额外引入的 mock 路径。不能只修改浏览器配置，也不能在 API 出错时回退到 mock。新建时选择 `authenticationMode="kooboo"` 已不含这些区块；重复执行 create_app_base 保留已有资源，不能替你完成模式切换。

用户认证与通知业务分别负责自己的数据，不为了启动方便重新聚合为一个服务端 API。用户模型 ID 和平台管理员标识只是身份事实，不自动构成后续业务 API 的授权策略。

POST user/logout 使用相同的服务端身份模式：mock 返回成功且不清除已有真实会话；真实模式通过 Kooboo 退出并清理账号 cookie。前端退出始终调用此 API。

## TypeScript 源码

服务端 API 和 CodeBlock 使用 `.ts` 编写。安装时保留 TypeScript 原文，设置 `Extension = ".ts"`，继续使用 `ScriptType.Module`；运行时沿用 Kooboo 现有的 TypeScript 编译链。共享业务类型使用 `import type`。请求体以 `unknown` 接收，在 API 边界校验后向 Service 传递明确类型的参数；类型不能替代运行时校验。

仓库内编辑由 `assets/tsconfig.json` 的 `rootDirs` 关联 API、CodeBlock 和响应模板目录。`assets/backend-env.d.ts` 仅提供模板需要的平台类型子集；这两个开发辅助文件不安装到 App。CLI 项目使用生成的 `kooboo.d.ts` 和本地 import 路径转换。
