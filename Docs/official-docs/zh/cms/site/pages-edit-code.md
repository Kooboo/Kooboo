# 编辑代码

> 从 [页面列表](./pages.md) 行内 **编辑代码** 进入（设计页为 **设计**）  
> 深链（HTML 页面）：`/_Admin/page/edit?SiteId={站点GUID}&id={页面Id}`  
> Layout 页面：`/_Admin/page/layout-edit?SiteId=...&id=...&layoutId={LayoutId}`  
> 新建 HTML 页：`/_Admin/page/edit?SiteId=...`（无 `id`，保存后写入 `id`）

全屏 **Monaco** 编辑器修改 Page 的 **body**（HTML 源码），支持 KScript 提示、格式化与代码引用。

::: tip 页面类型
| 类型 | 本页路由 |
|------|----------|
| HTML 页面 | `page-edit` |
| Layout 页面 | `layout-page-edit` |
| 设计页面（Designer） | `page-design` / `layout-page-design`（可视化，非 Monaco） |
| 富文本页面 | `rich-page-edit`（富文本，非本文） |
:::

## 如何打开

1. **页面** 列表 → 目标行 → **编辑代码**（`</>` 图标）。
2. 新建 **HTML 页面** 时，创建向导完成后也会进入本页（先填名称与 URL）。

<DocImage src="/cms/site/pages-edit-code.png" alt="编辑代码：Monaco 编辑器" width="1120" />

## 界面说明

| 区域 | 说明 |
|------|------|
| 顶栏左侧 | **页面名称**、**URL**（新建时可编辑名称；已有页名称通常锁定） |
| 顶栏右侧 | **预览**（前台地址）、**格式化**（整理 HTML） |
| 中部 | 代码编辑器，`language="html"`，支持 **k-script** 与**代码引用**（已保存的页面） |
| 底部栏 | **返回**、**保存**、**保存并返回** |

快捷键：**保存** 与系统其它编辑器一致（以 Kooboo 后台快捷键配置为准）。

## 保存行为

- 校验 **名称**（新建）、**URL** 唯一性。
- 保存成功后写入站点 Page 资源；**新建** 保存后会将地址栏替换为带 `id` 的 `page-edit`，避免重复创建。
- 离开页面前未保存会提示（登录页除外）。

## 写什么内容

### HTML 页面

可写完整页面，或使用 Layout：

```html
<layout id="main">
  <placeholder id="Main">
    <h1>关于我们</h1>
  </placeholder>
</layout>
```

约定见 [模板引擎 · Page](../../templateEngine/page/index.md)、[Layout](../../templateEngine/layout/index.md)。

### Layout 页面

编辑器中维护的是**该页在 Layout 各占位区**的内容（与 Layout 资源本体不同；Layout 本体在 **开发 → Layout**）。

## 预览

顶栏 **预览** 打开当前 `previewUrl`（与列表「预览」列一致）。改代码后需 **保存** 再预览，才能看到最新 body。

## 与设置、在线编辑的关系

| 操作 | 位置 |
|------|------|
| 改 `urlPath`、发布、Meta、本页 Style/Script 列表 | [页面设置](./pages-setting.md) |
| 改 body HTML | **本页** |
| 点选 DOM 改文案/图/链 | [在线编辑](./pages-inline-edit.md) |

大段结构调整用本页；运营微调可用在线编辑，二者改的都是同一 Page 资源。

## 相关

| 文档 | 说明 |
|------|------|
| [k.site.pages](../../api/site/page.md) | 脚本 `add` / `updateBody` 等 |
| [View](../../templateEngine/view/index.md) | `<view id="...">` |
| [媒体库](./media.md) | 图片路径 |
