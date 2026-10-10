# Loyalty

> Menu: **Commerce -> Loyalty**  
> Deep link: `/_Admin/commerce/loyalty?SiteId={siteGuid}`  
> Tab deep links: `...&name=membership|member|earnPointsConfig|redeemPointsConfig`

Configure **membership tiers (Membership)**, view **members** with points and tiers, and set **earn points** and **redeem points** rules. Data is stored in Commerce `settings.earnPoint` / `settings.redeemPoint` and membership tier tables; [carts](./carts.md) can enable points deduction, [orders](./orders.md) show points earned. Script APIs: [k.commerce.loyalty](../../api/commerce/loyalty.md), [membership](../../api/commerce/membership.md).

## How to Open

1. In the [Edit menu](../navigation.md#edit-menu), enable **Commerce -> Loyalty** (and the parent **Commerce** group).
2. Open **Commerce -> Loyalty** in the left sidebar.

Four tabs at top; switching updates URL query `name` for bookmarks and deep links.

## Membership Tiers
(default tab)

Define sellable **membership tiers**: name, validity period, subscription purchase, auto-upgrade by rules, etc.

| Column / Action | Description |
|-----------------|-------------|
| **Name** | Tier name |
| **Description** | Description text |
| **Tier** | `priority`; higher number = higher tier (see UI tooltip) |
| **Duration** | `duration` + `durationUnit` (e.g. year/month) |
| **Price** | Shown when **Subscribe** (`allowPurchase`) is checked; currency is site default |
| **Create membership tier** | Opens [create/edit dialog](#membership-tier-dialog) |
| **Edit** | Row pencil |
| **Batch delete** | Select → Delete (with confirmation) |

<DocImage src="/cms/commerce/loyalty-membership.png" alt="Membership tiers tab: tier list and create button" width="1120" />

### Membership Tier Dialog

**Create** / **Edit** share `EditForm` (edit fetches detail with `id`).

<DocImage src="/cms/commerce/loyalty-membership-dialog.png" alt="Create membership tier dialog: name, upgrade method and conditions" width="960" />

| Field | Description |
|-------|-------------|
| **Name** | Required |
| **Description** | Multiline text |
| **Tier** | `priority` integer |
| **Duration** | Membership validity duration and unit |
| **Upgrade method** | **Subscribe**: purchasable, enter price; **Upgrade by rules**: configure **customer** dimension `Condition` (AND/OR), at least one condition required |
| **Custom data** | `KeyValueEditor` key-value pairs |

Save refreshes the tier list.

## Members

View **customers** registered as members (linked to [customers](./customers.md)): current tier status, points balance.

| Column / Action | Description |
|-----------------|-------------|
| **Name** | `firstName` / `lastName` |
| **Email / Phone** | |
| **Membership** | `MembershipStatus`: tier name tag; **Expired** when past end; **Non-member** when no tier |
| **Points** | Current available points |
| **Search** | Keyword; press Enter to query |
| **Details** | Row eye icon → [member details dialog](#member-details-dialog) |
| **Pagination** | 10 rows per page by default |

<DocImage src="/cms/commerce/loyalty-members.png" alt="Members tab: list, tier status and points" width="1120" />

### Member Details Dialog

| Section | Description |
|---------|-------------|
| **Summary** | Contact, email, phone, current membership status, start/end time, points |
| **Membership details** tab | Purchase/change history; **Change tier**, **Renew** (requires `loyalty` · `edit`) |
| **Points history** tab | Points ledger; **Manual top-up**, **Manual spend** with notes |

<DocImage src="/cms/commerce/loyalty-member-detail-dialog.png" alt="Member details dialog: summary, membership details and points history" width="960" />

Point change types include: manual top-up/spend, order reward, order deduction, login reward, etc.

## Earn Points

Configuration written to `settings.earnPoint`; bottom **Save** (`saveSettings`); keyboard shortcut save supported.

<DocImage src="/cms/commerce/loyalty-earn-points.png" alt="Earn points: validity, login and order rules" width="1120" />

| Section | Description |
|---------|-------------|
| **Reward points validity** | `activeDuration` + unit: points valid for this duration from earn date |
| **Login earn rules** | Multiple rules: fixed points per **period** (day/week, etc.) + **logged-in user** conditions; add/delete; click **Done** after edit |
| **Order earn rules** | Multiple rules: fixed points or **percentage of paid order amount** + **paid order** conditions |

## Redeem Points

Configuration written to `settings.redeemPoint`; bottom **Save**.

<DocImage src="/cms/commerce/loyalty-redeem-points.png" alt="Redeem points: exchange rate and order deduction rules" width="1120" />

| Section | Description |
|---------|-------------|
| **Points to money** | `exchangeRate`: N **points** = 1 unit default currency (UI example shows `1` currency unit) |
| **Points deduction rules** | Multiple rules: points deduction at checkout, fixed points or percentage of amount + **paid order** class conditions |

When cart "Use points" is checked, calculation uses this rate and rules (see points deduction on [carts](./carts.md#discount-codes) same page).

## Related

- [Customers](./customers.md) — customer profiles; contact card can show membership tag  
- [Carts](./carts.md), [Orders](./orders.md)  
- [k.commerce.loyalty](../../api/commerce/loyalty.md), [k.commerce.membership](../../api/commerce/membership.md)
