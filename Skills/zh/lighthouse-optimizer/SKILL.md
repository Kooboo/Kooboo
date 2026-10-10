---
name: lighthouse-optimizer
description: 分析 Kooboo 站点的 Lighthouse 或 PageSpeed 结果，将失败 audits 及 DOM/资源证据定位到 Page、View、Layout、Script、Style、Image、API 或 CodeBlock，生成按优先级排列的优化建议，并且只在用户明确授权后实施修复和执行 Preview、Preflight 验证。用于 Lighthouse、Core Web Vitals、performance、accessibility、SEO、best practices 优化，以及只读的定时 Lighthouse 巡检。
---

# Lighthouse 优化

使用内置检查脚本，把 Kooboo 现有的 Lighthouse 报告转换成精简的结构化证据。禁止把原始 Lighthouse JSON 发送给模型。

## 必须执行的流程

1. 确定页面的绝对公开 URL。当前站点应调用 `get_site_info` 并使用 `SiteSettings.BaseUrl`，除非用户提供了更具体的页面。
   - 只允许能被远程 Lighthouse provider 访问的公开 `http://` 或 `https://` URL。
   - 禁止使用 `localhost`、`localkooboo.com`、单标签内网主机名、`.local`/`.lan`/`.internal` 域名、loopback、private、link-local 或 reserved IP。
   - BaseUrl 不符合要求时直接停止并说明原因，不得替换、猜测或尝试本地 URL。
2. 通过 `run_skill_script` 运行 `scripts/inspect-lighthouse.js`：

   ```json
   {
     "skillName": "lighthouse-optimizer",
     "scriptName": "scripts/inspect-lighthouse.js",
     "arguments": {
       "url": "https://example.com/",
       "mobileDevice": true,
       "maxIssues": 15,
       "maxItemsPerAudit": 5,
       "createReportUrl": false
     }
   }
   ```

3. 以返回的 `formFactor` 为准，不要根据请求中的 boolean 推断实际测试设备。
4. 如果结果不是 `success`，返回原始错误并停止。禁止编造 Lighthouse 发现。
5. 在将问题定位到 Kooboo 对象或提出修改建议前，读取 `references/audit-playbook.md`。
6. 调用发现的函数前，先使用 `function_discover` 和 `use_function`。定位源码时优先使用以下只读函数：
   - `list_site_object`
   - `search_site_object`
   - `search_code`
   - `read_object_text`
   - `read_preview_element_context`
7. 每个结论至少关联一种证据：Audit ID、数值指标、estimated savings、资源 URL、DOM selector/snippet 或对象源码。
8. 输出按优先级排列的人工决策清单，包含 audit、证据、可能相关的 Kooboo 对象、预期收益、风险，以及是否仍需人工确认。
9. 默认设置 `createReportUrl=false`。未经确认可以正常打开并渲染 Lighthouse UI，不得生成或输出“完整报告”链接，也不要输出空的“完整报告”栏目。

## 授权边界

- 分析、巡检、解释和定时任务执行必须保持只读。
- 不得仅因为 Lighthouse 报告失败项就修改站点。
- 用户明确选择需要修复的问题前，禁止调用 create、update、delete、图片修改、设置修改、publish 或任意 `run_kscript` 函数。
- “分析”“扫描”“检查”“告诉我如何优化”等请求不代表修复授权。
- “修复 P1 和 P2”等请求只授权处理用户选中的问题，并且只能实施有证据支持的最小修改。

## 修复流程

获得明确修复授权后：

1. 加载 `kooboo-coding`，并读取受影响对象类型对应的 references。
2. 读取每个目标对象的最新源码。不得只根据 Lighthouse snippet 直接编辑。
3. 说明计划实施的具体修改、受影响对象、预期改善的指标，以及功能或视觉风险。
4. 使用现有 typed create/update/image 函数；禁止通过 `run_kscript` 重新实现这些能力。
5. 回读修改后的对象。
6. 对修改的对象执行有针对性的 Preflight。
7. 刷新 Preview，检查相关 DOM、视觉表现、diagnostics 和 Network 证据。
8. 适合重新远程测量时，再次运行 `scripts/inspect-lighthouse.js`。只比较实际返回的指标，禁止承诺目标分数。

如果 Lighthouse 修复方案与业务需求、analytics、advertising、authentication、payment、accessibility semantics 或现有视觉设计冲突，停止修改并让用户决定取舍。

## 定时任务模式

在 AI Task Job 中运行时：

- 对配置页面运行一次检查脚本。
- 只读取足以判断高优先级问题归属的站点对象。
- 禁止修改、创建、删除、publish 或执行任意站点代码。
- 运行期间不要向用户提问。
- 最后调用 `ai_task_finish`：确认存在 P1/P2 或扫描不完整时使用 `Warning`，只有低风险优化机会时使用 `Suggestion`，没有明显问题时使用 `Success`。

## 输出格式

每个可处理问题应包含：

- 优先级和 Lighthouse Audit ID
- 当前 metric、score 或 estimated savings
- 资源 URL 或 DOM 证据
- 可能相关的 Kooboo 对象及归属理由
- 建议修改方向
- 预期收益和风险
- 状态：已确认、可能相关或需人工复核

只有用户明确要求生成报告链接，并且已经确认当前环境的 Lighthouse UI 可以正常渲染时，才允许设置 `createReportUrl=true` 并输出 `reportUrl`。链接无法验证或预览为空时，直接从回答中省略链接，不要把不可用链接交给用户。
