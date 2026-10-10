---
name: debug
description: Diagnose and fix problems in the current Kooboo site through an evidence-first workflow using Preview diagnostics, Network, Browser Control, source inspection, and Preflight. / 排查并修复当前 Kooboo 站点中的白屏、报错、网络失败、交互异常和渲染问题。
allowed-tools: read_preview_diagnostics read_preview_network browser_control navigate_iframe set_iframe_device observe_iframe observe_iframe_changes set_iframe_files find_iframe_elements inspect_iframe_element act_on_iframe_element wait_for_iframe
metadata:
  explicit-only: true
---

# Kooboo site debugging

The user explicitly selected `/debug` for this request. Treat it as an instruction to diagnose the described site problem through evidence, apply only an authorized minimal fix, and verify the same reproduction path.

## Scope before workflow

For an existing element's spacing, alignment, size, color, or ordering, use the existing-object repair path in `kooboo-coding`: inspect its current markup and loaded CSS, apply the authorized minimal fix, and verify that element. Do not load the full runtime triage references or App planning documents unless evidence reveals a runtime, resource-loading, or business-contract problem. An existing App Plan does not turn a local repair into Milestone implementation. When the change is purely presentational and preserves business behavior, do not read or validate Blueprint/App Plan or run App Preflight; use source readback and target UI verification as specified by `kooboo-coding`. The runtime workflow below applies only when the evidence requires it.

## Required workflow

1. Load `kooboo-coding` unless already in context. Reuse references and schemas already read; do not reload them between tool calls.
2. Read this skill's `references/triage.md`. When the live Preview is involved, also read `references/preview-runtime.md`. For HTTP 500, SSR/template failures, or blank pages without an identified client error, read `references/common-errors.md` and run the `kooboo-coding` Preflight script in the first evidence pass.
3. Establish the route, device, actions, expected result, and actual result from available context. Ask only when missing information would materially change the reproduction path.
4. Reproduce once through Preview when available, then read the smallest useful diagnostics. If a client error names a component or Script, immediately read its current source and relevant dependencies with `read_object_text` before additional documentation or Preflight. When the error location maps to the current site object source, read a window around that line and widen it as needed; generated or bundled code line numbers require a verified source mapping first. Use the window and patch rules in `kooboo-coding`’s `references/Functions/read-update.md`. This source-first path takes precedence over the general startup sequence in the Preview reference. Read Network when the symptom involves resources or data requests.
5. Reload at most once and wait only for a bounded interval. If Preview remains unavailable, returns `frame_not_ready`, reports `agentReady=false`, or Log/Network remains unavailable, stop browser retries and switch immediately to Preflight plus Page/Layout/View source inspection.
6. Confirm the root cause with matching runtime, network, Preflight, source, or configuration evidence before changing resources.
7. Apply one minimal fix based on the current source. If a patch cannot match its old text, reread the object and rebuild it; never retry guessed source. If the same error survives a repair, inspect the new source and failing expression before another patch. Do not combine unrelated cleanup or speculative patches.
8. Repeat the same route, device, inputs, and actions. Rerun relevant diagnostics and Preflight after the fix.

## Authorization boundary

- `/debug` changes the investigation workflow; it does not grant write permission by itself.
- Requests to analyze or explain remain read-only.
- Requests to diagnose and fix authorize the minimal repair after the root cause is confirmed.
- Obtain explicit confirmation before real payments, destructive actions, messages, account changes, or other business side effects.

## Completion

Report the exact outcome as one of: diagnosed, fixed and verified, changed but not fully verified, or blocked. Never claim success without observing the expected behavior through the same path.
