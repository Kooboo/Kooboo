# AB Testing

> Menu: **Operations Center → AB Testing**  
> Deep link: `/_Admin/system/ab-testing?SiteId={siteGuid}`

Configure **A/B experiments** for a site: pick an experiment object (page, view, product, etc.) as **control A**, configure one or more **variants B/C…** (full object replacement or field replacement), split visitors by traffic share and optional user conditions, and view reports with a **Goal** as the winning metric.

::: tip Permission
Menu permission: `abTesting`. Enable **Operations Center → AB Testing** (and parent **Operations Center**) in Edit Menu.
:::

## How to Open

1. In [Edit Menu](../navigation.md#permissions-and-edit-menu), enable **Operations Center → AB Testing**.
2. Open **Operations Center → AB Testing** on the left.

The main screen is the experiment list; **Create AB Test** opens a four-step wizard; row actions open **Experiment Report**. Sections below include screenshots.

<DocImage src="/cms/operations/ab-testing-overview.png" alt="AB testing list overview" width="1120" />

## Header and Global Switch

Path: `/_Admin/system/ab-testing?SiteId=...`

| Control | Description |
|------|------|
| **Enable** | Site-wide AB testing master switch (`updateAbTestStatus`); when off, the frontend stops splitting traffic |
| (List area) | See next section |

<DocImage src="/cms/operations/ab-testing-header.png" alt="AB testing header and enable switch" width="1120" />

## List Toolbar

| Control | Description |
|------|------|
| **Create AB Test** | Go to `/_Admin/system/ab-testing/create?SiteId=...` |
| **Status** | Filter: Draft, Running, Paused, Completed |
| **Search** | Filter by experiment name or description keywords |

<DocImage src="/cms/operations/ab-testing-list-toolbar.png" alt="Create button and filters" width="1120" />

## Experiment List

Table columns:

| Column | Description |
|----|------|
| **Experiment Name** | Name + description summary |
| **Status** | Draft `Draft`, Running `Running`, Paused `Paused`, Completed `Completed` / `Archived` |
| **Traffic Share** | **Control A** (100% − sum of variant weights) and each **variant** weight percentage |
| **Progress** | When running: days elapsed / total days and start/end time |
| **Actions** | **View Report** (published and past start time) or open detail; **Delete** experiment |

Click the experiment name area or **View Report** → [Experiment Report](#experiment-report-subpage).

<DocImage src="/cms/operations/ab-testing-list-table.png" alt="AB testing experiment list" width="1120" />

## Create / Edit Experiment (Four-Step Wizard)

Path: `/_Admin/system/ab-testing/create?SiteId=...`  
Edit: same path with `name={experimentName}` query param, prefilled from the list.

Footer: **Previous**, **Next** / **Save**; **Cancel** returns to the list.

### Step 1: Basic Info

| Field | Description |
|------|------|
| **Experiment Name** | Required, unique identifier |
| **Experiment Description** | Optional |
| **Start Date / End Date** | Experiment window; end must be after start |

<DocImage src="/cms/operations/ab-testing-create-step-basic.png" alt="Create wizard: basic info" width="1120" />

### Step 2: Experiment Goal

| Field | Description |
|------|------|
| **Object Type** | e.g. Page, View, Product, TextContent (`ObjectOptions`) |
| **Test Object** | Specific object bound to control A (searchable) |
| **Winning Metric** | Select a **Goal** (from behavior-analysis traffic settings); **Edit Goal** opens Goal management dialog |

<DocImage src="/cms/operations/ab-testing-create-step-goal.png" alt="Create wizard: experiment goal and winning metric" width="1120" />

### Step 3: Experiment Configuration

Configure each variant:

| Mode | Description |
|------|------|
| **Replace Entire Object** | Pick another object as variant content |
| **Replace Fields** | Available when object type supports `fieldOverWrites`; set field + multilingual replacement values |

You can **Add Variant**, delete variants, and rename variants. Control A is the object from Step 2 and is not selected again here.

<DocImage src="/cms/operations/ab-testing-create-step-config.png" alt="Create wizard: variant configuration" width="1120" />

### Step 4: Traffic and User Assignment

| Block | Description |
|------|------|
| **Traffic Allocation** | Control A share = 100% − all variant weights; set 0–100% per variant; total must not exceed 100% |
| **User Group Conditions** | Optional: attribute + comparator + value (e.g. country, device); **Add Condition** |

<DocImage src="/cms/operations/ab-testing-create-step-traffic.png" alt="Create wizard: traffic and user assignment" width="1120" />

## Experiment Report Subpage

Path: `/_Admin/system/ab-testing/report?SiteId=...&name={experimentName}`

Top bar: back, experiment name, status tag; buttons such as **Edit**, **Start Experiment**, **Pause Experiment**, **Restart**, **End Early** depending on status.

### Experiment Details Tab

Includes:

- **Basic Info**: name, description, target users, winning metric, duration and time range  
- **Experiment Configuration**: control and variant configuration summary  
- **Traffic Allocation**: share per group  

<DocImage src="/cms/operations/ab-testing-report-detail.png" alt="Experiment report: Experiment Details tab" width="1120" />

### Experiment Report Tab

Shown when the experiment is **running or completed**:

- **Experiment Conclusion**  
- **Group Data** (performance per variant)  
- **Confidence Statistics**  

Draft or not-yet-started experiments show an empty state (no report data yet).

<DocImage src="/cms/operations/ab-testing-report-report.png" alt="Experiment report: Experiment Report tab" width="1120" />

::: tip Goals and behavior analysis
The **winning metric** depends on Goals configured in behavior analysis. If the dropdown is empty, maintain Goals in **Behavior Analysis** (or related traffic settings) first, then add via **Edit Goal** in the create wizard.
:::

## Related

- [Operations Center overview](./index.md)  
- [Page Interaction](./page-interaction.md) — anchor click statistics  
- Behavior Analysis — Goals, user segments
