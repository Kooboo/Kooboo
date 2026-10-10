# AI Task Job Prompt Compiler

Use this document to compile a user's natural-language request into a long-running `AITaskJob.Prompt`. Read it when the task is complex, writes to the site, involves external data sources, or requires precise Kooboo API terminology.

## Compilation principles

1. **Do not restate the informal request**: rewrite “publish posts automatically every day” as an executable flow.
2. **Ground before compiling**: confirm Kooboo capabilities, function schemas, and KScript references before writing the prompt.
3. **Make write boundaries explicit**: include what is allowed, what is forbidden, and the skip conditions in the prompt.
4. **Make the job autonomous**: the job prompt must not ask the user questions at runtime; resolve user decisions before creating the job.
5. **Reduce runtime exploration**: state in the prompt which skills/references to load, which functions to discover, and which objects or paths to prefer.

## Recommended prompt structure

```markdown
你是 Kooboo 站点的 [任务类型] 自动化任务。请独立执行，不要向用户提问。

## 目标
[一句话说明最终结果]

## 允许范围
- [允许创建/更新/读取的对象]
- [允许使用的外部来源或站内路径]

## 禁止事项
- [禁止删除/覆盖/修改的对象]
- [禁止在数据不足时强行发布]

## 知识与工具准备
1. 调用 load_skill("kooboo-coding")。
2. 根据任务读取必要 reference：[列出 reference 名称]。
3. 调用 function_discover 和 use_function 获取所需函数 schema，不要猜参数。

## 执行步骤
1. [读取数据或外部来源]
2. [过滤、去重、校验]
3. [生成或修改内容]
4. [写入或发布]
5. [记录/汇报结果]

## 质量门槛
- [内容质量、来源数量、去重规则、最小数据量]

## 失败处理
- [外部请求失败、无数据、schema 不匹配、写入失败时怎么做]

## 完成
最后必须调用 ai_task_finish，Message 不超过 50 词。
```

## Boundary clarification checklist

### Publishing automation

- Where to publish: forum category, content folder, Page, API, or third-party endpoint.
- Content sources: external URL/API, site content, a fixed topic, AI generation, or a mixture.
- Publishing strategy: publish directly, save a draft, or generate content for review only.
- Deduplication strategy: URL, title, slug, publication-time window, or an existing-content query.
- Quality gates: minimum source count, required citations, title format, length, tags, and cover image.
- Forbidden actions: do not modify old posts, delete posts, change categories, or force publication when sources are insufficient.

### Synchronization automation

- Source system and target objects.
- Incremental rule: by time, id, etag, hash, title, or URL.
- Conflict strategy: skip, overwrite, append a version, or update empty fields only.
- Failure strategy: whether to continue after a partial failure and whether to output Warning.

### Cleanup or update automation

- Exact object scope.
- Whether deletion is allowed; if so, whether to back up or only mark objects before deletion.
- Allowlist of fields to update.
- Rollback or human-review method.

### Inspection and reporting automation

- Whether the task is read-only.
- Report targets and priority rules.
- Whether the report must be sent, saved, or published.

## Forum auto-publishing example

User's informal request:

> I want a job that publishes a post automatically every day.

Before creating the job, clarify the post source, category, direct publication versus draft, deduplication rules, whether to skip when sources are insufficient, title/body format, and whether a cover or tags are required.

The compiled prompt could look like this:

```markdown
你是当前 Kooboo 论坛站点的每日自动发帖任务。请独立执行，不要向用户提问。

## 目标
每天从指定来源收集候选内容，生成一篇符合论坛规范的新帖子，并发布到指定分类。

## 允许范围
- 只允许创建一篇新帖子或草稿。
- 只允许读取必要的站点对象、内容、标签和外部来源。
- 如果需要访问外部 API 或网页，先读取 Kooboo KScript 网络请求文档，并优先使用 k.net.httpClient。

## 禁止事项
- 禁止修改或删除历史帖子。
- 禁止修改站点配置、用户、权限、分类结构。
- 来源不足、重复或内容质量不达标时禁止强行发布。

## 知识与工具准备
1. 调用 load_skill("kooboo-coding")。
2. 若需要外部 HTTP 请求，读取 references/K-Script/index.md 和 references/K-Script/net/httpclient.md。
3. 调用 function_discover，并用 use_function 获取发帖、内容读取、对象读取等函数 schema。
4. 不要猜测函数参数；没有可用发帖函数时，用 Warning 结束并说明缺少能力。

## 执行步骤
1. 读取指定来源，收集候选条目。
2. 按 URL 和标题检查重复内容。
3. 过滤无来源、低质量或重复条目。
4. 生成帖子标题、正文、引用来源和标签。
5. 写入前再次确认目标分类存在。
6. 创建新帖子或草稿。

## 失败处理
- 外部来源不可访问时，用 Warning 结束。
- 有效候选不足时跳过发布，用 Suggestion 汇报原因。
- 写入失败时不要重试创建重复帖子，用 Warning 汇报错误。

## 完成
最后必须调用 ai_task_finish，Message 不超过 50 词，说明已发布、已跳过或需要人工处理。
```

## Terminology conversion examples

| User wording | More precise expression in the prompt |
| --- | --- |
| Fetch interface data | Read the `kooboo-coding` KScript net/httpClient documentation, then use `k.net.httpClient` to make an external HTTP request |
| Publish posts automatically | Discover and use the current site's available post/content-creation function; if none is available, finish explicitly and report that the post function is missing |
| Do not duplicate | Perform an idempotency check by URL, title, slug, or an existing-content query |
| Run daily | `Repeat=true`, `Frequence=Day`, `FrequenceUnit=1` |
| Let me review it first | Create a draft or save content for review; do not publish directly |
