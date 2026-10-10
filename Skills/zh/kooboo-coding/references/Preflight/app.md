# App Preflight

Preflight 检查实际写入资源和交付证据。App Plan 保存、设计读取不等于实施授权或验收。

## 带规划 App

扫描前通过 `read_app_plan({})` 读取服务端当前 Plan。adapter 选择 `profile=app`，注入 `appPlanId` 和会话内 `scopeKey`，模型不自行构造这些参数。scanner 加载规则前重读服务端 Plan，核对其身份。

App Plan 保存/删除或 Blueprint 保存成功后，重新读取 Plan 再扫描；Ledger 使此前结构、运行和业务证据失效。失败或空批次 Plan 写入保留原证据；未变 Plan 的读取保留当前证据上下文。scopeKey 只标识当前会话的验证上下文，不是持久化 Plan 版本或业务交付范围。同一个 appPlanId 在规划修改前产生的迟到回执也会被拒绝。

资源写入使受影响结构覆盖和运行/业务证据失效。纳入全部修改资源和直接消费者，内聚批次后用 focused 检查，整个选定范围收尾前用 capability；adapter 自动补入已跟踪资源。当前匹配回执只证明其扫描结果，不证明完整业务覆盖；还需独立核对选定 Screen、Action、Service Operation、DataContract 和真实数据行为。

## 无规划 App

无 Blueprint/App Plan 的 App 由 adapter 选择共享 site 结构 profile，不切换到 Site Building。SiteMilestone 清单只是进度记录，既不作为扫描的证据，也不影响扫描结果。见 [SiteMilestone](../Scenario/app-building/site-milestones.md)。

## 证据与恢复

调用和资源覆盖遵循 [Preflight Core](index.md)。Plan 读取失败或身份不匹配会停止扫描，修正读取后再扫描，不编造标识或自行改 profile。所需运行/业务观察是独立证据；浏览器操作需明确授权，不可用证据保持待验。结构 PASS 不单独证明实际行为通过验证。

仅规划工作不需要资源扫描，也不触发自动实施。业务无关的简单 UI 调整遵循 Router 局部修复例外，不读 Plan 或恢复绑定；局部验证不代表整个 App 完成。
