# 可选服务端模板

模板将普通 CodeBlock 和带路由的 API 复制到站点，代码归 App 所有。它提供默认写法，不强制绑定框架。新接口优先复用已选择的工具；已有接口和用户指定的结构保持原有契约。替换工具应只需局部修改显式 import 和调用位置。

## 发现与安装

1. 使用 `read_skill_resource`，设置 `skillName="kooboo-coding"`，读取 `assets/backend-templates/index.json`。
2. 沿目录读取所选模板说明，确认导出、配置、接入与移除方式。需要轻量文件操作时，选择 `files` 并读取[FileIO 工具](../../assets/backend-templates/files/README.md)。它不安装表或 API；业务权限、引用关系和清理调度由调用方负责。
3. 通过 `use_function` 加载 `install_backend_templates`，传入 `{"templates":["response"]}`。安装器本身不要求 App Base 或 App Plan；仍遵守当前任务的实施范围。
4. 检查 `Success`、`Conflicts` 和 `Objects`。尚未读过的 `UsageGuides` 按需读取；已有副本可能经过用户修改，使用前先读实际代码。
5. `DatabaseDefinitions` 列出状态为 `not-checked` 的初始表结构资源，不代表数据库就绪。mock 认证的预览不需要表；`kooboo` 认证时须在首次登录请求前建表；真实 API 使用前按[数据库准备](app-base-services.md#数据库准备与-mock-切换)检查现有结构，通过 SQLite Functions 初始化缺失表；独立安装模板也适用。

仅安装所选模板；`[]` 不写入。一个模板可以包含多个 CodeBlock 和 API。写入前统一检查所选资源、同名冲突及路由冲突。内容和类型一致的已有代码跳过；内容或类型不同则返回冲突，绝不覆盖。持久化失败可能留下先前已创建的对象，返回结果会报告已完成和失败的对象，重试前先检查。

## App Base 默认行为

`create_app_base.backendTemplates` 控制选择：

| 参数 | 行为 |
|---|---|
| 首次创建时省略 | 安装 `response` 和 `notifications` |
| 已有 `app-config.js` 或 `app.js` 时省略 | 不隐式安装模板，保留修改、删除和先前跳过的选择 |
| `[]` | 不安装服务端模板，并关闭通知接入 |
| 显式 ID 列表 | 仅安装所选模板，执行相同的冲突检查 |

结果中的 `BackendTemplates` 返回模板专属回执。后续追加使用 `install_backend_templates`，无需重新创建 App Base。显式重新安装可以恢复已删除的副本。不提供自动升级或依赖管理器。

## 响应约定

可执行示例及导出契约见 `assets/backend-templates/response/README.md`。`successResponse(data = null, message = "", httpStatus = 200)` 和 `errorResponse(httpStatus, code, message, data = null)` 返回带扁平 `{ success, code, message, data }` 响应体的 `k.api.httpCode(...)`。API 中应直接 `return successResponse(...)` 或 `return errorResponse(...)`。这些函数只统一 HTTP 状态和响应体；认证、日志和数据访问仍由 API/Service 层处理。

模板不会改写 bootstrap/notification API 或前端请求工具；App 的 service 可以局部适配响应。自定义格式、下载及重定向均可使用；Kooboo 运行时和 Preflight 不强制该结构。

App Base 负责启动与用户骨架，可选能力归模板所有。通知存储不创建用户表，用户存储不创建通知表。独立安装模板不会修改前端配置；为已有 App 追加通知后，明确启用 `appENV.notificationsEnabled`。
