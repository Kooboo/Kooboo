# Integrations

> Menu: **Settings -> Integrations**  
> Deep link: `/_Admin/system/config?SiteId={siteGuid}`

**Integrations** centralizes site **CoreSetting**: database, payment, SMS, OAuth login, object storage, JWT, etc. The page groups items by **Group**; each row shows **name** and **value summary** (masked/truncated in the list). Click **Edit** to open the config dialog.

::: tip Permissions
View list: `config`. Edit and save any item: `config·edit`.
:::

::: warning Security
Dialogs often contain **secrets, connection strings, certificates**, etc. Do not put real values in public docs or Git; CMS docs explain **admin paths and field meaning**—values come from vendor consoles.
:::

## How to Open

1. In [Edit Menu](../../navigation.md#permissions-and-edit-menu), enable **Settings -> Integrations**.
2. Open **Settings -> Integrations** in the left sidebar.

URL query **`group`** expands a group, e.g. `group=Database` from [MySQL tables](../../database/mysql-table.md) onboarding.

<DocImage src="/cms/settings/settings-integrations-overview.png" alt="Integrations overview" width="1120" />

## Page Structure

| Area | Description |
|------|-------------|
| **Fold groups** | Group title (e.g. `Database`, `Payment`); multiple groups can be open |
| **Row: Name** | Integration `name`, matches scripts/admin API (e.g. `Mysql`, `Alipay`) |
| **Row: Value** | `value` summary (often JSON or connection snippet), truncated when long |
| **Row: Edit** | Pencil; `data-cy` is item name; requires `config·edit` |

<DocImage src="/cms/settings/settings-integrations-database.png" alt="Database group" width="1120" />

## Edit Dialog

1. Click **Edit** on a row.  
2. Top **Alert** (if any): callback URLs, connection examples (`GetAlert`).  
3. Form fields from `CoreSetting/GetFields`:

| Control type `type` | UI |
|---------------------|-----|
| Default | Single-line **text** (`el-input`) |
| `checkbox` | **Switch** |
| `file` | **Choose file** (≤10KB, Base64; can remove selected file) |

4. **Confirm** calls `CoreSetting/update`, closes dialog, refreshes list.

<DocImage src="/cms/settings/settings-integrations-edit-dialog.png" alt="Edit integration dialog" width="1120" />

::: info CMS vs API
- **This doc (CMS)**: which group, where to click, field types, save flow.  
- **API docs**: script calls, callbacks, `k.payment.*`, etc. Payment field details live in [k.payment](../../../api/payment/index.md) sub-pages—not duplicated in CMS with secrets.
:::

## Groups and Docs

| Group | Doc | Typical items |
|-------|-----|---------------|
| **Database** | [Database](./database.md) | Mysql, SqlServer, Mongo, Redis, Sqlite |
| **Payment** | [Payment](./payment.md) | Alipay, WeChatV3, Stripe, Paypal… |
| **SMS** | [SMS](./sms.md) | AliSMS, TencentSMSSetting, ChinaMobileSMSSetting |
| **OAuth2** | [OAuth2](./oauth2.md) | Google, Facebook, WeChat/Weibo/Apple login |
| **Storage** | [Storage](./storage.md) | AmazonS3, AliyunOSS, AzureBlob |
| **Others** | [Others](./others.md) | JwtSetting, FirebaseSetting |

Visible items vary by Kooboo version and installed modules; list after login is authoritative.

## Relationship to Other Menus

| Integration | Menus after config |
|-------------|-------------------|
| Database · Mysql / SqlServer | [MySQL tables](../../database/mysql-table.md), [SQL Server tables](../../database/sqlserver-table.md) |
| Payment | [Commerce](../../commerce/index.md), [k.payment](../../../api/payment/index.md) in Code |
| OAuth2 | Front-end login callbacks; dev: [Authentication](../../development/authentication.md) (JWT gateway rules) |
| Storage | [Media library](../../site/media.md), content files |
| JwtSetting | JWT validation in [Authentication](../../development/authentication.md) rules |

Commerce **Settings** does not duplicate Payment—configure **Payment** group here.

## Typical Workflow

1. Expand the **Group**, find the item.  
2. **Edit**, fill vendor parameters per Alert, **save**.  
3. Verify in business menus (MySQL tables after Mysql, test payment after Alipay, etc.).  
4. Read matching API docs for scripts.

## Related

| Doc | Description |
|-----|-------------|
| [Settings overview](../index.md) | Full settings menu |
| [k.payment](../../../api/payment/index.md) | Payment API |
| [k.DB](../../../api/database/index.md) | Database API |
