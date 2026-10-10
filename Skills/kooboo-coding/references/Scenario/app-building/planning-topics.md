# Site planning topics

Blueprint records adopted business definitions. App Plan records implementation design. Keep assumptions, open questions, suggestions and user answers in site-level planning topics, using the existing skill scripts and `k.file`.

## Read and write

- `read_planning_topics()` returns bounded summaries with `total` and `nextCursor`. Pass that cursor to continue. Use `ids` for details; `status` and `artifact` optionally filter the list. An artifact filter includes site-wide topics.
- Pagination: `cursor` is a nonnegative integer, default `0`; `maxItems` is an integer from `1` to `30`, default `20`. First-call arguments: `{ "cursor": 0, "maxItems": 20 }`. Keep the same filters on subsequent pages and use the returned `nextCursor`; stop when it is `null`.
- `update_planning_topic(item, expectedUpdatedAt)` creates or replaces one complete item by stable ID. For an existing ID, read its details and copy the response `updatedAt` into `expectedUpdatedAt`. Omit the token only for a new ID. On `TOPICS_CHANGED`, reread and merge your intended changes while preserving the current answer, confirmation evidence and unrelated fields; do not blindly retry the old item. Reuse the same ID when the question is answered. Do not delete or recreate the Blueprint/App Plan to record discussion.
- Use ordinary conversation or `ask_user_input` to clarify questions. Describe the question, suggestion and answer in business language. The storage format is not a user-facing confirmation UI.

## Item

| Field | Meaning |
| --- | --- |
| `id` | Stable discussion ID |
| `kind` | `assumption` or `open-question` |
| `question`, `context` | Business question and why it matters |
| `suggestion` | Proposed answer or working assumption, nullable |
| `options[]` | Optional choices, each with `id`, `label`, `description` |
| `answer` | Recorded answer, nullable |
| `status` | `open`, `answered`, `confirmed`, `dismissed` |
| `confirmationEvidence` | Explicit user confirmation/dismissal and its context; otherwise null |
| `impact` | `low`, `medium`, `high`; for discussion priority |
| `resolveBefore` | `app-plan-creation`, `implementation`, `production-release` |
| `scope` | `artifact` (`site`, `blueprint`, `app-plan`), `relatedIds[]`, `milestoneIds[]` |

An assumption is a suggested answer to a question, not an adopted fact. For example: question “Does the first release serve multiple teams?”, suggestion “One team initially”. Only record confirmation after the user explicitly agrees. `answered` remains unconfirmed. `dismissed` records the user's explicit decision that the question no longer applies. If the question or proposed answer changes, reopen it and clear stale confirmation evidence until the user confirms again.

Related IDs provide navigation and may refer to objects not planned yet; they are not foreign keys or copies of those objects. `milestoneIds` only records which SiteMilestone items the topic relates to. It does not block implementation or change any gate, and Topics never bypass the App Plan creation requirements.

## Readiness and authority

Unconfirmed topics block their named stage and later stages, regardless of priority. Blueprint validation and App Plan creation check `app-plan-creation`; App Plan implementation readiness also checks `implementation` and the target Milestone. Read `blockingPlanningTopicIds` and use `read_planning_topics(ids=...)` to discuss them. If `planningTopicsAvailable=false`, retry the read; do not infer there are no questions. Before release, explicitly read and resolve outstanding `production-release` topics.

Confirmation is a discussion record, not an approval token or permission to implement. It does not apply changes to Blueprint or App Plan. Update adopted planning documents separately through their existing tools and only within the user's authorization. Choose P1/P2 only for initial App creation on a blank Site; Topic discussions and incremental changes to an existing App continue from its current planning; do not create blocking questions for every safe, reversible P1 default.

Storage: `_ai/site-development/planning-topics.json`. Topics are site-scoped and remain readable independently of conversation or planning version. No separate version history or migration of test App data is introduced.

`scope.milestoneIds` records the related SiteMilestone IDs from `get_site_info` for reference only; it does not block implementation. Do not copy Topics into the item content or App Plan.
