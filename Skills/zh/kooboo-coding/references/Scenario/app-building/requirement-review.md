# 已有 App 的需求变更确认


通过 `read_app_blueprint` 传 `{}` 读取当前 Blueprint 语义图，以返回的 GUID 定位受影响对象，见 [Blueprint](blueprint.md)。通过 `read_app_plan({})` 读取服务端当前 App Plan，以返回的 ID 定位受影响对象，见 [App Plan](app-plan.md)。参数错误只修正对应调用，不重读仍有效的数据。

`review_app_requirements` 的讨论标识放在 `review.discussionId` 内，不放在顶层。该工具在 AI Chat 中延迟声明，一个会话内首次调用前先用 `use_function` 载入其 InputSchema。

## 选择流程前判断规划影响

每项新请求先通过 `get_site_info` 发现目标，再读取当前 Blueprint 语义图、受影响的 App Plan 对象与资源，以当前持久化定义为基线。系统自动维护成功读取记录。

分析需求是否改变已有定义或业务行为。此协议只适用于已有 App 的需求变更；新 App 首次创建时沿用首次创建 SOP，先用 `propose_blueprint` 展示提案、经用户确认后 `save_blueprint`，不调用 `review_app_requirements`。已有 App 的新增能力及影响 Blueprint 或 App Plan 的变更，必须在规划或资源写入前使用 `review_app_requirements` 对齐。样式、文案以及恢复既有约定行为的修复，可按普通已授权维护处理。分类新增图标字段需要对齐；调整已有图标间距通常不需要。

读取失败或影响未明时，保持只读分析，可展示 draft 草稿，不能实施；当前规划不可读时不得标记 ready。普通偏好选择不授予实施权限。只有完全相同且未变更的已审阅范围可以复用授权，独立新需求必须重新对齐。两种规划均不存在的站点沿用首次创建 SOP。

## 先讨论，再写入

读取当前 Blueprint 语义图、受影响的 App Plan 对象、相关 Topic 和实际资源。区分用户明确要求、待确认问题与需要保留的已有行为。“产品管理支持上传图片”表示开始讨论新增需求，不代表用户已决定单张主图、图片数量、存储规则或修改其他页面。

规划或业务资源写入前，用 `review_app_requirements` 展示具体方案。关键未决项保持空值并设为必填。独立问题、建议和答案存入 PlanningTopic；确认 Topic 只记录讨论。不得把 AI 推断标为 `user-stated`。影响规划的新需求即使首条消息要求实施，也要进入需求对齐；只有同一已审阅且已授权范围的续作无须重复确认。

## 可编辑草稿

函数接受 `question`、可选的 `description` 和 `review`。`question` 是消息标题，`review` 包含 `discussionId`、`analysisStatus`、`impacts`、`notes`（初始空字符串）和恰好四个 section：

| section.id | 内容 |
| --- | --- |
| `requirements` | 需求描述、业务规则、关键问题和选项 |
| `pages` | 页面变化、操作和纳入范围的勾选项 |
| `data` | 数据与关联影响分析 |
| `acceptance` | 实施范围、可编辑和增删的验收标准 |

每个 section 包含 `id` 和 `fields` 数组。每个 field 包含：

- `id`：在整个草稿内唯一，重新分析时保持稳定。
- `label`：面向用户的标题。
- `type`：`text`、`textarea`、`boolean`、`select` 或 `multiselect`。
- `value`：对应类型的字符串、布尔值或字符串数组。选择题保存 option ID，未回答单选使用空字符串。
- `required`：可选，未确认关键问题设为 true。
- `options`：仅选择题使用，每项包含唯一 `id` 与非空 `label`。
- `current`：可选，用自然语言交代改动前的行为。
- `readOnly`：只用于 AI 推导的影响分析；需求和验收标准保持可编辑。

使用会话语言，不展示 JSON、规划内部 ID、版本或工具参数。完整结构示例见英文同名 reference。

用户可以编辑文本与范围、增删验收标准，也可以新增、重命名或删除选项。新增选项自动选中；删除已选单选项恢复为待选择。取消勾选只改变答案，删除则移除候选项。任意编辑都会使影响分析失效。收起消息保留当前草稿；浏览器存储可用时，同一调用的草稿保留在当前浏览器标签页。

## 返回动作与授权

返回 `{confirmed, value, feedback}`，其中 `value={action, review, implementationAuthorized}`：

1. `action="analyze"`，`confirmed=false`：用户要求重新分析，不是取消。以 `value.review` 为准，保留用户修改的文本、ID、自定义选项、答案、补充意见和验收标准；只补读相关事实，更新推导的影响内容，再次调用需求确认消息。有实质冲突时增加必答问题，不得擅自覆盖用户答案。此动作不授权规划或资源写入。
2. `action="confirm"`，`confirmed=true`，`implementationAuthorized=false`：记录 Topic 答案，等待实施指示。不更新 Blueprint、App Plan 或资源。
3. `action="plan"`，`confirmed=true`，`implementationAuthorized=false`：记录答案，仅局部更新已确认范围的 Blueprint 和 App Plan，检查一致性后停止，不创建或修改代码、资源或业务数据。
4. `action="implement"`，`confirmed=true`，`implementationAuthorized=true`：记录答案，局部更新规划，再实施已确认范围。

不能仅凭外层 `confirmed` 判断实施授权。分析期间只读取事实和记录 Topic，不写规划、代码或用于探测的媒体文件。

## 局部更新

读取受影响定义，复用已有对象 ID。Blueprint 写入前先读 [分对象保存契约](blueprint.md#保存边界)。获得规划更新或实施授权后，通过对应 `save_*` Function 逐个保存审核后的完整对象，保留未改字段和已有 ID，先保存依赖，使用工具的类型化 ID 参数增加关联。未提交对象和关联保留；更新业务定义不需要重新生成根提案或进入资源实施。

该保存契约没有对象或关系删除操作。已确认需求需要删除时，记录待解决的删除事项，先明确该操作再进行依赖它的写入；不要用省略对象或清空集合表示删除，也不能声称删除已完成。

受影响的 App Plan 内容遵循 [App Plan 编写](app-plan.md)，用对应强类型保存 Function 提交完整变更对象，基于当前读取保留未变字段与稳定 ID。修改 Action 需完整保存所属 Screen；修改字段或 Operation 需完整保存所属 DataContract 或 Service。审核后的顶级对象删除用 `delete_app_plan_objects`，先修复引用或同批删除依赖对象。

先更新 Blueprint，再更新受影响的 App Plan。保留设计选择与未受影响内容；读回两者，核对审核范围后再按授权实施。执行进度不放入 Plan，编写或实施不要求 Milestone 绑定。写入失败遵循对应对象的恢复协议。

## 消息内步骤与规划影响

`discussionId` 标识同一次需求讨论，重新分析必须复用；确认需求、更新规划或确认并实施之后再次调整，使用新的讨论 ID，保留之前的确认快照。它用于关联分析轮次，不是规划版本。

`analysisStatus` 为 `draft` 或 `ready`。关键选择未确定时用 `draft`；读取相关现有规划、根据完整选择更新影响分析后才设为 `ready`。填写完成不等于分析完成。前端分为明确需求、查看变化、确认范围三个步骤，支持收起消息和查看各轮记录。用户直接通过聊天补充时，返回 `analyze`、当前编辑草稿和原文反馈；必须据此更新选择并重新分析，不表示授权实施。不明确的反馈转为必答问题，不能猜测或覆盖用户已选内容。

`impacts` 为 `{id,target,path,label,change,before,after,reason,fieldIds,dependsOn}` 数组。`target` 为 `blueprint` 或 `app-plan`，`change` 为 `add`、`modify` 或 `remove`。`path` 使用局部读取获得的规划属性和稳定对象 ID，新增项使用拟议 ID，不使用数组下标。`label`、`before`、`after`、`reason` 用简洁业务语言描述具体对象、当前情况、建议变化和原因。`fieldIds` 引用触发变化的需求字段；`dependsOn` 引用上游影响项 ID，用于描述提案依赖，不代表业务对象关系，不能按排列顺序编造关系。没有影响时使用空数组，不为填满维度虚构改动。

例如产品图片：Blueprint 可涉及产品图片关系、数量上限、主图规则；App Plan 可涉及图片存储和关联、上传能力、新增编辑表单与产品列表。应根据现有设计与用户选择推荐字段、数组或关联对象，不能将互斥方案同时作为确定改动。先检查引用产品的其他页面，再讨论是否扩大为图片加名称的展示范围。影响引用只是提案，既不是可执行补丁，也不代表实际规划已更新。

## 影响分析范围

讨论阶段通过当前语义图及受影响的 Plan/资源读取确认现状，用简洁的 `impacts` 描述当前行为、建议变化及原因，已有对象引用实际 ID。获得规划更新或实施授权后，再准备 Blueprint 对象批次和 App Plan 更新。主要选择已明确、影响范围已说明后，设置 analysisStatus 为 ready。
