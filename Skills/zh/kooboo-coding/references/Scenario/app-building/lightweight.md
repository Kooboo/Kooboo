# 无规划 Artifact 的 App 实施

业务 Screen 写入前读取 [图片与上传](../../Frontend/images-and-files.md)，落实内容配图、图片字段上传及已落盘文件清理；不增加规划步骤。

当前发现确认 Blueprint 与 App Plan 均不存在，且用户目标已清楚到可以实施时，使用本路径。用户选择 P1 快速模式，或明确要求直接做出可用版本时，也使用本路径；这表示不创建 Blueprint 或 App Plan。适用于新建 App，也适用于未建立规划的既有 App 继续增加能力。用户希望先讨论时，先澄清重要业务问题；只有用户选择业务方案和实施计划路径时才创建规划。已有规划或规划无法读取时，使用现有规划路径并解决其状态。

## 范围与记忆

选择范围或写入资源前，读取 [SiteMilestone 契约](site-milestones.md)，了解其中的条目结构和进度记录方式。

- 读取 `get_site_info`、相关 Planning Topics 和站点 Memory 入口，再按需读取本次相关笔记及源码。复用已确定的设计、登录、路由和数据约定。
- 用简洁业务描述表达无规划时的每项能力。例如：“产品管理：查看、搜索、新增、编辑和停用产品。本次不包含出入库。”不增加结构化验收方案，不把页面/API 设计复制到清单。
- Planning Topics 管问题和答案；SiteMilestones 管范围和进度；Todos 管临时步骤；源码管运行配置。Memory 保存没有其他权威归属的稳定要求、约定、原因和资源入口，不复制实施清单或完整资源设计。

## 安装与实施

设计系统的选择与安装统一见 `references/Frontend/app-design-systems.md`；组件选择、Shell 与升级方式遵循所选包的指南。

1. 保留有意义的站点显示名。为空或临时名称时，等到即将调用 `create_app_base` 前再用 `update_site_settings` 仅将 `DisplayName` 改为已确定的 App 名称。
2. 新建客户端 App 时读取设计系统目录，根据当前需求选择精确的内置 ID，尊重用户明确选择。读取选定包与 [App 布局](../../Frontend/cdn-vue/app-layout.md)。依据当前需求确定 `mock` 或 `kooboo` 登录方式，只询问重要的未决选择。使用 mock 数据前说明策略，保留用户对真实持久化的要求。
3. 按 [设计系统消费规则](../../Frontend/app-design-systems.md)，通过直接配置调用 `create_app_base`；它安装公共基础能力与中性首页占位，不要求页面清单。已有 App Base 不重复创建。
4. 实施当前能力的真实 API → service/store → 组件 → Screen 依赖闭包。AI 自行安排必要临时步骤和检查，遵守与带规划 App 相同的客户端资源、交互、认证和设计系统约定。依赖保存并读回后再导入；业务页面实施时逐步增加路由与导航，保留已有路由。首个入口页面完成后替换中性首页占位，不得改写共享占位组件来冒充多个页面。
5. 新 App 默认先 mock，用户要求真实持久化时接入，保留已有数据策略。mock API 在 API 中保留样本读取，写入返回成功信号，不宣称持久化，不为预览创建业务表。`kooboo` 认证时，DatabaseDefinitions 中的 FoundationTables 仍须在首次登录前创建。真实使用前按[数据库准备与 mock 切换](../../Backend/app-base-services.md#数据库准备与-mock-切换)读取返回的 DatabaseDefinitions，通过 SQLite Functions 初始化缺失表；替换 mock 内部实现时保持前端 HTTP 契约稳定。安装后以源码配置为准，稳定决策与入口按需保存到 Memory。

## 验证与完成

读回修改后的源码，使用适当检查核对目标行为。每个内聚批次后执行 focused Preflight，完成前执行 capability Preflight，参见 [App Preflight](../../Preflight/app.md)。Adapter 为本次扫描选择共享的 `site` 结构 profile，不填写或编造 Plan 版本；这个扫描 profile 不代表转入 Site Building。

解决写入失败与实际检查失败，收集扫描要求的运行证据，包括适用时现有的只读 Preview diagnostics。浏览器操作仍需用户明确要求。结构检查与通用 diagnostics 不证明业务完成：按 `content` 判断实际结果，说明检查内容和未实测行为，未满足的范围保持未完成。不要求结构化业务验收 Artifact。

按 [SiteMilestone 契约](site-milestones.md) 保存授权批次的清单进度；保存只是进度记录，不门控收口。
