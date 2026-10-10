# 建站状态协议

本文定义建站里程碑、会话 Todo、下一步推荐三类状态。三者外观相似，但维度、生命周期和函数完全不同，禁止互相替代。

## 三个概念

| 概念 | 维度 | 函数 | 回答的问题 | 生命周期 |
|------|------|------|------------|----------|
| 建站里程碑 | 站点级 | `update_site_milestones` | 这个站点整体走到哪 | 跨会话，随站点建设推进 |
| 会话 Todo | 会话/阶段级 | `update_todos` | AI 当前这一轮正在做什么 | 当前阶段内有效 |
| 下一步推荐 | 决策入口 | `show_next_recommended_action` | 当前阶段完成后还能做什么 | 当前轮结束后的决策窗口 |

## 建站里程碑

建站里程碑是站点级路线图，不是当前阶段 todo，也不是工具调用清单。每个 milestone 应是一段用户可感知的建设阶段，例如静态首版、内容动态化、表单线索、在线支付、多语言 SEO。

`update_site_milestones` 参数为对象：

```json
{
  "milestones": [
    {
      "content": "交付可预览静态首版",
      "completed": false
    }
  ]
}
```

模型提交完整的 `milestones` 数组（每项包含 `content` 与 `completed`），保存会覆盖已存储的列表。它只是进度记录：不依赖 Completion Ledger，不门控任何步骤，也不重置阶段证据。

规则：

- `content` 只写阶段名，不要附加状态词、序号、图标或 “进行中/已完成”。
- `completed` 是唯一状态字段。当前只支持 `todo/done` 两态，不要编造 `running`、`paused`、`blocked`。
- 里程碑只写用户可感知的大节点，不写 `create_page`、`create_view`、读取文档、刷新预览、生成设计系统、执行 Preflight 等内部细活。
- 真正的站点级 milestones 必须在 S2 产出 Site Brief + Page Plan 后生成；S0/S1 信息不足，不得生成固定通用路线图。
- 先按能力前置关系排列主干，再合并成 2-4 个用户可感知结果；同一依赖链上的能力不能作为无先后关系的平级清单。
- `content` 使用 Site Brief 与 Page Plan 中的业务名词描述结果，不直接复制能力目录的内部标题。例如应说明哪类内容可维护、哪条用户路径可完成，而不是只写“内容动态化”或“会员系统”。
- 只有用户已明确要求或完成最终目标所必需的能力才进入 milestones；可选增强留给阶段完成后的推荐。
- 静态首版 milestone 的完成只表示当前静态阶段结束，不表示后续已承诺的动态或业务能力已经完成。
- 若用户采纳下一步推荐进入动态化、表单、支付、会员、多语言等能力，先追加或激活对应站点级 milestone，再生成该 milestone 下的当前阶段 todos。

## 会话 Todo

会话 Todo 是当前阶段的细颗粒执行清单。它是当前 milestone 的子级任务，但当前函数不表达父子结构，因此 `content` 只写当前阶段内的用户可读任务。

`update_todos` 参数为对象：

```json
{
  "todos": [
    {
      "content": "创建首页骨架并打开预览",
      "completed": false
    }
  ]
}
```

规则：

- `content` 只写任务本身，不要附加状态词、序号、图标或 “进行中/已完成”。
- 只覆盖当前阶段，不要把站点级总流程塞进 `update_todos`。
- 不要一次性展开所有 milestones 的子任务。
- S1 返回有效 `buildMode` 后，若继续推进建站，必须先调用 `update_todos` 建立当前阶段待办。
- 每次进入新的 milestone 阶段，第一步都是生成当前阶段 todos。
- 完成当前阶段后，必须把当前 todos 全部更新为 `completed: true`，再进入推荐或下一阶段判断。

## 下一步推荐

下一步推荐是阶段完成后的非阻塞决策入口。它不是 `ask_user_input`，不会阻塞用户，也不代表用户已经接受下一阶段。

`show_next_recommended_action` 是 AI Chat 默认函数，直接调用即可，不需要 `function_discover` 或 `use_function`。它不是 skill script，禁止写成 `run_skill_script({ "skillName": "kooboo-coding", "scriptName": "show_next_recommended_action" })`。

正确调用形态是直接调用函数，参数为对象：

```json
{
  "actions": [
    {
      "title": "把产品内容接入动态数据",
      "description": "当前静态首版已完成，产品列表适合改为可维护的数据源。"
    }
  ]
}
```

规则：

- 每次阶段完成、当前 todos 全部 done 后，若存在自然下一步，调用 `show_next_recommended_action` 展示 1-3 个推荐。
- 推荐项必须来自当前 Site Brief、Page Plan、已完成 milestone、用户最近表达和 [capability-catalog.md](capability-catalog.md)，不要枚举与站点无关的能力。
- 先推荐“依赖已满足的未完成 milestone”。若后续 milestone 依赖当前项，只展示当前项，不把整条依赖链同时铺成多张卡。
- 只有多个能力可独立开始且都具备直接证据时才展示 2-3 项；没有未完成的既定主干后，才从 Page Plan 的表单、列表、订阅、语言或搜索意图中选择可选增强。
- `title` 写当前站点的下一项可见结果；`description` 用一句话说明“现有证据 + 完成后的变化”。标题和描述都要按结果去重，不能把同一条交易链拆成近义推荐。
- 推荐不替代用户确认。涉及动态化、数据源、支付、登录、邮件营销等会改变范围的动作，必须等用户采纳或明确回复后才进入对应阶段。
- 用户下一轮无论点击推荐还是自由输入，上一轮推荐都视为过期；不要继续假定用户接受了推荐。

## 标准生命周期

```text
S1 返回 buildMode
  -> update_todos 建立 S2 需求收敛待办
  -> S2 产出 Site Brief + Page Plan
  -> update_todos 将 S2 待办全部 done
  -> 读取 capability-catalog.md
  -> update_site_milestones 生成当前站点路线图
  -> update_todos 建立 S3 设计系统待办
  -> S3 产出 designSystem
  -> update_todos 将 S3 待办全部 done
  -> update_todos 建立 S4 静态实现待办
  -> 执行静态首版并持续更新 todos
  -> 当前阶段 todos 全部 done
  -> update_site_milestones 更新站点级完成状态
  -> 如存在自然下一步，show_next_recommended_action 展示推荐
  -> 用户采纳推荐或自由输入下一步
  -> 再生成下一阶段 todos
```

## Minimal Execution Trace（正确调用轨迹）

以下 trace 是最小正确顺序。具体站点内容可以变化，但顺序不得反转。

```text
load_skill("kooboo-coding")
read_skill_resource("kooboo-coding", "references/Scenario/site-building/index.md")
read_skill_resource("kooboo-coding", "references/Scenario/site-building/concepts.md")
read_skill_resource("kooboo-coding", "references/Scenario/site-building/planning.md")
S0 执行就绪检查
ask_user_input(S1 起步模式)
update_todos(S2 需求收敛待办)
load_skill("ui-ux-pro-max-pp")
run_skill_script(mode: "page-plan", brief: object)
update_todos(S2 需求收敛待办全部 completed: true)
read_skill_resource("kooboo-coding", "references/Scenario/site-building/capability-catalog.md")
update_site_milestones(基于 Site Brief + Page Plan 的站点路线图)
update_todos(S3 设计系统待办)
run_skill_script(mode: "design-system", brief: same object)
update_todos(S3 设计系统待办全部 completed: true)
update_todos(S4 静态首版实现待办)
read_skill_resource("kooboo-coding", "references/Scenario/site-building/static-delivery.md")
read_skill_resource(Frontend/function/preflight 等当前子步骤必读文档)
create_layout / create_style / view-batch(shared blockRefs) / create_view(shared)
page-item -> create_page(含 composition wrapper，首页并打开预览)
view-batch -> 逐 item create_view -> update_text_object(保留 wrapper) -> Preflight
read_skill_resource("kooboo-coding", "references/Preflight/index.md")
run_skill_script(preflight-scan.js, deliveryStage: "static-first", mode: "capability")
update_todos(当前阶段全部 completed: true)
update_site_milestones(当前 milestone completed: true)
show_next_recommended_action(若存在自然下一步)
最终回复用户并展示预览结果
```

反向 trace 一律视为错误：

| 错误 trace | 为什么错 |
|------------|----------|
| `read index.md -> ask_user_input -> update_todos -> read concepts.md` | `concepts.md` 必须紧跟 `index.md`，否则模型会先执行再学习状态边界 |
| `read index.md -> load ui-ux -> designSystem -> pagePlan` | 整站建站必须先 S1 + todos，再 S2 Page Plan，设计系统属于 S3 |
| `pagePlan -> designSystem -> update_site_milestones` | 站点级 milestones 必须在 Page Plan 后、S3 前生成 |
| `Preflight PASS -> Markdown 表格写下一步建议 -> final reply` | 下一步推荐必须调用默认函数 `show_next_recommended_action`；Markdown 只能补充说明，不能替代函数 |
| `run_skill_script(scriptName: "show_next_recommended_action")` | `show_next_recommended_action` 是默认函数，不是 skill script |
| `show_next_recommended_action -> update_todos(下一阶段)` | 推荐不是用户确认；只有用户采纳后才能生成下一阶段 todos |

## 常见错误

| 错误 | 正确做法 |
|------|----------|
| 把 Preflight 当成全站最后一步 | milestone 写成“完成静态首版验收”，并把它限定为静态首版阶段 |
| S0/S1 后立即生成完整固定 milestones | S2 有 Site Brief + Page Plan 后再生成 |
| 用 `update_todos` 写完整建站路线图 | 用 `update_site_milestones` 写站点级路线图，用 `update_todos` 写当前阶段 |
| 在同一批 todos 中同时写 S2、S3、S4 | 每进入一个阶段重新调用 `update_todos`，只保留当前阶段细活 |
| 建立 milestones 后直接 `run_skill_script` 或 `create_*` | S1 后先 `update_todos`，再进入实施类函数 |
| 阶段完成后直接切下一批 todos | 先更新 todos done 和 milestones，再给下一步推荐，等用户采纳 |
| 纯展示站默认推荐支付/会员 | 从能力目录按 applies/notWhen 裁剪，只推荐当前站点自然下一步 |
