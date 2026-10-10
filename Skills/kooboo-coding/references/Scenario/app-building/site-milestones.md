# Site Milestone checklist

SiteMilestones are a persisted delivery checklist kept as a progress record. They do not gate any tool, implementation, Preflight or closeout step, and whether a Blueprint/App Plan exists does not change how the checklist is read or saved. Planned App authoring still follows [App Plan](app-plan.md).

## Item contract

```json
{
  "content": "Customer management: list, create, edit and detail",
  "completed": false
}
```

Read the checklist with `get_site_info`. Save with `update_site_milestones` by sending the complete array; the save overwrites the stored list. To keep unrelated items, include them unchanged. Mark an item `completed=true` only when its delivery has been implemented and the verification you actually performed supports it.

## Choose and order batches

Keep coherent business capabilities as separate checklist items even when implementing them together.

Apply the same scope priority to lightweight and planned Apps:

1. Follow the user's explicit implementation scope first. Implement the named module or modules; when the user explicitly requests all modules, select and finish the entire requested batch without asking again. Reuse scope already established in the conversation.
2. When implementation is authorized but the current delivery scope is unspecified, default to a usable core business workflow and its necessary dependencies. Briefly state what this delivery includes and defers, then proceed; ask only when a material business ambiguity prevents choosing the core workflow. A core workflow may span multiple modules. A Dashboard alone qualifies only when it delivers the requested core business outcome.
3. Full implementation is an explicit option, not the fallback for unspecified scope. A broad request such as "build an inventory system", a complete Blueprint/App Plan, or a module checklist does not by itself select all modules for this delivery. Preserve the overall scope and remaining items, but close out after the default core batch without automatically starting them.

Follow the selected data strategies in every batch. A planning-only request still ends after planning; a narrowly scoped repair does not authorize other modules. Apply the core default only when scope is unspecified, never to reduce an explicit request or rebuild an existing completed core.

## Implement and record

1. Read the checklist and the relevant current planning. Decide the batch the user requested.
2. Create concrete Todos for the authorized outcomes. Inspect actual source and resources, and resolve any real dependency before writing code that relies on it.
3. Implement the selected capabilities and required shared resources. Keep future routes as explicit placeholders; each implemented route needs its actual Screen and behavior. Installation or navigation inventory alone is not delivery. Follow the agreed data strategy; mock writes do not claim persistence.
4. Run the Preflight checks that fit the change, then compare the goals with the actual resources and behavior. Partial or unverified outcomes keep their Todos incomplete.
5. Save the complete checklist with `update_site_milestones`, marking only the items that are actually done as `completed=true`. If runtime evidence is unavailable, record the progress and state that verification is pending.
6. Continue any remaining authorized work. Save relevant site memory, reconcile Todos, then provide a final summary that reports completed and pending items separately.

## Runtime verification

Preview `authentication-required`, `loading` and `unavailable` mean runtime verification is pending. Do not poll unchanged results or require Preview to be opened before continuing implementation. A login-page audit is not evidence for the protected business page; after login and return to the business page, collect fresh diagnostics. Browser operation remains opt-in. Actual failed writes, incompatible dependencies, known runtime errors and structural failures still require repair; do not label them merely pending login.

Closeout may also record already-discussed user decisions with `update_planning_topic` and display an application link with `show_action_button`. A Topic is a discussion record, not a Blueprint/App Plan change or new implementation authorization. Do not invent requirements or confirmations during closeout.
