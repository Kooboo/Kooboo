# Complex Job SOP Files

默认把完整任务说明写入 `AITaskJob.Prompt`。只有复杂自动化才建议额外创建 kfile 或 content file 保存 SOP，再让 job prompt 读取 SOP。

## 什么时候使用外部 SOP

使用外部 SOP 文件的条件：

- 执行流程很长，prompt 会变得难以审核。
- 规则会经常调整，例如每日发帖来源、标题规范、黑名单、发布模板。
- 需要保存较长示例、JSON schema、分类映射、去重规则或失败处理矩阵。
- 多个 job 需要复用同一份 runbook。

不使用外部 SOP 的条件：

- 任务步骤少于 8 到 10 步。
- 没有长规则、长示例或可复用 runbook。
- 用户只是创建一个普通日报、周报、巡检或简单同步任务。

## 创建外部 SOP 的规则

1. 创建 SOP 文件前先向用户说明原因和路径。
2. 只有在当前函数/toolset 支持创建 kfile、content file 或通过已验证 KScript 写入 FileIO 时才创建。
3. 如果使用 `run_kscript` 写入 FileIO，必须先 `load_skill("kooboo-coding")`，并读取 `references/K-Script/index.md` 与 `references/K-Script/file/index.md`，确认 `k.file` API 后再写。
4. SOP 文件内容应稳定、可读、可维护，不要把一次性聊天过程塞进去。
5. job prompt 中要写明如何读取 SOP，以及 SOP 不存在时如何失败退出。

## 建议文件位置

推荐使用稳定路径，例如：

- `automation/<job-slug>.md`
- `automation/sops/<job-slug>.md`
- `ai-task-sops/<job-slug>.md`

路径应由用户确认，或者根据 job 名生成清晰 slug。

## job prompt 读取 SOP 的写法

```markdown
## SOP
本任务的详细执行规则保存在 FileIO 路径 `automation/daily-forum-post.md`。

执行前：
1. 调用 load_skill("kooboo-coding")。
2. 读取 references/K-Script/index.md 和 references/K-Script/file/index.md。
3. 使用 run_kscript 通过 k.file.read("automation/daily-forum-post.md") 读取 SOP。
4. 如果 SOP 文件不存在或读取失败，不要继续写入站点；调用 ai_task_finish，Status=Warning，说明 SOP 不可用。
```

## SOP 文件模板

```markdown
# [Job Name] SOP

## Goal
[长期运行目标]

## Inputs
[外部来源、站内路径、配置项]

## Allowed Writes
[允许创建/更新的对象]

## Forbidden Actions
[禁止修改、删除或覆盖的对象]

## Workflow
1. [步骤]
2. [步骤]
3. [步骤]

## Quality Gates
[去重、最小数据量、内容质量、审核规则]

## Failure Handling
[跳过、降级、Warning、Suggestion]

## Finish Message
[ai_task_finish Message 格式]
```

## 注意

不要把外部 SOP 当成逃避用户审批的方式。无论 prompt 还是 SOP，写入范围、禁止事项和失败处理都必须让用户在创建 job 前看懂并确认。
