# 做第一个页面

> 用后台页面功能创建一个可以访问的 URL。

## 打开页面列表

| 项目 | 说明 |
|------|------|
| 菜单路径 | 站点后台 -> 站点 -> 页面 |
| 深链 | `/_Admin/site/pages?SiteId={站点GUID}` |
| 详细文档 | [CMS：页面列表](../cms/site/pages.md) |

点击 **新建** 后，先给页面一个清晰的名称和 URL。第一次建议从简单页面开始，例如 `/about`、`/news` 或 `/contact`。

## 选择编辑方式

| 方式 | 适合场景 | 接下来看 |
|------|----------|----------|
| 在线编辑 | 快速调整可视化内容 | [在线编辑](../cms/site/pages-inline-edit.md) |
| 编辑代码 | 直接维护 HTML、模板绑定与资源引用 | [编辑代码](../cms/site/pages-edit-code.md) |
| 开发者模式 | 同时编辑页面、布局、视图、脚本和样式 | [开发者模式](/product/developer-mode.md) |

## 理解页面资源

Kooboo 页面通常由这些资源组成：

| 资源 | 作用 | 文档 |
|------|------|------|
| Page | 定义可访问 URL 与页面内容 | [Page](../templateEngine/page/index.md) |
| Layout | 复用页面骨架、导航与占位区 | [Layout](../templateEngine/layout/index.md) |
| View | 复用局部片段，如 Header、列表卡片 | [View](../templateEngine/view/index.md) |
| Script / Style | 前端交互与样式资源 | [Script](../templateEngine/js/index.md) · [Style](../templateEngine/css/index.md) |

## 验证页面

1. 保存页面。
2. 在页面列表点击预览或复制页面 URL。
3. 确认浏览器可以打开，并且标题、导航、基础样式正常。
4. 如果需要 SEO 字段，回到 [页面设置](../cms/site/pages-setting.md) 或 [基础设置：SEO](../cms/settings/basic/seo.md) 补充。

## 下一步

继续 [添加内容](./add-content.md)，让页面读取后台维护的数据。
