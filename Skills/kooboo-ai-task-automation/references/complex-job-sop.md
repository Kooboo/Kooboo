# Complex Job SOP Files

By default, put the complete task description in `AITaskJob.Prompt`. Create an additional kfile or content file to store an SOP only for complex automation, then have the job prompt read the SOP.

## When to use an external SOP

Conditions for using an external SOP file:

- The execution flow is long and the prompt would become difficult to review.
- Rules change frequently, such as daily-post sources, title conventions, blacklists, or publishing templates.
- Long examples, JSON schemas, category mappings, deduplication rules, or failure-handling matrices must be stored.
- Multiple jobs need to reuse the same runbook.

Conditions for not using an external SOP:

- The task has fewer than 8 to 10 steps.
- There are no long rules, long examples, or reusable runbooks.
- The user is creating an ordinary daily report, weekly report, inspection, or simple synchronization task.

## Rules for creating an external SOP

1. Explain the reason and path to the user before creating the SOP file.
2. Create it only when the current function/toolset supports creating a kfile, content file, or writing FileIO through verified KScript.
3. If using `run_kscript` to write FileIO, first call `load_skill("kooboo-coding")`, read `references/K-Script/index.md` and `references/K-Script/file/index.md`, and confirm the `k.file` API before writing.
4. Keep the SOP stable, readable, and maintainable; do not put a one-time chat process into it.
5. The job prompt must state how to read the SOP and how to fail and exit when the SOP does not exist.

## Suggested file locations

Use a stable path, for example:

- `automation/<job-slug>.md`
- `automation/sops/<job-slug>.md`
- `ai-task-sops/<job-slug>.md`

The path should be confirmed by the user or generated from the job name as a clear slug.

## How the job prompt reads the SOP

```markdown
## SOP
本任务的详细执行规则保存在 FileIO 路径 `automation/daily-forum-post.md`。

执行前：
1. 调用 load_skill("kooboo-coding")。
2. 读取 references/K-Script/index.md 和 references/K-Script/file/index.md。
3. 使用 run_kscript 通过 k.file.read("automation/daily-forum-post.md") 读取 SOP。
4. 如果 SOP 文件不存在或读取失败，不要继续写入站点；调用 ai_task_finish，Status=Warning，说明 SOP 不可用。
```

## SOP file template

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

## Note

Do not use an external SOP to bypass user approval. Whether in the prompt or the SOP, the write scope, forbidden actions, and failure handling must be understandable to and confirmed by the user before the job is created.
