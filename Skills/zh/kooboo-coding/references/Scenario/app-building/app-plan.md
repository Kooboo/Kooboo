# App Plan 编写

用于已具备审核后的 Blueprint 业务语义、且用户授权实施规划的带规划路径。P1 快速直接交付遵循[无规划实施](lightweight.md)，不创建 Blueprint 或 App Plan。

## 读取和工具发现

调用 `read_app_plan({})` 读取完整服务端当前 Plan，返回 `exists` 和 `plan`；读取不创建 Plan。复用返回的 ID 和定义。强类型聚合包含 `id`、`blueprintId`、`settings`、`sourceFiles`、`dataContracts`、`services`、`screens`、`deliverySlices`。

按需通过 `use_function` 加载各 Native AI Function 及其 Schema，再调用：

| Function | 输入 | 用途 |
| --- | --- | --- |
| `read_app_plan` | `{}` | 完整当前 Plan |
| `save_source_file` | `sourceFiles: [...]` | 资源文件名和目录 |
| `save_data_contract` | `dataContracts: [...]` | 共享输入/输出结构 |
| `save_service` | `services: [...]` | 服务端能力与操作 |
| `save_screen` | `screens: [...]` | 界面、Block 和 Action |
| `save_delivery_slice` | `deliverySlices: [...]` | 内聚可交付范围 |
| `save_app_settings` | `settings: {...}` | 设计、布局、导航 |
| `delete_app_plan_objects` | `targets: [{type,id}, ...]` | 显式删除 |

Function 直接持久化服务端模型。遵循强类型 Schema，不调用 Skill Script 或写文件保存规划。

## 保存边界

- 保存前审核授权范围内的设计。写入受批准机制约束；同一已审核范围复用已有授权。保存规划不授予资源实施权限。
- 首次成功保存自动初始化 Plan，要求已有 Blueprint，无需单独创建根对象。读取、校验失败和空批次不初始化。
- 每次提交一个或多个同类型完整对象。稳定 ID 替换整个对象及其内嵌定义。基于当前读取保留未变字段；提交对象省略的可选字段会清空，未提交对象保留。空批次不写入也不删除，集合不可为 null。
- 引用目标必须已保存或包含在同类型批次中。先保存依赖；互相引用的 Screen、嵌套契约可同批提交，但契约不能循环。可按 SourceFiles → DataContracts → Services → Screens → DeliverySlices → 带导航的 Settings 推进。Settings 也可先用空导航保存，待路由 Screen 存在后完整替换。不要为满足顺序编造占位对象或去掉所需引用。
- 整批候选图校验后一次写入聚合，非法输入整批拒绝，不部分接受。按内聚范围拆成适量批次。
- 写入返回 `persisted`、`appPlanId`、`ids`；空批次返回 `persisted=false`。完成授权批次后读回，核对已保存设计。
- 校验失败时修复回执点名的对象或引用。存储结果不确定时先读实际状态再重试，不重建整个 Plan 或更换稳定 ID。

## 对象字段与语义

Plan 对象及内嵌字段、操作、Block、Action、导航项的 ID 全局唯一，使用稳定 ASCII ID（如 `screen.requests`）或 GUID 字符串。Blueprint 关联使用 `read_app_blueprint({})` 返回的真实 GUID，不从名称猜 ID。可选集合默认空数组。

| 对象 | 字段 |
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

- **Settings：** 按[设计系统选择](../../Frontend/app-design-systems.md)复用目录精确 ID/name。layout.pattern 描述 Shell 布局。导航只选业务入口，不是全部 Screen；叶节点引用有路由的 Screen，无 Screen 的分组需有子项。
- **SourceFile：** type 为 `script | style | page | api | codeblock`。fileName 带扩展名、不含目录；path 为相对逻辑目录，不含文件名，空字符串表示资源根目录。例如 `{id:"source.requests",type:"script",fileName:"screen.requests.js",path:"screens"}`。多个对象可共享文件；规划文件不创建资源，也不证明文件存在。
- **DataContract：** 共享请求/响应或展示/输入结构，不是物理存储或执行映射。字段 name 为真实属性名；type 为 `text | integer | decimal | boolean | date | datetime | object` 及相应 `[]` 类型。required 表示属性必须出现；nullable 表示整体值可为 null，数组元素非 null。只有 object/object[] 需要 objectContractId；契约不能循环引用。
- **SourceRef：** type 为 `businessField | stateDimension | state | fact | businessRule`，id 指向相应 Blueprint 类型的真实 GUID。记录业务来源，不执行转换。计算/技术字段可没有 sourceRefs，不编造业务来源。
- **Service：** 服务端业务能力，前端请求封装属于 SourceFile。描述操作行为和约束，关联规则与 Scenario。非 HTTP 操作可省略 api；否则使用大写 HTTP 方法和以 `/` 开头的路径，method/path 组合唯一。mock/真实存储策略及执行进度属于实施关注点，不作为 Service 状态。
- **Screen：** 有独立任务与布局的主界面；简单表单、弹窗、详情和搜索区可作为所属 Screen 的 Block，不为每个表单创建 Screen。需要独立导航时才提供 routePath；accessRuleIds 指向 authorization BusinessRule，关联不代表已执行权限。
- **Block：** kind 为 `filter | table | form | detail | chart | custom`；列表顶部搜索区用 filter。fieldIds 来自该 Block 的 DataContract，顺序表达展示/输入顺序；actionIds 属于所在 Screen。
- **Action：** description 描述行为、反馈、刷新和关闭效果。至多选择一个目标：本 Screen 的 Block 或其他 Screen。presentation 为 `page | drawer | modal`，提供时必须有目标。Action 可仅关联 Operation，没有展示目标。
- **DeliverySlice：** 组合内聚业务结果所需的共享设计，不重复内嵌 Screen/Service，不记录执行状态，不依赖 Milestone。sourceFileIds 表达预计资源；多个 Slice 可共享文件、Screen、Service。

## 最小顺序示例

已有审核后的 Blueprint 时，以下分别按顺序调用，省略可选业务引用；实际业务应补入真实 GUID。设计选择另用真实目录项保存。

`save_data_contract`：
```json
{"dataContracts":[{"id":"contract.request-query","name":"申请搜索","description":"搜索输入","fields":[{"id":"field.request-query.keyword","name":"keyword","type":"text","required":false,"nullable":false}]}]}
```

`save_service`：
```json
{"services":[{"id":"service.requests","name":"申请服务","description":"查询申请","operations":[{"id":"operation.requests.search","name":"搜索申请","description":"返回匹配的申请","inputContractId":"contract.request-query","api":{"method":"GET","path":"/api/requests"}}]}]}
```

`save_screen`：
```json
{"screens":[{"id":"screen.requests","name":"申请列表","purpose":"查找申请","routePath":"/requests","dataContractIds":["contract.request-query"],"blocks":[{"id":"block.requests.filter","kind":"filter","description":"搜索区域","dataContractId":"contract.request-query","fieldIds":["field.request-query.keyword"],"actionIds":["action.requests.search"]}],"actions":[{"id":"action.requests.search","label":"搜索","description":"按当前条件查询，失败提示错误，成功刷新结果","serviceOperationId":"operation.requests.search"}]}]}
```

`save_delivery_slice`：
```json
{"deliverySlices":[{"id":"slice.requests.search","name":"申请查询","goal":"找到相关申请","screenIds":["screen.requests"],"serviceIds":["service.requests"]}]}
```

## 删除与更新

`delete_app_plan_objects` 支持 `screen | service | dataContract | sourceFile | deliverySlice`。先完整保存受影响对象以去掉入向引用，或同一个跨类型批次一起删除依赖者和被引用对象。校验剩余引用，不级联修复，不删除运行资源。根对象和 Settings 不属于删除目标。内嵌字段、Operation、Block、Action 通过完整替换所属对象移除，先修复引用。

App Plan 表达如何实施业务，不记录执行状态、证据、版本或 Milestone scope。写入校验检查一致性，不证明业务完整或已有授权。授权实施前读当前 Plan，选择用户要求的 Slice/对象范围，解决实质设计缺口。按[客户端交付](../../Frontend/cdn-vue/delivery.md)、已保存 Settings 调用 `create_app_base`，通过各资源 Function 实施。执行记录独立，分别说明结构、运行和业务验证。
