# 菜单（Menus）

> 菜单：**开发 → 菜单**  
> 深链：`/_Admin/development/menus?SiteId={站点GUID}`

管理站点 **导航菜单** 定义：创建菜单组、维护多级 **菜单项**（名称、链接、多语言），并配置输出 HTML **模板**（占位符渲染子项）。定义保存后须在 **Layout / Page / View** 中引用才会出现在访客站点上，见 [前台使用](#前台使用)。

::: info 不是后台左侧菜单
本页是 **访客站点** 的导航数据，与 [编辑菜单](../navigation.md#权限与编辑菜单)（控制 Kooboo 后台功能树）无关。
:::

::: tip 权限
菜单权限：`menu`（查看）。**新建菜单**、编辑项、拖拽排序、删项、改模板需 `menu·edit`；列表批量删除菜单需 `menu·delete`。行末 **版本** 需 `site·log`。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → 菜单**。
2. 左侧 **开发 → 菜单**。

<DocImage src="/cms/development/menus-overview.png" alt="菜单列表总览" width="1120" />

## 菜单列表

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **新建菜单** | `menu·edit` | 打开 [新建菜单弹窗](#新建菜单弹窗) |

<DocImage src="/cms/development/menus-toolbar.png" alt="菜单列表工具栏" width="1120" />

### 列表表格

| 列 | 说明 |
|----|------|
| **名称** | 点击进入 [编辑菜单](#编辑菜单) |
| **被引用** | 引用关系 |
| **最后修改** | 保存时间 |
| **版本** | [版本历史](../operations/site-logs.md) |

支持多选 **删除**整个菜单（`menu·delete`）。

<DocImage src="/cms/development/menus-list-table.png" alt="菜单列表" width="1120" />

### 新建菜单弹窗

填写 **名称**（必填、唯一，1～50 字符），确认后创建菜单并返回列表（再点名称进入编辑）。

<DocImage src="/cms/development/menus-add-dialog.png" alt="新建菜单弹窗" width="1120" />

## 编辑菜单

路径：`/_Admin/development/menu/edit?SiteId=...&id={MenuId}`

面包屑：**菜单 → {菜单名}**。卡片页头显示菜单名称；右侧 **编辑模板** 打开根级 [菜单模板弹窗](#菜单模板弹窗)（与有子项的菜单项上的模板按钮相同能力）。

<DocImage src="/cms/development/menus-edit-overview.png" alt="编辑菜单：菜单项树" width="1120" />

### 菜单项树

- 顶级与子级以 **可拖拽** 卡片展示：左侧为 **名称** 与 **URL**，右侧为操作图标。
- **拖拽**（`menu_move_handler`）调整同级顺序，保存时调用 `swap` 接口。
- 无子项时：可 **添加** 子项、**编辑**、**删除**。
- 有子项时：可 **编辑模板**（子菜单容器）、**添加** 子项、**编辑**、**删除**；底部 **+** 继续添加同级子项。

若尚无菜单项，页内引导 **立即创建** 第一个菜单项。

<DocImage src="/cms/development/menus-edit-items.png" alt="菜单项列表与操作" width="1120" />

### 菜单项弹窗

**添加菜单项** 或行内 **编辑** 打开（标题 **菜单项**）：

| 字段 | 说明 |
|------|------|
| **名称** | 多语言站点时按 [多语言](../site/pages.md) 配置显示多行 **名称**；默认语言为菜单项主名称 |
| **URL** | 下拉选择 **页面** 路由（`routesByType('Page')`），可筛选或手输；留空保存为 `#` |

右上角 **多语言选择器** 切换当前编辑的语言列。

<DocImage src="/cms/development/menus-item-dialog.png" alt="菜单项弹窗：名称与 URL" width="1120" />

### 菜单模板弹窗

**编辑模板** 打开（标题 **编辑模板**）。用于定义菜单项（含子级）的 HTML 输出结构：

| 区域 | 说明 |
|------|------|
| 上 / 下容器 | `subItemContainer` 中 `{items}` 上下的 HTML 片段 |
| 中间模板 | `subItemTemplate`，单条菜单项的 HTML |
| 占位标签 | 点击插入：`AnchorText` → `{anchortext}`、`Href` → `{href}`、`SubItems` → `{items}`、`ActiveClass` → `{activeclass:className}`、`ParentId` → `{parentid}`、`CurrentId` → `{currentid}` |
| **预览** | 右侧根据当前模板与菜单数据实时生成 HTML 预览 |

<DocImage src="/cms/development/menus-template-dialog.png" alt="菜单模板编辑与预览" width="1120" />

## 前台使用

后台保存菜单后，在 **公共 Layout** 或需要导航的 **Page / View** 中选用以下方式之一（无单独「启用」开关）。

### 使用 `<menu>` 组件（沿用本节模板）

在 HTML 中插入 **Menu** 组件，`id` 填写上表 **名称** 列中的菜单名（如 `main`）。输出结构由 [菜单模板弹窗](#菜单模板弹窗) 中的 `{href}`、`{anchortext}`、`{items}` 等占位符决定，并自动处理当前页高亮。

```html
<menu id="main"></menu>
```

可选属性 `menulevel` 限制渲染层级（如 `menulevel="2"`）。设计器组件面板拖入 **Menu** 等价。

### 使用 `k.site.menus` + `k-for`（自定义结构）

在 `<script env="server">` 中读取菜单，再用 [模板绑定语法](../../templateEngine/binding/index.md) 的 `k-for` 自行拼 HTML，适合与站点 CSS 强耦合的导航栏。

```html
<script env="server">
    var nav = k.site.menus.get("main");
    var items = nav && nav.children ? nav.children : [];
</script>
<ul>
    <li k-for="item in items">
        <a k-attribute="href {item.url}" k-content="item.name"></a>
    </li>
</ul>
```

方法说明、`createSubMenu`、多语言 `getName` 等见 **[k.site.menus](../../api/site/menu.md)**。请勿使用已弃用的 `k-query` / `v-query` 取菜单。

### 脚本动态维护（可选）

安装向导、迁移等场景可在 Code / `k.api` 中调用 `k.site.menus.create`、`menu.createSubMenu` 等；日常运营仍建议在本页维护。

## 相关

| 文档 | 说明 |
|------|------|
| [k.site.menus](../../api/site/menu.md) | KScript API：`get`、`list`、子项维护、前台示例 |
| [模板绑定语法](../../templateEngine/binding/index.md) | `k-for`、`k-content`、`k-attribute` |
| [开发概述](./index.md) | 开发菜单索引 |
| [页面](../site/pages.md) | 菜单项 URL 常指向页面 |
| [URL](./urls.md) | 站点路径与路由 |
| [编辑菜单](../navigation.md#权限与编辑菜单) | 后台左侧功能树（非本文） |
