# Kooboo Debug SOP

Unified fault-triage and repair entry point for Kooboo site development involving Layout / Page / View, template SSR, Script, KScript, and API / Service.

The goal is not to quickly offer a “possible cause”, but to complete an auditable loop:

> Clear symptom → minimal reproduction → evidence collection → confirmed root cause → minimal fix → same-path verification

When the fault occurs in the site preview on the right side of AI Chat, also read [Preview Runtime Debugging](preview-runtime.md). For blank pages, HTTP 500, template SSR, or Page / Layout / View rendering failures, also read [Kooboo Common Errors Quick Path](common-errors.md).

This reference handles diagnostic orchestration. When Kooboo object contracts or resource writes are involved, first load `kooboo-coding`, then use `read_skill_resource("kooboo-coding", "...")` to read the technical references linked below.

---

## 1. Scope and triggers

Any of the following establishes a fault context; follow this document before concluding a root cause or modifying resources:

- Build or site preview failure.
- HTTP 4xx / 5xx, network failure, timeout, or a request that remains pending.
- A clear error or stack in the browser console or server log.
- A blank page, unresponsive interaction, missing data, or a save that has no effect.
- Behavior that clearly conflicts with a confirmed requirement when the user's intent is to fix it rather than change the requirement.
- Explicit fault wording such as “broken”, “wrong”, “not working”, “error”, or “diagnose and fix”.
- The user explicitly selected the `/debug` skill for the current request.

The following do not require the full Debug loop:

- A pure copy change or localized visual issue (including incorrect spacing, alignment, size, color, or ordering). Use the scoped repair path in `kooboo-coding` and verify the target element; expand to runtime triage only when the evidence requires it.
- Normal feature development without fault semantics.
- The user only asks for an explanation of existing code.

When the boundary is unclear, collect read-only evidence first and do not modify anything.

---

## 2. Authorization boundary

`/debug` changes the focus of the current request; it does not expand write authorization automatically.

| User wording | Allowed behavior |
|---|---|
| “Analyze the cause” / “Help me see why” | Collect read-only evidence, locate, and explain; do not modify resources |
| “Diagnose and fix” / “Solve this problem” | After confirming the root cause, apply the minimal fix and verify it |
| Only `/debug` is selected without a concrete problem | Wait for the user to describe the problem; do not modify proactively |

For login submission, real order creation, payment, deletion, sending email/SMS, modifying users, or other business side effects, obtain explicit confirmation or use clearly identified test data before executing the side effect, even when the user authorized the repair.

---

## 3. Hard red lines

- Do not broadly rewrite without reproduction or error information.
- Do not treat a symptom as the root cause; for example, “the list is empty” does not mean the query is necessarily wrong.
- Do not stack multiple speculative patches; return to the evidence stage after a failed verification.
- Do not overwrite a Page / View / Layout / Script / Style / API / CodeBlock without reading the current object content with `read_object_text`. If `oldContent` is not found, reread that object before rebuilding the patch; do not retry guessed text or line numbers. After failed verification, inspect the current source together with the new error, not an earlier generated version.
- Do not treat generated or bundled code line numbers as site-object source positions without verifying the mapping. For a verified source location, start with a window covering the failing expression and its enclosing structure, and widen it when evidence is missing. Follow [Object reading and updating](../../kooboo-coding/references/Functions/read-update.md) for raw patch text and absolute line numbers. A partial read does not establish whole-object correctness.
- Do not manufacture database, order, cart, user, or login state just to make the UI look successful.
- Do not invent `k.*` APIs; verify every new or retained call in the relevant KScript reference.
- Do not invent `k.req.services` / `k.request.services`; API-to-Service calls must use the real `import` / `export` contract.
- Do not continuously push complete Log or Network records into the session; read only the minimum required by the current hypothesis.
- Do not leave temporary `AI_DEBUG`, `console.*`, or extra `k.logger` instrumentation after completion.
- Do not claim “fixed” without same-path verification.

---

## 4. Default troubleshooting loop

### 4.1 Establish the problem baseline

First determine:

1. Target URL / route.
2. Device or viewport.
3. The user's actions and inputs.
4. Expected result.
5. Actual result.
6. Whether it reproduces reliably.

When the available information is sufficient, proceed directly instead of repeating each question to the user. Ask only when missing information would materially change the reproduction path or introduce risk.

### 4.2 Minimal reproduction

- Reduce the reproduction path to the fewest steps that reliably trigger the symptom.
- When Preview is available, use Browser Control to automate safe steps first.
- For a blank page or first-screen load failure, reload once to capture startup Log and Network.
- For a blank page without an identified client error, HTTP 500, or template SSR failure, run Preflight during the first evidence pass and read the current Page, bound Layout, and mounted View; do not wait for the Preview Agent to recover before static inspection.
- When Preflight returns `knownErrors`, follow its `playbook` deterministic quick path before considering general hypotheses.
- For intermittent issues, preserve the same route, device, inputs, and action order; do not change multiple variables in each attempt.

### 4.3 Evidence collection

Choose evidence by symptom; do not mechanically read every tool:

| Symptom | Priority evidence |
|---|---|
| Blank page, JS crash, component not rendered | Preview Log issues, stack, related Script / View |
| Click has no response, dialog does not appear, state not updated | Browser Control replay, DOM changes, Preview Log |
| Data empty, save fails, endpoint error | Preview Network issues, server Error Log, API / Service source |
| Slow request, perpetual loading, duplicate requests | Preview Network all, pending, duration, initiator stack |
| Image / CSS / JS / font failure | Network resource type, Log resource error |
| Failure only on mobile or a specific size | Replay after setting the same device |
| KScript / CodeBlock error | Original server error, corresponding object source, KScript reference |

See [Preview Runtime Debugging](preview-runtime.md) for the exact Preview tool order and cursor/document switching semantics.

### 4.4 Confirm the root cause

Before changing anything, you should be able to form this chain:

```text
现象
→ 可重复的触发步骤
→ 直接运行证据
→ 能解释该证据的真实源码或配置
→ 一个可被修复和复验的根因
```

Direct evidence must include at least one of:

- A clear exception stack or server error.
- A failed, timed-out, or duplicate network request.
- A DOM state that differs from expected.
- A call path that can be matched across request, log, and source.
- A mismatch between the real response shape and the fields being read.

“It may be the cache”, “it may be the endpoint”, or “it may be the framework” is not a confirmed root cause.

### 4.5 Minimal fix

- First read the current object content. Reuse references already in context and load another only for an unresolved contract. A named client component error should lead directly to its Script, imports, setup return bindings, template, and relevant data shape, not another framework-documentation pass.
- Change only one evidence-supported root cause or one class of same-source issues at a time.
- Do not refactor unrelated resources at the same time.
- When parameters for an object-creation/update function are uncertain, read [Function Guide](../../kooboo-coding/references/function-guide.md) first.
- When modifying KScript, recheck every new or changed `k.*` call.
- If the existing evidence is still insufficient, add minimal temporary instrumentation:
  - Use a searchable `AI_DEBUG` prefix in the browser.
  - Use `k.logger` in server-side KScript; do not use `console.*`.
  - Remove it immediately after obtaining evidence.

### 4.6 Same-path verification

After the fix, repeat the following exactly as before the fix:

- The same URL.
- The same device.
- The same inputs.
- The same action order.
- The same wait conditions.

Completion requires all of the following:

1. The behavior expected by the user appears correctly.
2. The original fault no longer reproduces.
3. Related Log / Network / server errors are gone.
4. No obvious new regression was introduced.
5. Temporary instrumentation has been removed.

If any item cannot be completed, state which verification evidence is missing.

---

## 5. Kooboo fault map

### 5.1 Blank Preview, broken layout, or ineffective template binding

Check first:

- Run Preflight first; when `knownErrors` is hit, read [Kooboo Common Errors Quick Path](common-errors.md). `KOOB-CDATA-WRAPPER` is a high-confidence root cause for immediate handling.
- Whether `k-content`, `k-if`, `k-for`, `k-attribute`, and `<k-data>` are in the `env="server"` inheritance chain.
- Whether `import`, `type="module"`, and `export` are paired correctly in `<script env="server">`.
- Whether HTML tags, attribute quotes, and nesting are complete.
- Whether Layout / Page / View placeholders, references, and names match.
- Whether Network contains failed Script / Style / Image resources.

Continue by reading:

- [Template Binding Syntax](../../kooboo-coding/references/Frontend/template-binding-syntax.md)
- [Layout](../../kooboo-coding/references/Frontend/layout.md)
- [Page](../../kooboo-coding/references/Frontend/page.md)
- [View](../../kooboo-coding/references/Frontend/view.md)

### 5.2 KScript / CodeBlock errors

Check first:

- Whether the stack points to a specific object and line.
- Whether the server incorrectly uses `console.*`.
- The read and return contracts of `k.request` / `k.response`.
- Whether the `k.*` method actually exists.
- Whether the dynamic behavior should use `<k-data>` + `<query>`.
- Whether `undefined` / `is not a function` comes from an incorrect API path.

Before adding or modifying `k.*`:

1. Locate the module from [KScript Index](../../kooboo-coding/references/K-Script/index.md).
2. Search the module's subdocuments for the same call path.
3. If there is no match, treat the capability as undefined by the current documentation; do not invent a neighboring name to try.

### 5.3 API / Service / Data problems

Check first:

- URL, HTTP method, status code, and duration in Network.
- Whether `{action}`, the route, and the caller match.
- Whether the request trigger corresponds to the actual API.
- Whether API, Service, and Data layer responsibilities are mixed.
- Whether the return shape matches the fields read by the frontend.
- Whether permissions, data sources, or environment configuration have been mistaken for a code error.

Continue by reading:

- [Backend Guide](../../kooboo-coding/references/Backend/index.md)
- [API Layer Guide](../../kooboo-coding/references/Backend/api-layer-guide.md)
- [Service Layer Guide](../../kooboo-coding/references/Backend/service-layer-guide.md)
- [Data Layer Guide](../../kooboo-coding/references/Backend/data-layer-guide.md)
- [Function Guide](../../kooboo-coding/references/function-guide.md)

---

## 6. Distinguish the two kinds of Automation

### Preview Browser Control

`browser_control`, `observe_iframe`, `find_iframe_elements`, and `act_on_iframe_element` are used to reproduce and verify user actions in the Preview on the right; they are part of the main Debug path.

### Site Automation

`list_automation` / `execute_automation` list and execute existing site Automation tasks. Use them only when the fault itself belongs to an Automation task; `execute_automation` requires approval and cannot replace ordinary page actions or browser tests.

---

## 7. Status and delivery wording

Distinguish:

- **Reproduced**: the symptom has been observed reliably.
- **Root cause confirmed**: evidence and source form a closed loop.
- **Change completed**: resources have been updated, but same-path verification is not complete.
- **Fixed and verified**: same-path verification passed and related errors did not recur.
- **Cannot verify**: permissions, data, an external service, user action, or an environment condition is missing.
- **Blocked**: the current capabilities cannot obtain the necessary evidence.

The final result must at least state the root cause, changed objects, verification path, verification result, and remaining risks.
