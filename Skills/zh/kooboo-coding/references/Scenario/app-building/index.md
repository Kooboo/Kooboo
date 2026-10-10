# ToB App Building 入口

先按本次变更语义分流。已有 App 的间距、对齐、颜色、字体、展示位置等简单 UI 调整走 `kooboo-coding` 的局部修复，不进入规划发现。业务含义、数据、权限、操作、路由契约或功能范围改变时读取业务契约；已有业务变化走[需求对齐](requirement-review.md)。

App Building 与公开 Site Building 是不同路径，不切换路径绕过授权或验证。

## 发现与路径选择

新 App Building 会话先调用 `get_site_info`，按用户希望的起步方式选择：

- **业务建模：** 读取 `read_app_blueprint({})`，复用语义图，遵循 [Blueprint](blueprint.md)。用户未同时授权实施规划时不加载 App Plan。
- **应用交付：** 读取 `read_app_blueprint({})` 和 `read_app_plan({})`，再续作、更新或建立用户要求的规划。新会话不意味着重建；读取失败不意味着不存在。
- 两份规划均无但已有有意义的应用资源时，默认继续维护已有 App，用户要求时才建立规划。

**P1 快速模式**直接实施当前授权范围，不创建 Blueprint 或 App Plan；两者均不存在时走[无规划实施](lightweight.md)。**P2 详细模式**按下文两个规划阶段形成可审核的业务方案和实施计划。已有 Artifact 应续作，不重建或清空后切到快速模式。

不按系统类别、领域名称、模块数量或熟悉案例强制选择规划。复用用户已选的快速/详细模式或规划要求。只有下一步意图仍不明确时才问“先做出可用版本，还是先讨论业务方案和实施计划？”需求缺口只触发必要澄清；复杂度影响实施范围和顺序，不强制创建规划。

已授权实施时优先遵循用户明确范围；范围未指定时选择可用核心业务闭环及其依赖。完整 Plan 不自动授权全模块实施，保留后续工作。仅要求规划时在规划完成后停止。

## 阶段一：Blueprint

语义写入前读 [Blueprint 编写](blueprint.md)。从业务意义开始：Blueprint → Actor/Goal/Module → Scenario → 原子 Fact → 衍生 Business Object 和业务规则。Module 是业务单元，Scenario 可属于多个 Module，不从表开始，也不为表达归属复制 Scenario。

遵循专题中的 Native Function 和完整对象保存边界，复用稳定 ID 与已有确认授权。纯语义请求完成本批语义对象后停止。`read_app_blueprint` 返回 `canCreateAppPlan=true` 且用户授权规划后进入 App Plan；就绪不等于授权或完整业务覆盖。

## 阶段二：App Plan

写入前读 [App Plan 编写](app-plan.md)，选择设计前读[设计系统](../../Frontend/app-design-systems.md)。App Plan 是当前服务端实施设计，包含 Settings、SourceFiles、DataContracts、Services、Screens、DeliverySlices。

通过 `use_function` 按需加载强类型保存工具，按依赖顺序保存审核后的完整对象，用 `read_app_plan({})` 读回。首次成功保存自动创建根对象。不通过脚本或资源文件持久化规划。Schema、批次、删除、恢复由 App Plan 专题定义，入口不重复其协议。

## 阶段三：实施

App Plan 定义如何实施业务，DeliverySlice 组合选定业务结果；执行状态独立。带规划 App 实施不要求 Milestone 绑定，以选定 Slice 及其 Screen、Action、DataContract、Service、SourceFile 为设计依据，用 Todo 记录临时步骤。[SiteMilestone](site-milestones.md) 在适用场景保留为独立清单，不是 Plan 的前置条件。

读[客户端实现入口](../../Frontend/cdn-vue/index.md)和[交付指南](../../Frontend/cdn-vue/delivery.md)。基于已保存 Settings 调用 `create_app_base`，按依赖顺序实施后端资源、前端适配与 Screen。App Base 安装和路由占位不等于业务实现。后端工作先读 [API 分层](../../Backend/api-layer-guide.md)。数据策略按实际实施需求确定，mock 不代表真实持久化。

App 显示名为空或临时名称时，等到即将调用 `create_app_base` 前再用 `update_site_settings` 只设置已确定的 DisplayName；规划期间不改名，不改 Website.Name、域名、路径，保留已有有意义名称。

恢复、规划修改、实现与设计冲突或完成前核对交付范围时读取当前 Plan。已有业务变化走[需求对齐](requirement-review.md)。完成前逐项核对选定能力的实际资源与证据，遵循 [Preflight](../../Preflight/index.md) 中适用检查；规划保存成功不代表验收。

## 讨论与对用户的表达

称为“业务方案”和“实施计划”，不要求用户理解内部 ID、工具名。技术实施细节不变成额外确认问题。同一审核范围复用已有授权，区分规划与实施，只询问真实未决选择。

假设、开放问题、未确认答案属于站点级[待确认事项](planning-topics.md)，独立于 Blueprint/App Plan。继续讨论、规划或实施前读取相关事项，使用其当前 Function 记录讨论。Topic 确认不单独授予规划或资源写入权限。
