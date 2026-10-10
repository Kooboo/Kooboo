# Site Building Entry Flow

This entry is only for standalone public Sites, such as content sites, marketing sites, corporate sites, portfolio sites, and storefronts. ToB Apps / back-office systems must use [App Building](../app-building/index.md) and must not use this flow to create business resources.

## Entry Conditions

Before loading this document, confirm all of the following:

1. The user has explicitly authorized implementation in this round;
2. The site's business goal or the object to be changed can be identified;
3. The current conversation is not in an App Blueprint context.

If any condition is unmet, remain at the intent gate in `kooboo-coding/SKILL.md`: discuss or clarify only, do not load the UI planning Skill, and do not write site resources.

## Load on Demand

1. After reading this document, immediately read [Site Building State Protocol](concepts.md).
2. For S0–S3, read only [Planning Phase](planning.md).
3. Read [Static Delivery](static-delivery.md) only when actually entering S4/S5.
4. Read [Dynamic Delivery](dynamic-delivery.md) only when the user actively requests dynamicization or explicitly accepts a dynamicization recommendation.
5. After S2 generates the Page Plan, read [Capability Catalog](capability-catalog.md) to establish site milestones.
6. Read [Commerce Dynamicization Contract](commerce-dynamicization.md) only when commerce dynamicization is involved.

Do not preload static implementation, dynamicization, payment, Backend, or Commerce details merely because they might be needed later.

## State Transitions

| Current state | Next step | Execution lock |
|---|---|---|
| This document has been read | Read `concepts.md` and `planning.md` | Do not call planning or write functions before this |
| S0 ready | Use `ask_user_input` to choose P1/P2 | Do not silently default the starting mode |
| S1 mode selected | Use `update_todos` to establish S2 tasks | Do not combine S2/S3/S4 into one todo batch |
| S2 Page Plan complete | Read the capability catalog and call `update_site_milestones` | Do not enter S3/S4 before milestones exist |
| S3 Design System complete | Read `static-delivery.md` and switch S4 todos | Do not create resources before switching todos |
| Current S4/S5 batch complete | Run capability Preflight, complete todos and update the checklist with `update_site_milestones` | Deliver only the current Page Plan pages |
| User confirms dynamicization | Read `dynamic-delivery.md` | Do not enter S6–S10 without confirmation |
| Current-stage Preflight passes | Call `show_next_recommended_action` when a natural next step exists | A recommendation is not authorization for the next stage |

## Invariants Throughout the Flow

- Site Brief, Page Plan, Design System, static delivery, and dynamic delivery are separate stages; do not expand them in one todo.
- `pages` in the Page Plan is the current delivery batch. Establish the next batch's todos only after the current batch is complete.
- After each resource write, follow [Site Preflight](../../Preflight/site.md) and mark only the actually changed objects dirty. An explicit `mode: "focused"` batch Preflight may be used (dirty objects + direct consumers); when no authoritative dirty scope is connected, keep the default `mode: "capability"`. Run a full capability scan when the entire capability stage is complete. A structural PASS does not mean the business result is complete.
- Before creating or modifying an object, read the relevant Frontend, Backend, KScript, or Function reference as needed; do not guess field or function names.
- Unless explicitly requested, do not use emoji or invent business data, external image links, or interactions that do not exist.
