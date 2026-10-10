# App Preflight

Preflight checks actual written resources and delivery evidence. App Plan persistence and design reads are not implementation authorization or acceptance.

## Planned Apps

Before scanning, read the current server Plan with `read_app_plan({})`. The adapter selects `profile=app` and injects `appPlanId` plus a session-local `scopeKey`; do not construct these arguments yourself. The scanner rereads the server Plan and checks its identity before loading rules.

After a successful App Plan save/delete or Blueprint save, read the Plan again before scanning. The Ledger invalidates prior structural and runtime/business evidence; failed or empty App Plan writes preserve it. An unchanged Plan read retains the current evidence context. The scopeKey identifies the current verification context inside this conversation, not a persisted Plan version or business delivery scope. Receipts from before a planning change are rejected even when appPlanId remains the same.

Resource writes invalidate affected structural coverage and runtime/business evidence. Include all changed resources and direct consumers. Use focused checks after a coherent write batch and a capability check covering the entire selected scope before completion; the adapter adds tracked artifacts. A matching current receipt proves only its scan results, not full business coverage. Reconcile selected Screens, Actions, Service operations, DataContracts and actual data behavior separately.

## Apps without planning

Apps without a Blueprint/App Plan use the shared site structural profile selected by the adapter; this does not switch them to Site Building. The SiteMilestone checklist is a progress record only and is not evidence for or against a scan. See [SiteMilestones](../Scenario/app-building/site-milestones.md).

## Evidence and recovery

Follow [Preflight Core](index.md) for invocation and resource coverage. Missing or mismatched Plan reads stop the scan; correct the read and scan again without fabricating identifiers or changing the profile. Required runtime/business observations remain separate evidence. Browser operation is opt-in; unavailable evidence remains pending. A structural PASS alone never establishes tested behavior.

Planning-only work needs no resource scan and does not trigger automatic implementation. Business-unrelated simple UI changes use the router's local repair exception, without Plan reads or binding restoration; local verification does not establish overall App completion.
