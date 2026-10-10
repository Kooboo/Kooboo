# App 运行时设计系统

本文用于 ToB App 可执行 Vue 设计系统的选择与消费。`assets/design-systems/catalog.json` 是选择入口；目录中的每套系统必须提供受支持的 runtime manifest。静态 CSS/Preview 设计包不再作为安装选项。

## 选择一套运行时

- 可选项以目录为准；新增系统须具备 runtime、组件 API 文档和安装契约。
- 优先遵循用户明确选择。否则结合业务任务、信息密度、交互需求和响应式布局，对比目录中的 `bestFor`、`signatureTraits` 和 `reuseDefaults`。不读取全部候选包，也不对所有 App 固定使用同一品牌。
- 无 Blueprint/App Plan 时，将选定 ID 传入 `create_app_base`，在站点记忆记录理由与资源路径；不为选择设计系统额外建立规划。
- 有规划时复用用户明确设计选择；否则通过 `ask_user_input(kind="design-system")` 提供推荐 `defaultValue` 与业务理由，要求 `confirmed=true`。通过 `save_app_settings` 在 `settings.designSystem` 保存目录精确 ID/name，更新时保留完整 Settings。快速直接交付遵循上方无规划路径。
- 后续 Screen 沿用已选系统。已保存的旧系统 ID 必须在新安装前显式重新选择，不能静默映射成另一套 runtime，也不能覆盖已有站点运行库。
- 阅读 [Frontend Design](frontend-design.md)，遵循布局完整性、空间节奏、层级和响应式容纳要求。运行库负责视觉语言，业务要求决定页面组合。

## 渐进读取

规划阶段使用目录与当前业务证据；只有明确的能力疑问才读取所选系统概览。不要为了排序读取全量 manifest、CSS、组件 API 或所有示例。规划不存储 HTML/CSS 实现内容；App Plan 的 SourceFiles 可以规划文件名和相对目录。

实施阶段：

1. 阅读所选包的 `SKILL.md`；选择组件能力或查找 API 名称时读取轻量的 `runtime/component-selection.json`。
2. 按所选包指南设置 `create_app_base.components`；它对安装范围与 API 指南的影响由该包契约决定。不要假设所有运行库采用同一策略。
3. 直接使用返回的 `DesignRuntime.UsageGuides.Resources`；仅根据 `UsageGuides.Deferred` 或 `DesignRuntime.Components.api`，通过 `read_skill_resource` 补读当前缺失的指南。文档缺失不能作为猜测 props、事件或 slots 的理由。
4. 从 `DesignRuntime.ModuleUrl` 导入精确命名的 Vue 导出。service、directive、locale 不能当模板组件使用。保留原生 API，不跨库套用 props，也不引入另一份 Vue。
5. 只有排查明确故障或未文档化行为时才读取托管源码。使用已文档化组件不要求全量读取设计 CSS、静态 Preview 或 UIKit，也不递归枚举维护文件。

## Shell 与样式

- `create_app_base` 安装基础能力、所选运行库和路由占位。工作区符合业务时选择所选包指南中的 Shell 预设；否则用已安装组件自行组合布局。安装成功不代表业务 Screen 完成。
- Shell 的路由归属与基础能力接入遵循 [App 布局](cdn-vue/app-layout.md)；修改前读取实际布局组件源码。
- 按所选包指南加载运行库与适配样式，再加载业务 `/app.css`；核对 Shell 已有留白与滚动容器，避免 Screen 重复叠加。
- 运行库负责组件 DOM、CSS、交互和无障碍行为；使用公开 props、事件、slots 和主题 tokens。不从编译 CSS 重建按钮、表单、表格、卡片与导航，也不将各库改造成统一的 `.ds-*` API。
- `/app.css` 负责业务布局、间距、宽度分配、滚动和针对具体契约缺口的局部覆盖；编辑前读取已有业务样式。不用模型生成代码替换安装器管理的 JS/CSS 或基础 reset。
- 业务组件组合阅读所选 `runtime/extensions.md`，主题与颜色按包内指南处理；不假设各库共享 token 或暗色能力。
- 按所选包支持的模式预览 `sync_design_runtime`，执行时使用该预览精确的 `expectedRevision`。尊重资产修改冲突并保留业务代码，不静默迁移已淘汰的静态设计系统。

## 业务内容与图标

设计包提供视觉和交互契约，不提供当前 App 的业务事实。示例名称、指标、身份、时间、表格记录和说明文字都是 fixture。所有可见标签、标题、aria-label、placeholder、tooltip、空状态和图表值必须来自当前业务要求、用户输入或 API/service 数据。Mock 数据也必须表达当前业务，不搬用运行库演示内容。不需要的区域应删除并收拢布局。

按业务语义选择导航和操作图标。业务图形使用设计包声明的图标适配器与已安装 App Base Lucide，库组件内部保留其上游原生图标。不用 emoji、Unicode 符号、空框或手绘字形替代。纯图标控件需有无障碍名称；文字旁装饰图标不重复其可访问名称。

## 图表、动效与交付

- 只为实际业务任务增加图表。需要坐标轴、图例、tooltip、多系列、响应式更新或交互时优先成熟图表引擎，使用 runtime 文档的 tokens/主题适配；CSS 柱条不能代替所需图表能力。
- 复用组件已文档化的动效和状态；没有明确依据时，新增动效保持简短且服务于功能。
- 业务资源实施和交付顺序见 [客户端交付](cdn-vue/delivery.md)。
- 间距、层级、容纳、状态处理和控件行为与 token 一致性分别检查。避免重复 Shell、无效账户控件、无法滚动的溢出、装饰指标与演示内容泄漏。
- 源码/结构检查、人工组件验收和真实业务 App 验收是独立证据。没有用户明确授权不操作浏览器；未实测的视觉/运行行为保持未验证。
- 保留运行库许可证与归属声明。目录收录代表实施契约，不代表浏览器验收通过。
