# 前端对象创建

创建前先 `list_site_object`；同名对象存在时转到 [读取与更新](read-update.md)，不能换随机名称重复创建。

## 参数

| Function | 当前字段 |
|---|---|
| `create_layout` | `name`、`url`、`content` |
| `create_page` | `name`、`url`、`content`、可选 `layoutName` |
| `create_view` | `name`、`content` |
| `create_style` | `name`、`url`、`content`；名称包含后缀 |
| `create_script` | `name`、`url`、`content`；名称包含后缀 |

名称使用简洁的字母、数字、下划线、连字符或点，最长 64 字符；URL 以 `/` 开头。

`create_page.url` 在可能时应为需要独立访问的详情页使用动态路径模板，例如 `/blog/{slug}` 或 `/product/{seoName}`。占位参数名要与详情页 `k-data` 一致；搜索、筛选、排序和分页等列表状态使用 query string。

## Page / Layout 结构

Layout 负责全站 chrome View 与 `k-placeholder`。Page 的 `<layout>` 必须作为根容器包裹 `<placeholder>`，并与 `layoutName`、真实 Layout 名称一致；placeholder id 与 `k-placeholder` 大小写一致。

```html
<layout id="main">
  <placeholder id="Main">
    <view id="home_hero"></view>
  </placeholder>
</layout>
```

共享 Header/Footer 只在 Layout 引用一次，不放入 Page placeholder。View 只输出一个 section，不包含 Page、Layout 或完整文档壳。

## Page Plan 装配

首次规划只传 `mode: "page-plan"` 与 `brief`。后续使用 Page Plan `resolvedBrief`：

1. `design-system` 取得全站视觉变量；
2. `page-item` 取得当前页有序 `blockRefs[]`；
3. `view-batch` 一次取得当前页 prompts；
4. 每个 item 以 `baseContract + item.prompt` 独立生成 View，并使用 `recommendedViewName`；
5. 共享 blocks 单独批量取得并由 Layout 引用。

Page Plan 的默认首批页面由脚本按入口、核心路径、行动闭环和统一预算选择。`pages` 只在用户明确指定当前批次时传入；下一批次复用 `resolvedBrief` 并替换 `pages`。

## 创建顺序

站点级：Layout → Style/Script → 共享 View。

单页级：`page-item` → Page 可见轻骨架 → 打开 Page 并读取 `read_preview_diagnostics` → `view-batch` → 逐 View 创建与读回 → 替换骨架 → Preflight → 刷新 Page。诊断通过前不得取得 View prompts 或创建 page-level View。

不要等全部 View 创建完才创建首页，也不要只凭 Page Plan 摘要编写 View。创建成功后读回关键内容；需要修正时使用 `update_text_object`。
