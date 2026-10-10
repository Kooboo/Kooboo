---
name: kooboo-ai-task-automation
description: Use when the user wants to create or refine a Kooboo AI Task Job, fixed routine, scheduled task, or chat-created automation from natural language, including write-capable jobs such as scheduled posting, content sync, cleanup, SEO updates, reports, and inspections.
---

# Kooboo AI Task Automation

把用户口语化的自动化需求，编译成高质量的 `AITaskJob` prompt，再通过聊天创建或更新 AI 固定例行事项。重点不是机械调用 `add_ai_task_job`，而是让长期运行的 job prompt 具备清晰目标、Kooboo 能力依据、执行流程、写入边界、异常处理和完成标准。

## 适用场景

用户表达以下意图时使用本 skill：

- 创建、更新、删除或查看 AI task job、固定例行事项、定时自动化、例行任务。
- 让站点每天/每周/定期自动发帖、同步内容、生成报告、巡检、清理、更新 SEO、处理商品或订单摘要。
- 把一句自然语言需求转成可长期自主执行的 Kooboo 自动化。

如果用户说的是 `Kooboo.Sites/Automation` 批处理引擎、图片压缩等站点 automation runner，而不是 AI task job，先说明区别，再根据用户选择继续。

## 核心流程

1. **理解自然语言目标**
   - 识别任务类型：发布、同步、巡检、报表、清理、更新、提醒或混合流程。
   - 判断是否会修改站点。写入型任务是允许的，但必须明确边界。

2. **澄清边界**
   - 只问创建 job 必需的问题，避免无谓来回。
   - 写入型任务至少确认：允许写什么、禁止写什么、目标对象/分类/目录、是否直接发布、去重策略、失败时跳过还是降级。
   - 如果用户已有明确答案，不要重复追问。

3. **Kooboo 能力 grounding**
   - 先调用 `get_site_info`，读取站点信息、可用 provider/model 和资源概况。
   - 如果任务涉及 Kooboo 站点开发、KScript、页面、内容、API、CodeBlock、网络请求或文件读写，先 `load_skill("kooboo-coding")`。
   - 根据场景读取必要 reference。示例：外部 HTTP 采集读取 `references/K-Script/net/httpclient.md`；文件型 SOP 读取 `references/K-Script/file/index.md`；函数参数不确定读取 `references/function-guide.md`。
   - 不要把用户的泛词直接写入 job prompt。应转换成 Kooboo 内真实能力、函数和术语，例如把“抓取接口数据”明确为使用 `k.net.httpClient` 或已发现的站点函数。

4. **编译 job prompt**
   - 生成的 prompt 必须是可执行 SOP，而不是复述用户原话。
   - prompt 应包含目标、权限边界、必要知识来源、执行步骤、质量门槛、去重/幂等策略、失败处理、`ai_task_finish` 结束规则。
   - 复杂 prompt 编译细节见 `references/prompt-compiler.md`。

5. **创建或更新 job**
   - 用 `function_discover` 查找 AI task job 相关函数。
   - 必须先 `use_function(["list_ai_task_jobs", "add_ai_task_job"])`，需要更新/删除时再读取 `update_ai_task_job` 或 `delete_ai_task_job` schema。
   - 先 `list_ai_task_jobs` 查重。同名已存在时，询问用户是更新、改名还是取消。
   - `Frequence` 使用真实 enum 名：`Day`、`Hour`、`Minutes`、`Week`、`Month`、`Second`。
   - `StartTime` 必须是 UTC。把用户的本地/相对时间转换清楚后再调用函数。
   - 调用 `add_ai_task_job` 前，简短展示将创建的 job 名称、首次执行时间、重复规则、模型、写入范围和 prompt 摘要，等待用户审批。

## 写入型自动化规则

写入型任务可以创建，但 job prompt 必须非常明确：

- 允许的写入动作，例如只创建新帖子、只更新指定内容文件、只修改指定 SEO 字段。
- 禁止的动作，例如不得删除历史内容、不得修改站点配置、不得覆盖未确认对象。
- 数据质量门槛，例如来源不足、去重命中、内容不合格、外部 API 失败时跳过。
- 人工审核要求，例如先生成草稿或只输出建议，不直接发布。
- 完成汇报，必须调用 `ai_task_finish`，用 `Success`、`Warning` 或 `Suggestion` 总结。

## 复杂 job 的外部 SOP

默认把完整任务说明写入 `AITaskJob.Prompt`。只有当任务流程较长、需要维护固定 runbook、包含较多示例/规则或会频繁调整时，才建议额外创建 kfile 或 content file 保存 SOP，再在 job prompt 中要求读取该文件。

外部 SOP 使用策略见 `references/complex-job-sop.md`。

## 创建前检查清单

调用 `add_ai_task_job` 前确认：

- 用户目标已经从口语化需求转换成专业 job prompt。
- 写入边界和禁止事项已写入 prompt。
- 需要的 Kooboo skill/reference 已读，不凭空编造 API。
- `Provider` 和 `Model` 来自 `get_site_info.aiInfos` 或用户明确指定的可用值。
- `Name` 不会意外覆盖已有 job。
- `StartTime` 是 UTC，`Frequence` 枚举合法。

## 完成回复

创建或更新成功后，只需简短说明：

- job 名称。
- 首次执行时间和重复规则。
- 它会做什么以及允许的写入边界。
- 如有外部 SOP 文件，说明 job prompt 会读取哪个路径。
