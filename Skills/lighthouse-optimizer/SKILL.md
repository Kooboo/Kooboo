---
name: lighthouse-optimizer
description: Analyze Lighthouse or PageSpeed results for a Kooboo site, map failed audits and DOM/resource evidence to Pages, Views, Layouts, Scripts, Styles, Images, APIs, or CodeBlocks, produce prioritized optimization recommendations, and implement fixes with Preview and Preflight verification only after explicit user authorization. Use for Lighthouse, Core Web Vitals, performance, accessibility, SEO, best-practices optimization, and read-only scheduled Lighthouse inspections.
---

# Lighthouse Optimization

Use the built-in inspection script to convert an existing Lighthouse report for a Kooboo site into concise, structured evidence. Never send raw Lighthouse JSON to the model.

## Mandatory Workflow

1. Determine the page's absolute public URL. For the current site, call `get_site_info` and use `SiteSettings.BaseUrl` unless the user provided a more specific page.
   - Only public `http://` or `https://` URLs reachable by the remote Lighthouse provider are allowed.
   - Do not use `localhost`, `localkooboo.com`, single-label intranet hostnames, `.local`/`.lan`/`.internal` domains, or loopback, private, link-local, or reserved IP addresses.
   - If BaseUrl does not meet these requirements, stop and explain why. Do not substitute, guess, or try a local URL.
2. Run `scripts/inspect-lighthouse.js` through `run_skill_script`:

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

3. Treat the returned `formFactor` as authoritative. Do not infer the actual test device from the request boolean.
4. If the result is not `success`, return the original error and stop. Never fabricate Lighthouse findings.
5. Read `references/audit-playbook.md` before mapping an issue to a Kooboo object or proposing changes.
6. Before invoking a discovered function, use `function_discover` and `use_function`. Prefer the following read-only functions when locating source code:
   - `list_site_object`
   - `search_site_object`
   - `search_code`
   - `read_object_text`
   - `read_preview_element_context`
7. Associate every conclusion with at least one form of evidence: an Audit ID, numeric metric, estimated savings, resource URL, DOM selector/snippet, or object source.
8. Return a prioritized human decision list containing the audit, evidence, potentially related Kooboo objects, expected benefit, risk, and whether human confirmation is still required.
9. Set `createReportUrl=false` by default. Until the Lighthouse UI has been confirmed to open and render correctly, do not generate or output a "full report" link, and do not output an empty "full report" section.

## Authorization Boundaries

- Analysis, inspection, explanation, and scheduled task execution must remain read-only.
- Do not modify the site merely because a Lighthouse report contains failed audits.
- Until the user explicitly selects issues to fix, do not call create, update, delete, image modification, settings modification, publish, or any `run_kscript` function.
- Requests such as "analyze," "scan," "inspect," or "tell me how to optimize" do not authorize fixes.
- A request such as "fix P1 and P2" authorizes only the issues selected by the user, and only the smallest changes supported by evidence.

## Fix Workflow

After receiving explicit authorization to fix:

1. Load `kooboo-coding` and read the references for the affected object types.
2. Read the latest source for every target object. Do not edit directly from a Lighthouse snippet alone.
3. Explain the exact proposed changes, affected objects, metrics expected to improve, and functional or visual risks.
4. Use the existing typed create/update/image functions. Do not reimplement those capabilities through `run_kscript`.
5. Read back the modified objects.
6. Run focused Preflight checks for the modified objects.
7. Refresh Preview and inspect the relevant DOM, visual rendering, diagnostics, and Network evidence.
8. When another remote measurement is appropriate, run `scripts/inspect-lighthouse.js` again. Compare only metrics actually returned, and never promise a target score.

If a Lighthouse fix conflicts with business requirements, analytics, advertising, authentication, payment, accessibility semantics, or the existing visual design, stop modifying and let the user decide the tradeoff.

## Scheduled Task Mode

When running in an AI Task Job:

- Run the inspection script once for each configured page.
- Read only enough site objects to determine ownership of high-priority issues.
- Do not modify, create, delete, publish, or execute any site code.
- Do not ask the user questions during execution.
- Finally call `ai_task_finish`: use `Warning` when P1/P2 issues are confirmed or the scan is incomplete, `Suggestion` when only low-risk optimization opportunities exist, and `Success` when there are no obvious issues.

## Output Format

Each actionable issue should include:

- Priority and Lighthouse Audit ID
- Current metric, score, or estimated savings
- Resource URL or DOM evidence
- Potentially related Kooboo objects and the ownership rationale
- Recommended direction of change
- Expected benefit and risk
- Status: confirmed, potentially related, or requires human review

Set `createReportUrl=true` and output `reportUrl` only when the user explicitly requests a report link and the Lighthouse UI has been confirmed to render correctly in the current environment. If the link cannot be verified or the preview is empty, omit it from the response instead of giving the user an unusable link.
