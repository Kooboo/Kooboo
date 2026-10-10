# Site Preflight

Use the Site profile for Site Building and ordinary object-level coding conversations that have not established the App profile. Once a conversation establishes App identity, an invalidated Plan binding requires App validation again.

The AI Chat adapter injects `profile: "site"`. Do not submit adapter-owned binding fields. Continue using the shared `taskType`, `layers`, `deliveryStage`, `mode`, and `artifacts` contract in [Preflight Core](index.md).

Site Preflight owns:

- Site Brief and Page Plan delivery stages;
- Site delivery stages and the currently Site-only k-data/Commerce-template checks;
- capability-selected assembly, navigation, multilingual, assets, Commerce and Payment checks shared with App, as described in [Capabilities](capabilities.md);
- Closeout checks and natural next-step recommendations.

A Site receipt contains `structuralReceipt.profile: "site"`. A structural PASS remains distinct from runtime and business evidence.
