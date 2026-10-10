# 开发

> 菜单：左侧 **开发** 分组（须在 **编辑菜单** 中勾选显示）  
> 路由前缀：`/_Admin/development/...?SiteId={站点GUID}`

**开发**分组集中管理站点的 **模板与站点级资源**：View、Layout、Script、Style、Code，以及 URL 路由、表单、后台菜单、身份验证、Open API、SPA 多语言、模块与定时任务等。与 [页面](../site/pages.md)（页面树与可视化编辑）配合，构成 Kooboo 站点的前端实现层。

::: tip 与模板引擎文档
本系列说明 **后台点哪里、填什么**；语法与 API 见 [模板引擎](../../templateEngine/index.md)（Layout、View、Script、Style 等）及 [KScript API](../../api/index.md)。二者通过菜单中的对象类型一一对应。
:::

::: info 高级菜单
开发分组及多数子项在编辑菜单中标记为 **advanced**。未勾选时左侧不显示本分组。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发** 及其需要的子项。
2. 左侧展开 **开发**。

## 子菜单（与后台顺序一致）

| 菜单 | 路径 | 权限（feature） | 文档 |
|------|------|-----------------|------|
| 组件（Views） | `/development/views` | `view` | [组件](./views.md) |
| 布局（Layouts） | `/development/layouts` | `layout` | [布局](./layouts.md) |
| 脚本 | `/development/scripts` | `script` | [脚本](./scripts.md) |
| 样式 | `/development/styles` | `style` | [样式](./styles.md) |
| 代码 | `/development/code` | `code` | [代码](./code.md) |
| 代码日志 | `/development/code-log` | `code·log` | [代码日志](./code-log.md) |
| 代码搜索 | `/development/code-search` | `code·view` | [代码搜索](./code-search.md) |
| URL | `/development/urls` | `link` | [URL](./urls.md) |
| 表单 | `/development/forms` | `form` | [表单](./forms.md) |
| 菜单 | `/development/menus` | `menu` | [菜单](./menus.md) |
| 身份验证 | `/development/authentication` | `authentication` | [身份验证](./authentication.md) |
| Open API | `/development/openapis` | `openApi` | [Open API](./openapis.md) |
| SPA 多语言 | `/development/spamultilingual` | `spaMultilingual` | [SPA 多语言](./spa-multilingual.md) |
| 模块 | `/development/modules` | `module` | [模块](./modules.md) |
| 定时任务 | `/development/jobs` | `job` | [定时任务](./jobs.md) |

### 开发参考（模板引擎 / API）

| 菜单 | 开发文档 |
|------|----------|
| Views | [View](../../templateEngine/view/index.md) · `k.site.views` |
| Layouts | [Layout](../../templateEngine/layout/index.md) |
| Scripts | [Script](../../templateEngine/js/index.md) |
| Styles | [Style](../../templateEngine/css/index.md) |
| Code | `k.site.codes`（API） |
| Menus | [k.site.menus](../../api/site/menu.md) |
| Authentication | [k.security.jwt](../../api/security/index.md) · [k.response](../../api/response/index.md) |
| Open API | [k.openApi](../../api/openapi/index.md) |
| SPA 多语言 | [SPA 多语言 API](../../api/spa-multilingual/index.md) |
| Modules | [k.module](../../api/module/index.md) |
| 定时任务 | [k.site.runJob](../../api/site/job.md) |

## 无侧栏菜单的编辑路由

以下页面从列表 **编辑** 进入，面包屑仍归属对应菜单项：

| 路由 | 说明 |
|------|------|
| `/development/layout/edit` | 编辑 Layout |
| `/development/view/edit` | 编辑 View |
| `/development/script/edit` | 编辑 Script |
| `/development/style/edit` | 编辑 Style |
| `/development/code/edit` | 编辑 Code |
| `/development/form/edit` | 编辑表单 |
| `/development/menu/edit` | 编辑后台菜单项 |
| `/development/openapi/edit` | 编辑 Open API 定义 |
| `/development/form-values` | 表单提交数据（`activeMenu: forms`） |
| `/development/openapi/authorizes` | Open API 授权记录 |
| `/development/module-menu/:module` | 进入已安装模块子菜单 |

## 与「页面」模块的关系

| 能力 | 入口 | 文档 |
|------|------|------|
| 页面树、发布、富文本/设计器 | **站点 → 页面** | [页面](../site/pages.md) |
| 在线改 HTML、绑定 View/Layout | 页面列表 **在线编辑** | [在线编辑](../site/pages-inline-edit.md) |
| 直接改页面代码文件 | 页面 **编辑代码** | [编辑代码](../site/pages-edit-code.md) |
| 站点级 View/Layout 库 | **开发 → Views / Layouts** | [Layouts](./layouts.md) · [Views](./views.md) |

页面设计器路由（如 `/_Admin/page/edit`、`/_Admin/page/design`）从 **页面** 列表进入，不在开发分组下列出。

## 建议阅读顺序

若从站点搭建角度使用开发能力，常见顺序为：

1. **Layouts** — 页面外壳与公共结构  
2. **Views** — 可复用片段，供页面或 Layout 引用  
3. **Scripts / Styles** — 站点级 JS/CSS 资源  
4. **URLs** — 自定义路由与绑定  
5. **Forms / Menus / Authentication** — 表单、后台菜单与登录方式（按项目需要）  
6. **Open API / SPA 多语言 / 模块 / 定时任务** — 集成与扩展  

具体步骤以各子篇文档为准。

## 相关

| 文档 | 说明 |
|------|------|
| [站点后台菜单总览](../navigation.md) | 完整菜单树 |
| [页面](../site/pages.md) | 页面与可视化编辑 |
| [运维中心](../operations/index.md) | 运行观测（与开发实现分离） |
| [模板引擎](../../templateEngine/index.md) | 模板语法与概念 |
| [KScript API](../../api/index.md) | 脚本 API |
