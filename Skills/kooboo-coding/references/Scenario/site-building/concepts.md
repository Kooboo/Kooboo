# Site Building State Protocol

This document defines three kinds of state: site-building milestones, session todos, and next-step recommendations. They look similar, but their dimensions, lifecycles, and functions are completely different and must not substitute for one another.

## Three Concepts

| Concept | Dimension | Function | Question answered | Lifecycle |
|------|------|------|------------|----------|
| Site-building milestone | Site-level | `update_site_milestones` | How far has the site progressed overall? | Across sessions, advancing with site construction |
| Session Todo | Session/stage-level | `update_todos` | What is AI doing in this round? | Valid within the current stage |
| Next-step recommendation | Decision entry point | `show_next_recommended_action` | What else can be done after the current stage? | Decision window after the current round ends |

## Site-Building Milestones

A site-building milestone is a site-level roadmap item, not a current-stage todo or a tool-call checklist. Each milestone should be a user-perceivable construction stage, such as a static first version, dynamic content, form leads, online payment, or multilingual SEO.

`update_site_milestones` takes an object:

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

The model submits the complete `milestones` array (each item has `content` and `completed`). The save overwrites the stored list. It is a progress record only: it does not depend on the Completion Ledger, does not gate any step, and does not reset stage evidence.

Rules:

- `content` contains only the stage name; do not append status words, sequence numbers, icons, or “in progress/completed”.
- `completed` is the only status field. Only the two states `todo/done` are supported; do not invent `running`, `paused`, or `blocked`.
- Milestones contain only user-perceivable major steps, not internal details such as `create_page`, `create_view`, reading documents, refreshing previews, generating a Design System, or running Preflight.
- Generate real site-level milestones only after S2 produces the Site Brief + Page Plan; S0/S1 lack enough information and must not generate a fixed generic roadmap.
- Order the main path by capability prerequisites first, then merge it into 2–4 user-perceivable outcomes; capabilities on the same dependency chain must not appear as an unordered flat list.
- `content` uses business terms from the Site Brief and Page Plan to describe results rather than copying internal capability-catalog titles. For example, state what kind of content can be maintained or which user path can be completed, instead of writing only “dynamic content” or “membership system”.
- Include a capability in milestones only when the user has explicitly requested it or it is required to complete the final goal; leave optional enhancements for recommendations after the stage is complete.
- Completing the static-first-version milestone means only that the current static stage is finished; it does not mean later promised dynamic or business capabilities are complete.
- If the user accepts a recommendation to enter dynamicization, forms, payment, membership, multilingual, or another capability, first add or activate the corresponding site-level milestone, then create current-stage todos under that milestone.

## Session Todos

A session Todo is the fine-grained execution checklist for the current stage. It is a child task of the current milestone, but the current function does not express parent-child structure, so `content` contains only a user-readable task within the current stage.

`update_todos` takes an object:

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

Rules:

- `content` contains only the task itself; do not append status words, sequence numbers, icons, or “in progress/completed”.
- Cover only the current stage; do not put the site-wide process into `update_todos`.
- Do not expand subtasks for all milestones at once.
- After S1 returns a valid `buildMode`, if site building continues, call `update_todos` first to establish current-stage tasks.
- The first step whenever entering a new milestone stage is to generate current-stage todos.
- After the current stage is complete, update all current todos to `completed: true` before entering recommendation or next-stage evaluation.

## Next-Step Recommendations

A next-step recommendation is a non-blocking decision entry point after a stage is complete. It is not `ask_user_input`, does not block the user, and does not mean the user has accepted the next stage.

`show_next_recommended_action` is an AI Chat default function and can be called directly; it does not require `function_discover` or `use_function`. It is not a skill script and must not be written as `run_skill_script({ "skillName": "kooboo-coding", "scriptName": "show_next_recommended_action" })`.

The correct call form is a direct function call with an object parameter:

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

Rules:

- After each stage is complete and all current todos are done, call `show_next_recommended_action` to show 1–3 recommendations when a natural next step exists.
- Recommendations must come from the current Site Brief, Page Plan, completed milestones, the user's recent statements, and [capability-catalog.md](capability-catalog.md); do not enumerate capabilities unrelated to the site.
- Recommend incomplete milestones whose dependencies are satisfied first. If a later milestone depends on the current item, show only the current item instead of laying out the entire dependency chain as multiple cards.
- Show 2–3 items only when multiple capabilities can start independently and each has direct evidence. Once no unfinished established main path remains, choose optional enhancements from Page Plan intent for forms, lists, subscriptions, language, or search.
- `title` states the next visible result for the current site; `description` explains “existing evidence + what changes after completion” in one sentence. Deduplicate both by outcome and do not split one transaction chain into near-synonymous recommendations.
- A recommendation does not replace user confirmation. Actions that change scope, including dynamicization, data sources, payment, login, or email marketing, must wait until the user accepts or explicitly responds before entering the corresponding stage.
- Regardless of whether the user clicks a recommendation or enters free text in the next round, all recommendations from the previous round are expired; do not continue assuming that the user accepted one.

## Standard Lifecycle

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

## Minimal Execution Trace (Correct Call Trace)

The following trace is the minimum correct order. Concrete site content may vary, but the order must not be reversed.

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

Any reverse trace is always an error:

| Incorrect trace | Why it is wrong |
|------------|----------|
| `read index.md -> ask_user_input -> update_todos -> read concepts.md` | `concepts.md` must immediately follow `index.md`; otherwise the model executes before learning the state boundaries |
| `read index.md -> load ui-ux -> designSystem -> pagePlan` | Whole-site building must do S1 + todos first, then the S2 Page Plan; the Design System belongs to S3 |
| `pagePlan -> designSystem -> update_site_milestones` | Site-level milestones must be generated after the Page Plan and before S3 |
| `Preflight PASS -> Markdown 表格写下一步建议 -> final reply` | Next-step recommendations must call the default function `show_next_recommended_action`; Markdown may add explanation but cannot replace the function |
| `run_skill_script(scriptName: "show_next_recommended_action")` | `show_next_recommended_action` is a default function, not a skill script |
| `show_next_recommended_action -> update_todos(下一阶段)` | A recommendation is not user confirmation; create next-stage todos only after the user accepts it |

## Common Errors

| Error | Correct approach |
|------|----------|
| Treating Preflight as the final step for the whole site | Write the milestone as “complete static first-version acceptance” and limit it to the static-first-version stage |
| Generating a complete fixed milestone list immediately after S0/S1 | Generate it after S2 has a Site Brief + Page Plan |
| Using `update_todos` for the complete site-building roadmap | Use `update_site_milestones` for the site-level roadmap and `update_todos` for the current stage |
| Writing S2, S3, and S4 together in one todo batch | Call `update_todos` again at each stage and keep only the current stage's details |
| Calling `run_skill_script` or `create_*` immediately after establishing milestones | After S1, call `update_todos` first, then enter implementation functions |
| Moving directly to the next todo batch after a stage completes | First mark todos done and update milestones, then recommend the next step and wait for user acceptance |
| Recommending payment/membership by default for a pure presentation site | Trim recommendations using the capability catalog's applies/notWhen rules and recommend only the site's natural next step |
