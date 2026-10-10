# 站点级待确认事项

Blueprint 保存已采纳的业务定义，App Plan 保存实施设计。假设、开放问题、建议及用户答案通过站点级待确认事项独立保存，沿用 Skill Script 和 `k.file`。

## 读写

- `read_planning_topics()` 返回有界摘要、`total` 和 `nextCursor`，使用返回的游标继续读取。传 `ids` 读取完整内容；可按 `status`、`artifact` 筛选。按 Artifact 筛选时也包含站点通用事项。
- 分页：`cursor` 为非负整数，默认 `0`；`maxItems` 为 `1`～`30` 的整数，默认 `20`。首次调用参数：`{ "cursor": 0, "maxItems": 20 }`。后续分页保持相同筛选条件，使用返回的 `nextCursor`；其为 `null` 时停止读取。
- `update_planning_topic(item, expectedUpdatedAt)` 按稳定 ID 创建或完整替换一个事项。更新已有 ID 前先读取详情，将响应的 `updatedAt` 原样传入 `expectedUpdatedAt`；只有创建新 ID 才可省略。遇到 `TOPICS_CHANGED` 应重读并合并本次修改，保留当前答案、确认依据及无关字段，不可直接重试旧对象。回答问题时使用原 ID。无需修改或重建 Blueprint／App Plan 来记录讨论。
- 通过普通聊天或 `ask_user_input` 澄清，用业务语言展示问题、建议与答案。存储结构不是给用户看的确认界面。

## 对象

| 字段 | 含义 |
| --- | --- |
| `id` | 稳定的事项 ID |
| `kind` | `assumption` 或 `open-question` |
| `question`、`context` | 业务问题与讨论背景 |
| `suggestion` | 建议答案或暂定假设，可为空 |
| `options[]` | 可选候选项，每项包含 `id`、`label`、`description` |
| `answer` | 已记录答案，可为空 |
| `status` | `open`、`answered`、`confirmed`、`dismissed` |
| `confirmationEvidence` | 用户明确确认／取消的原话及上下文；未确认时为空 |
| `impact` | `low`、`medium`、`high`，用于讨论优先级 |
| `resolveBefore` | `app-plan-creation`、`implementation`、`production-release` |
| `scope` | `artifact`（`site`、`blueprint`、`app-plan`）、`relatedIds[]`、`milestoneIds[]` |

假设是问题的建议答案，不是已经确认的业务事实。例如问题为“首批是否支持多个团队”，建议为“首批只支持单一团队”。只有用户明确同意后才记录确认。`answered` 仍表示答案尚未确认；`dismissed` 表示用户明确决定该问题不再适用。问题或建议答案改变后，应重新打开事项并清空旧确认依据，等待用户再次确认。

关联 ID 用于定位讨论对象，可以指向尚未规划的对象；不作为外键，也不复制对应对象。`milestoneIds` 只记录该 Topic 关联的 SiteMilestone 条目，不阻塞实施，也不改变任何门控；Topics 同样不能绕过 App Plan 创建门槛。

## 门槛与权限

未确认事项阻塞指定阶段及后续阶段，与讨论优先级无关。Blueprint 校验和 App Plan 创建检查 `app-plan-creation`；App Plan 实施 readiness 同时检查 `implementation` 和目标 Milestone。根据 `blockingPlanningTopicIds` 使用 `read_planning_topics(ids=...)` 读取并讨论。`planningTopicsAvailable=false` 时应重试读取，不能当作没有问题。发布前显式读取并解决 `production-release` 的未确认事项。

确认状态是讨论记录，不是审批凭证或实施授权，也不会把答案自动应用到 Blueprint／App Plan。已采纳的规划内容仍使用原工具单独更新，遵守用户授权范围。仅空白站点首次建 App 时选择 P1/P2；已有 App 的 Topic 讨论或增量变更直接沿用现有规划；不要把每一个安全、可逆的 P1 默认项都变成阻塞问题。

存储路径为 `_ai/site-development/planning-topics.json`。事项属于站点，可以独立于会话和规划版本读取。不新增版本历史，不迁移测试阶段的 App 数据。

`scope.milestoneIds` 记录关联的 `get_site_info` 返回的 SiteMilestone ID，仅供参考，不阻塞实施；不要把 Topics 复制到条目正文或 App Plan。
