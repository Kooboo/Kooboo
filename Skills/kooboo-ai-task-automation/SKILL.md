---
name: kooboo-ai-task-automation
description: Use when the user wants to create or refine a Kooboo AI Task Job, fixed routine, scheduled task, or chat-created automation from natural language, including write-capable jobs such as scheduled posting, content sync, cleanup, SEO updates, reports, and inspections.
---

# Kooboo AI Task Automation

Compile the user's natural-language automation request into a high-quality `AITaskJob` prompt, then create or update an AI scheduled routine through chat. The goal is not to mechanically call `add_ai_task_job`, but to make the long-running job prompt include a clear objective, grounded Kooboo capabilities, an execution flow, write boundaries, error handling, and completion criteria.

## Use cases

Use this skill when the user expresses any of the following intentions:

- Create, update, delete, or view an AI task job, fixed routine, scheduled automation, or recurring task.
- Have the site automatically publish posts, synchronize content, generate reports, run inspections, clean up, update SEO, or process product or order summaries daily, weekly, or on another schedule.
- Turn a natural-language request into Kooboo automation that can run autonomously over time.

If the user means the `Kooboo.Sites/Automation` batch engine, image compression, or another site automation runner rather than an AI task job, explain the distinction first, then continue according to the user's choice.

## Core workflow

1. **Understand the natural-language objective**
   - Identify the task type: publishing, synchronization, inspection, reporting, cleanup, updating, reminders, or a mixed flow.
   - Determine whether it will modify the site. Write-capable tasks are allowed, but their boundaries must be explicit.

2. **Clarify boundaries**
   - Ask only the questions required to create the job; avoid unnecessary back-and-forth.
   - For write-capable tasks, at minimum confirm what may be written, what is forbidden, the target objects/categories/directories, whether to publish directly, the deduplication strategy, and whether failures should be skipped or degraded.
   - Do not ask again when the user has already provided a clear answer.

3. **Ground in Kooboo capabilities**
   - Call `get_site_info` first to read site information, available providers/models, and the resource overview.
   - If the task involves Kooboo site development, KScript, pages, content, APIs, CodeBlocks, network requests, or file I/O, call `load_skill("kooboo-coding")` first.
   - Read the references required by the scenario. For example, read `references/K-Script/net/httpclient.md` for external HTTP collection, `references/K-Script/file/index.md` for a file-based SOP, and `references/function-guide.md` when function parameters are uncertain.
   - Do not put the user's generic wording directly into the job prompt. Convert it into real Kooboo capabilities, functions, and terminology; for example, make “fetch interface data” explicit as using `k.net.httpClient` or a discovered site function.

4. **Compile the job prompt**
   - The generated prompt must be an executable SOP, not a restatement of the user's words.
   - The prompt should include the objective, permission boundaries, required knowledge sources, execution steps, quality gates, deduplication/idempotency strategy, failure handling, and the `ai_task_finish` completion rule.
   - See `references/prompt-compiler.md` for complex prompt compilation details.

5. **Create or update the job**
   - Use `function_discover` to find the AI task job functions.
   - First call `use_function(["list_ai_task_jobs", "add_ai_task_job"])`; read the `update_ai_task_job` or `delete_ai_task_job` schema only when an update or deletion is needed.
   - Call `list_ai_task_jobs` first to check for duplicates. If the same name exists, ask whether the user wants to update it, rename it, or cancel.
   - Use the real enum names for `Frequence`: `Day`, `Hour`, `Minutes`, `Week`, `Month`, `Second`.
   - `StartTime` must be UTC. Convert the user's local or relative time clearly before calling the function.
   - Before calling `add_ai_task_job`, briefly show the job name, first execution time, repeat rule, model, write scope, and prompt summary that will be created, then wait for user approval.

## Rules for write-capable automation

Write-capable tasks may be created, but the job prompt must be explicit about:

- Allowed write actions, such as creating only new posts, updating only specified content files, or changing only specified SEO fields.
- Forbidden actions, such as deleting historical content, changing site configuration, or overwriting unconfirmed objects.
- Data quality gates, such as skipping when sources are insufficient, deduplication matches, content is inadequate, or an external API fails.
- Human review requirements, such as generating a draft or suggestions first instead of publishing directly.
- Completion reporting, which must call `ai_task_finish` and summarize with `Success`, `Warning`, or `Suggestion`.

## External SOP for complex jobs

By default, put the complete task description in `AITaskJob.Prompt`. Create an additional kfile or content file to store the SOP only when the flow is long, a fixed runbook must be maintained, it contains many examples/rules, or it will change frequently; then require the job prompt to read that file.

See `references/complex-job-sop.md` for the external SOP strategy.

## Pre-creation checklist

Before calling `add_ai_task_job`, confirm that:

- The user's objective has been converted from an informal request into a professional job prompt.
- Write boundaries and forbidden actions are included in the prompt.
- Required Kooboo skills/references have been read; no API is invented.
- `Provider` and `Model` come from `get_site_info.aiInfos` or from values the user explicitly identified as available.
- `Name` will not accidentally overwrite an existing job.
- `StartTime` is UTC and the `Frequence` enum is valid.

## Completion response

After a successful creation or update, briefly state:

- The job name.
- The first execution time and repeat rule.
- What it does and its allowed write boundaries.
- If there is an external SOP file, which path the job prompt will read.
