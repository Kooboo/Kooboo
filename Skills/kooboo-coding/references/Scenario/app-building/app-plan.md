# App Plan authoring

Use for reviewed Blueprint semantics and user-authorized implementation planning. P1 fast direct delivery follows [lightweight implementation](lightweight.md) without creating Blueprint or App Plan.

## Read and discover tools

Call `read_app_plan({})` to read the complete current server Plan: `exists` and `plan`. Reading never creates a Plan. Reuse returned IDs and definitions. The strongly typed aggregate contains `id`, `blueprintId`, `settings`, `sourceFiles`, `dataContracts`, `services`, `screens`, `deliverySlices`.

Load each needed Native AI Function and its schema through `use_function` before calling it:

| Function | Input | Purpose |
| --- | --- | --- |
| `read_app_plan` | `{}` | Complete current Plan |
| `save_source_file` | `sourceFiles: [...]` | Resource filenames/directories |
| `save_data_contract` | `dataContracts: [...]` | Shared input/output shapes |
| `save_service` | `services: [...]` | Server capabilities and operations |
| `save_screen` | `screens: [...]` | Interfaces, blocks and actions |
| `save_delivery_slice` | `deliverySlices: [...]` | Coherent deliverable scopes |
| `save_app_settings` | `settings: {...}` | Design, layout, navigation |
| `delete_app_plan_objects` | `targets: [{type,id}, ...]` | Explicit deletion |

Functions directly persist server models. Follow their typed schemas; do not invoke a Skill Script or write files to store planning.

## Save boundaries

- Review design within the authorized scope before saving. Writes are approval-gated; reuse authorization for unchanged reviewed scope. Planning saves do not authorize resource implementation.
- The first successful save automatically initializes a Plan and requires an existing Blueprint. No separate root creation is needed. Reads, failed validation and empty batches do not initialize it.
- Submit one or more complete objects of one type. A stable ID replaces that entire object and its nested definitions. Preserve unchanged fields from current reads. Omitted optional fields in submitted objects are cleared; unsubmitted objects remain. Empty batches do not write or delete; null collections are invalid.
- References must exist already or in the same-type batch. Save dependencies first; mutually referring Screens or nested contracts may be saved together, but contract cycles are invalid. Use SourceFiles → DataContracts → Services → Screens → DeliverySlices → Settings with navigation. Settings can be saved earlier with empty navigation and completely replaced once routed Screens exist. Do not invent dummy objects or strip intended references to meet this order.
- Each batch is validated as a whole before one aggregate write; invalid input rejects the batch. Use coherent manageable batches, not partial-acceptance assumptions.
- Writes return `persisted`, `appPlanId`, `ids`; empty batches return `persisted=false`. Read back after the authorized batch and compare with the reviewed design.
- Fix objects/references named by validation errors. If storage outcome is uncertain, read actual state before retrying; do not regenerate the entire Plan or replace stable IDs.

## Object fields and meaning

Plan IDs, including nested fields, operations, blocks, actions and navigation items, are globally unique stable ASCII IDs (e.g. `screen.requests`) or GUID strings. Blueprint links use actual GUIDs from `read_app_blueprint({})`, never label-derived guesses. Optional collections default to empty.

| Object | Fields |
| --- | --- |
| Settings | `designSystem: {id,name}, layout: {pattern}, navigation[]` |
| Navigation item | `id, label, screenId?, children[]` |
| SourceFile | `id, type, fileName, path` |
| DataContract | `id, name, description, fields[]` |
| DataField | `id, name, description?, type, required, nullable, objectContractId?, sourceRefs[]` |
| SourceRef | `type, id` |
| Service | `id, name, description, sourceFileIds[], operations[]` |
| Operation | `id, name, description, scenarioIds[], businessRuleIds[], inputContractId?, outputContractId?, api?: {method,path}` |
| Screen | `id, name, purpose, routePath?, scenarioIds[], dataContractIds[], sourceFileIds[], accessRuleIds[], blocks[], actions[]` |
| Block | `id, kind, title?, description, dataContractId?, fieldIds[], actionIds[]` |
| Action | `id, label, description, scenarioIds[], serviceOperationId?, targetBlockId?, targetScreenId?, presentation?, confirmationMessage?` |
| DeliverySlice | `id, name, goal, scenarioIds[], screenIds[], serviceIds[], sourceFileIds[]` |

- **Settings:** select exact catalog design ID/name through [design-system selection](../../Frontend/app-design-systems.md). Layout pattern describes Shell composition. Navigation contains chosen business entry points, not all Screens. Leaves need routed Screens; groups without a Screen need children.
- **SourceFile:** type is `script | style | page | api | codeblock`. fileName includes its extension, without directories. path is a relative logical directory without the filename; empty means resource root. Example: `{id:"source.requests",type:"script",fileName:"screen.requests.js",path:"screens"}`. Several objects may share a file; a file plan neither creates a resource nor proves it exists.
- **DataContract:** shared request/response or display/input shape, not physical storage or executable transformation. Field name is the actual property. Type is `text | integer | decimal | boolean | date | datetime | object` or the corresponding `[]` variant. required means the property must appear; nullable means the whole value may be null. Array elements are non-null. objectContractId is required only for object/object[], and contracts cannot form cycles.
- **SourceRef:** type is `businessField | stateDimension | state | fact | businessRule`; id is the actual Blueprint GUID of that type. Records semantic origin, not transformation code. Technical/computed fields can have no sourceRefs; never invent business origins.
- **Service:** server-side business capability. Frontend request wrappers are SourceFiles. Describe operation behavior and constraints, linking business rules and scenarios. api is optional for non-HTTP operations; otherwise use an uppercase HTTP method and a path beginning with `/`. Method/path pairs are unique. Mock/real storage strategy and execution progress are implementation concerns, not Service state.
- **Screen:** a main interface with a distinct task/composition. Simple forms, dialogs, details and search regions may be Blocks in one Screen; do not create a Screen for every form. Supply routePath for independently navigable interfaces. accessRuleIds point to authorization BusinessRules; a reference does not enforce access.
- **Block:** kind is `filter | table | form | detail | chart | custom`. The top search region of a list is a filter. fieldIds belong to its DataContract and specify display/input order; actionIds belong to its containing Screen.
- **Action:** description covers behavior, feedback, refresh and close effects. Choose at most one target: a Block in this Screen or another Screen. presentation is `page | drawer | modal` and requires a target. An action can reference an operation without a presentation target.
- **DeliverySlice:** groups shared designs for a coherent business outcome. It does not duplicate Screens/Services, track execution status or depend on Milestones. sourceFileIds describe expected resources; several Slices may share files, Screens or Services.

## Minimal ordered example

Separate calls after an existing reviewed Blueprint; optional business links are omitted here. Add real GUIDs for the actual business context. Save design selection separately using a real catalog choice.

`save_data_contract`:
```json
{"dataContracts":[{"id":"contract.request-query","name":"Request search","description":"Search input","fields":[{"id":"field.request-query.keyword","name":"keyword","type":"text","required":false,"nullable":false}]}]}
```

`save_service`:
```json
{"services":[{"id":"service.requests","name":"Request service","description":"Find requests","operations":[{"id":"operation.requests.search","name":"Search requests","description":"Return matching requests","inputContractId":"contract.request-query","api":{"method":"GET","path":"/api/requests"}}]}]}
```

`save_screen`:
```json
{"screens":[{"id":"screen.requests","name":"Requests","purpose":"Find requests","routePath":"/requests","dataContractIds":["contract.request-query"],"blocks":[{"id":"block.requests.filter","kind":"filter","description":"Search region","dataContractId":"contract.request-query","fieldIds":["field.request-query.keyword"],"actionIds":["action.requests.search"]}],"actions":[{"id":"action.requests.search","label":"Search","description":"Search using current filters; show errors or refresh results","serviceOperationId":"operation.requests.search"}]}]}
```

`save_delivery_slice`:
```json
{"deliverySlices":[{"id":"slice.requests.search","name":"Request search","goal":"Find the relevant request","screenIds":["screen.requests"],"serviceIds":["service.requests"]}]}
```

## Delete and update

`delete_app_plan_objects` supports `screen | service | dataContract | sourceFile | deliverySlice`. First remove inbound references by saving complete affected objects, or delete dependent/referenced objects together in a cross-type batch. Remaining references are checked; no cascading repair or runtime resource deletion occurs. Root/settings are not deletion targets. Remove nested fields/operations/blocks/actions by replacing their complete owner, repairing references first.

App Plan describes how business will be implemented, without execution status, evidence, versions or Milestone scope. Write validation checks consistency, not business completeness or authorization. Before authorized implementation, read the current Plan, select the requested Slice/object scope, and resolve material design gaps. Use [client delivery](../../Frontend/cdn-vue/delivery.md), `create_app_base` with saved Settings, and the resource-specific Functions. Keep execution records separate and distinguish structural, runtime and business verification.
