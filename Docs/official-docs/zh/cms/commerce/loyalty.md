# 会员

> 菜单：**电商 → 会员**  
> 深链：`/_Admin/commerce/loyalty?SiteId={站点GUID}`  
> Tab 深链：`...&name=membership|member|earnPointsConfig|redeemPointsConfig`

配置 **会员等级（Membership）**、查看 **会员** 积分与等级、设置 **获取积分** 与 **兑换积分** 规则。数据写入 Commerce `settings.earnPoint` / `settings.redeemPoint` 及会员等级表；[购物车](./carts.md) 可勾选积分抵扣，[订单](./orders.md) 展示获得积分。脚本 API：[k.commerce.loyalty](../../api/commerce/loyalty.md)、[membership](../../api/commerce/membership.md)。

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **电商 → 会员**（及父级 **电商**）。
2. 左侧 **电商 → 会员**。

页顶四个 Tab，切换时 URL 查询参数 `name` 会同步更新，便于书签与深链。

## 会员等级
（默认 Tab）

定义可售卖的 **会员等级**：名称、有效期、是否可订阅购买、是否按规则自动升级等。

| 列 / 操作 | 说明 |
|-----------|------|
| **名称** | 等级名称 |
| **描述** | 说明文案 |
| **等级** | `priority`，数字越大等级越高（见界面 Tooltip） |
| **时长** | `duration` + `durationUnit`（如年/月） |
| **价格** | 勾选 **订阅**（`allowPurchase`）时显示，币种为站点默认货币 |
| **创建会员等级** | 打开 [创建/编辑弹窗](#会员等级弹窗) |
| **编辑** | 行内铅笔 |
| **批量删除** | 勾选 → 删除（有确认提示） |

<DocImage src="/cms/commerce/loyalty-membership.png" alt="会员等级 Tab：等级列表与创建按钮" width="1120" />

### 会员等级弹窗

**创建** / **编辑** 共用 `EditForm`（编辑带 `id` 拉取详情）。

<DocImage src="/cms/commerce/loyalty-membership-dialog.png" alt="创建会员等级弹窗：名称、升级方式与条件" width="960" />

| 字段 | 说明 |
|------|------|
| **名称** | 必填 |
| **描述** | 多行文本 |
| **等级** | `priority` 整数 |
| **时长** | 会员有效时长与单位 |
| **升级方式** | **订阅**：可付费购买，填价格；**按规则升级**：勾选后配置 **顾客** 维度 `Condition`（且/或），须至少一条条件 |
| **自定义数据** | `KeyValueEditor` 键值对 |

保存后刷新等级列表。

## 会员

查看已注册为会员的 **顾客**（与 [顾客管理](./customers.md) 关联）：当前等级状态、积分余额。

| 列 / 操作 | 说明 |
|-----------|------|
| **姓名** | `firstName` / `lastName` |
| **邮箱 / 电话** | |
| **会员** | `MembershipStatus`：等级名标签；过期显示 **已过期**；无等级为 **非会员** |
| **积分** | 当前可用积分 |
| **搜索** | 关键词，回车查询 |
| **详情** | 行内眼睛 → [会员详情弹窗](#会员详情弹窗) |
| **分页** | 默认每页 10 条 |

<DocImage src="/cms/commerce/loyalty-members.png" alt="会员 Tab：列表、等级状态与积分" width="1120" />

### 会员详情弹窗

| 区块 | 说明 |
|------|------|
| **概要** | 联系人、邮箱、电话、当前会员状态、开始/结束时间、积分 |
| **会员详情** Tab | 会员购买/变更历史；可 **更换等级**、**续费**（需 `loyalty` · `edit`） |
| **积分明细** Tab | 积分流水；支持 **手动充值**、**手动消费** 及说明 |

<DocImage src="/cms/commerce/loyalty-member-detail-dialog.png" alt="会员详情弹窗：概要、会员详情与积分明细" width="960" />

积分变动类型包括：手动充值/消费、订单奖励、订单抵扣、登录奖励等。

## 获取积分

配置写入 `settings.earnPoint`，页底 **保存**（`saveSettings`），支持快捷键保存。

<DocImage src="/cms/commerce/loyalty-earn-points.png" alt="获取积分：有效期、登录与下单规则" width="1120" />

| 区块 | 说明 |
|------|------|
| **奖励积分有效期** | `activeDuration` + 单位：积分自获得起在该时长内有效 |
| **登录获取积分规则** | 多条：每 **周期**（日/周等）获得固定积分 + **登录用户** 条件；可增删、编辑后点 **完成** |
| **下单获取积分规则** | 多条：对已付款订单 **固定积分** 或 **按订单金额百分比** + **已付款订单** 条件 |

## 兑换积分

配置写入 `settings.redeemPoint`，页底 **保存**。

<DocImage src="/cms/commerce/loyalty-redeem-points.png" alt="兑换积分：积分兑换金额与下单抵扣规则" width="1120" />

| 区块 | 说明 |
|------|------|
| **积分兑换金额** | `exchangeRate`：N **积分** = 1 单位默认货币（界面示例展示 `1` 货币单位） |
| **积分抵扣规则** | 多条：下单时可用积分抵扣，支持固定积分或按金额百分比 + **已付款订单** 类条件 |

购物车勾选「使用积分」时，按此处汇率与规则计算（见 [购物车](./carts.md#优惠码) 同页积分抵扣说明）。

## 相关

- [顾客管理](./customers.md) — 顾客资料；联系人卡片可显示会员标签  
- [购物车](./carts.md)、[订单](./orders.md)  
- [k.commerce.loyalty](../../api/commerce/loyalty.md)、[k.commerce.membership](../../api/commerce/membership.md)
