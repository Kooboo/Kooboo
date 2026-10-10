# 建站流程入口

本入口只用于独立公开 Site，例如内容站、营销站、企业官网、作品站和商城前台。ToB App / 后台系统必须走 [App Building](../app-building/index.md)，不得借用本流程创建业务资源。

## 进入条件

加载本文前必须已经确认：

1. 用户明确授权本轮实施；
2. 能识别站点业务目标或明确的改造对象；
3. 当前对话不属于 App Blueprint 上下文。

未满足时停留在 `kooboo-coding/SKILL.md` 的意图闸门，只讨论或澄清，不加载 UI 规划 Skill，不写入站点资源。

## 按需加载顺序

1. 读取本文后，立即读取 [建站状态协议](concepts.md)。
2. S0–S3 只读取 [规划阶段](planning.md)。
3. 真正进入 S4/S5 时才读取 [静态交付](static-delivery.md)。
4. 只有用户主动要求动态化，或明确采纳动态化推荐后，才读取 [动态交付](dynamic-delivery.md)。
5. S2 生成 Page Plan 后读取 [能力目录](capability-catalog.md) 建立站点 milestones。
6. Commerce 动态化时再读取 [Commerce 动态化契约](commerce-dynamicization.md)。

不得为了“以后可能会用到”提前加载静态实现、动态化、支付、Backend 或 Commerce 细节。

## 状态转移

| 当前状态 | 下一步 | 执行锁 |
|---|---|---|
| 已读取本文 | 读取 `concepts.md` 和 `planning.md` | 之前不得调用规划或写入函数 |
| S0 就绪 | 用 `ask_user_input` 选择 P1/P2 | 不得静默默认起步模式 |
| S1 已选模式 | `update_todos` 建立 S2 待办 | 不得把 S2/S3/S4 混为一批 todos |
| S2 Page Plan 完成 | 读取能力目录并 `update_site_milestones` | milestones 之前不得进入 S3/S4 |
| S3 Design System 完成 | 读取 `static-delivery.md`，切换 S4 todos | 未切换 todos 不得创建资源 |
| S4/S5 当前批次完成 | 运行 capability Preflight，完成 todos，用 `update_site_milestones` 更新清单 | 只交付当前 Page Plan 页面 |
| 用户确认动态化 | 读取 `dynamic-delivery.md` | 未确认不得主动进入 S6–S10 |
| 当前阶段 Preflight 通过 | 有自然下一步时调用 `show_next_recommended_action` | 推荐不等于用户已授权下一阶段 |

## 全程不变的边界

- Site Brief、Page Plan、Design System、静态交付和动态交付是不同阶段，不在一次 todo 中展开。
- Page Plan 的 `pages` 是当前交付批次；完成当前批次后再建立下一批 todos。
- 每次资源写入后遵循 [Site Preflight](../../Preflight/site.md)，只把实际变更对象标记 dirty；可显式做 `mode: "focused"` 的批次 Preflight（dirty 对象 + 直接消费者）；未接入权威 dirty scope 时保持默认 `mode: "capability"`。完成整个能力阶段时必须运行一次 capability 全量扫描。结构 PASS 不等于业务结果完成。
- 创建或修改对象前按需读取对应 Frontend、Backend、KScript、Function reference，不猜字段或函数名。
- 用户未明确要求时不使用 emoji，不编造业务数据、外链图片或不存在的交互。
