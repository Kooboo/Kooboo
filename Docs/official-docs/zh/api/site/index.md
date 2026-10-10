# k.site

> 当前站点的版本化数据库访问入口（页面、布局、脚本、样式等资源）

## 概述

`k.site` 是 Kooboo **当前站点**内容的统一入口。通过 `pages`、`layouts`、`views`、`scripts`、`styles`、`menus` 等子 API 进行 CRUD，并支持创建站点、导入导出等管理操作。

::: warning 与 k.file、CMS 文件库的区别
- **k.site**：站点结构化资源（页面、布局、View、Script 等），带版本与路由。
- **k.file**：站点磁盘目录上的文件 KScript 读写，**无**后台专用管理页，见 [k.file](../file/file.md)。
- **k.site.files**：后台 [CMS 文件库](../../cms/content/files.md)中的版本化文件资源，与站点磁盘目录的 `k.file` 不同。
:::

## 子模块导航

| 成员 | 文档 |
|------|------|
| `k.site.pages` | [page.md](./page.md) |
| `k.site.layouts` | [layout.md](./layout.md) |
| `k.site.views` | [view.md](./view.md) |
| `k.site.scripts` | [js.md](./js.md) |
| `k.site.styles` | [css.md](./css.md) |
| `k.site.codes` | [code.md](./code.md) — API 与 CodeBlock（`codeType`） |
| `k.site.menus` | [menu.md](./menu.md) — 前台导航菜单 |
| `k.site.runJob` | [job.md](./job.md) — 定时任务手动触发 |

## 常用方法（节选）

### list()

列出当前登录用户可访问的站点摘要（多站点场景）。该方法需要用户和当前组织上下文；没有组织上下文时可能返回 `null`。

**参数：** 无。

**返回：** `SiteSummaryViewModel[] | null`。

```ts
k.api.get("sites", () => {
    return k.site.list() || []
})
```

### diskSpace

查询站点占用空间，例如 `k.site.diskSpace.getSiteSize()`。

```ts
k.api.get("siteSize", () => {
    return k.site.diskSpace.getSiteSize()
})
```

## 相关文档

- [模板引擎：Layout / Page / View](../../templateEngine/index.md) — HTML 侧如何引用布局与视图
- [k.page](../page/) — 当前渲染页的元数据（`k.page.setTitle` 等）
- [k.module](../module/) — 脚本模块上下文
- [k.label](../label/) — 多语言 `k.t` / `k.label`
- [后台：文件](../../cms/content/files.md) — CMS 文件库（非 `k.file`）
