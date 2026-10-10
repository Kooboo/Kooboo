# 业务 Blueprint 建模

审核通过的语义图存于站点 `AIApplicationStore`。每次通过专用 Function 保存一个完整对象；首次基础结构使用 `save_blueprint`。

## 建模流程

讨论参与者、目标和业务 Scenario。用 `propose_blueprint` 生成不落盘的初始提案，展示并经用户审核后，通过 `save_blueprint` 保存根对象及 Actors/Goals/Modules/Scenarios。随后逐个审核、保存完整 FCO-IM Fact、派生 Business Object、业务需要的 Business Rule 和 UserStory；不要求每个业务创建所有类型的对象。

## 当前读取与对象归属

`read_app_blueprint` 传 `{}`，返回 `exists`、`blueprint`、`actors`、`goals`、`modules`、`scenarios`、`facts`、`businessObjects`、`businessRules`、`userStories`、`relations`、`counts`、`canCreateAppPlan`。图未变化时复用成功读取；写入后、已知变化或读取失败/状态未知时重新读取。该 Function 没有视图或分页参数。

根对象包含 `id`、`name`、`description`；其他对象独立存储。依赖使用读取/保存返回的 GUID；单对象保存创建新对象时可省略 ID，由服务端分配，更新必须复用已有 ID。Fact 使用 `kind`、`fcoim`；Business Object 保留 `fields`、`sourceFactIds`、`dbName`、`storage`、`stateDimensions`。未决问题和假设归属 [Planning Topics](planning-topics.md)。

`canCreateAppPlan` 检查根对象、Module、Scenario、Fact、Business Object 是否存在，不证明需求完整或授予规划/实施授权。根 ID 表示站点单例，不是内容修订号。

## 保存边界

所有保存 Function 都需要用户批准。提交前读取对应 Function schema，先保存依赖对象，再保存引用它的对象；更新单个对象不要重新发送整个图。

| Function | 输入及关系归属 |
|---|---|
| `save_blueprint` | `proposal` 仅包含 `blueprint`、`actors`、`goals`、`modules`、`scenarios`、`relations`。首次需要根及基础结构，沿用 `propose_blueprint` 返回的 ID；更新根时仅发送完整根对象。结构关系使用 `sourceType`、`sourceId`、`targetType`、`targetId`，包括 `Goal -> Actor`、`Module -> Scenario` 和根关联。 |
| `save_actor` | 完整 `actor`，服务端自动关联当前根。 |
| `save_goal` | 完整 `goal` 和已保存的 `actorIds`；新 Goal 至少需要一个 Actor 关联。 |
| `save_module` | 完整 `module`，可选已保存的 `scenarioIds`；自动关联根。 |
| `save_scenario` | 完整 `scenario` 和已保存的 `moduleIds`；新 Scenario 至少需要一个 Module 关联。 |
| `save_fact` | 完整 `fact`，可选已保存的 `scenarioId`；包含审核后的 reading、对象类型、角色、字段、约束和示例。 |
| `save_business_object` | 完整 `businessObject`，从已保存且审核通过的 Fact 派生。 |
| `save_business_rule` | 完整 `businessRule`，可选已保存的 `scenarioIds`；服务端创建根和 Scenario 关联。 |
| `save_user_story` | 完整 `userStory`，包含受益人 `actorId`、需求、业务价值和 Given/When/Then `acceptanceCriteria`；新建必须传 `goalId`，可选已保存的 `scenarioIds`；服务端创建主要 Goal 和 Scenario 关联。 |

Module 是业务能力范围，不是表。Scenario 可以属于多个 Module，不为表达归属重复创建。提交对象完整替换已有定义，保留未改字段。未提交对象和关系保留，空集合不表示删除。删除或更换故事已有主要 Goal 需要先处理变更方案，再执行依赖保存；这些 Function 不删除关系。

## 业务规则

业务涉及验证、计算、授权、状态迁移、派生或策略行为时创建规则，先审核含义和来源。规则需要名称、statement、`reviewStatus="confirmed"`，及恰好一个与 `type` 对应的 `specification` 成员。严格使用 schema 枚举，例如 `validationKind="unique"`。引用的 Fact、Actor、Business Object 和状态必须已保存。使用 `scenarioIds`，不要自行构造规则关系。

已有 Blueprint 的策略规则输入示例（仅展示契约，未执行站点操作）：

```json
{"businessRule":{"name":"决策理由","type":"policy","statement":"决策应记录理由。","reviewStatus":"confirmed","specification":{"policy":{"requirements":["记录决策理由。"]}},"source":{"type":"user-stated","description":"已审核需求"}},"scenarioIds":[]}
```

## 保存验证与失败恢复

保存后读取语义图，核对该对象及关联。参数失败时，按回执指出的字段及合法枚举、形状或 GUID 修正，只重试该对象的 Function；保留已批准含义和 ID，不重新生成无关对象或重复加载未变 reference。缺少依赖时先保存依赖。校验在写入前完成；存储/运行失败后先读回，再按稳定 ID 重试，不能假定多存储写入已全部回滚。读取失败不是对象不存在。

## 持久化边界

保存仅记录业务定义，不实现 Page、API、数据库、权限或规则执行。报告实际保存结果；仅业务建模请求完成已授权语义范围后结束。带规划交付路径在模型就绪后按已有授权继续，保存本身不增加授权。下一阶段遵循 [App Building 入口](index.md)。
