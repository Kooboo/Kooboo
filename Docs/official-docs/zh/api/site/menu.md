# k.site.menus

> 站点前台导航菜单的读取与维护（与后台 **开发 → 菜单** 同一套数据）

## 概述

`k.site.menus` 管理 Kooboo **Menu** 资源：在后台 [开发 → 菜单](../../cms/development/menus.md) 中创建的菜单组、多级菜单项，以及用于输出的 HTML 模板。定义本身**不会**自动出现在页面上，须在 **Layout / Page / View** 中通过本 API 或 `<menu>` 组件引用。

::: tip 与「编辑菜单」的区别
| 对象 | 入口 | 作用 |
|------|------|------|
| **开发 → 菜单**（本文 API） | `/_Admin/development/menus` | 访客看到的站点导航 |
| **编辑菜单** | 右上角用户菜单 | 控制 Kooboo **后台** 左侧功能树是否显示 |

详见 [站点后台菜单总览](../../cms/navigation.md#权限与编辑菜单)。
:::

::: warning 与 k.site.pages 的区别
| API | 用途 |
|-----|------|
| `k.site.menus` | 导航菜单树（名称、URL、子项、渲染模板） |
| `k.site.pages` | 可访问的页面资源与路由 |
:::

菜单项 URL 通常指向 [页面](../../cms/site/pages.md) 路径；多语言名称在后台按语言维护，脚本侧用 `getName(culture)` 读取。

## TypeScript 定义（节选）

```ts
interface KMenus {
  get(nameOrId: string): Menu;
  create(name: string): Menu;
  list(): Menu[];
  remove(id: string): void;
}

interface Menu {
  id: string;
  name: string;
  url: string;
  source: string;
  children: Menu[];
  getName(culture: string): string;
  createSubMenu(name: string, url: string): void;
  updateSubMenu(
    subMenuId: string,
    name: string,
    url: string,
    culture?: string
  ): void;
  removeSubMenu(subMenuId: string): void;
}
```

## get()

按菜单 **名称** 或 **Id**（GUID 字符串）获取根菜单对象（含 `children` 树）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 根菜单名称或 Id |

**返回：** `Menu | null`。

```ts
k.api.get(() => {
    const menu = k.site.menus.get("main")
    if (!menu) return null
    return {
        name: menu.name,
        topLevel: (menu.children || []).map((c) => ({
            name: c.name,
            url: c.url
        }))
    }
})
```

::: tip
`nameOrId` 须与后台列表中的 **名称** 一致。菜单不存在时行为取决于运行时实现，调用前请确认已在后台创建。
:::

## list()

返回站点全部根菜单对象数组（不含单独展开子树以外的扁平列表）。

**参数：** 无。

**返回：** `Menu[]`。

```ts
k.api.get(() => {
    const menus = k.site.menus.list()
    return { count: menus.length, names: menus.map((m) => m.name) }
})
```

## create()

创建新的根菜单；名称已存在时抛出异常。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `name` | `string` | 是 | 根菜单名称 |

**返回：** `Menu`。

```ts
k.api.post(() => {
    const name = "doc-menu-" + Date.now()
    const menu = k.site.menus.create(name)
    return { id: menu.id, name: menu.name }
})
```

## remove()

按根菜单 **Id**（GUID 字符串）删除整棵菜单。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `id` | `string` | 是 | 根菜单 Id |

**返回：** `void`。

```ts
k.api.post(() => {
    const menu = k.site.menus.create("doc-menu-del")
    k.site.menus.remove(menu.id)
    return { removed: true }
})
```

## Menu 实例

对 `get()` / `create()` / `list()` 返回的对象，可读取字段并在运行时维护子项（会写回站点数据库）。

| 成员 | 说明 |
|------|------|
| `id` | 当前节点 Id |
| `name` | 显示名称（默认语言） |
| `url` | 链接；根菜单项常为 `#` 或空 |
| `children` | 子菜单项数组 |
| `source` | 固定为 `"menu"`，用于数据追踪 |
| `getName(culture)` | 指定语言下的显示名；无翻译时回退 `name` |

### createSubMenu(name, url)

在当前节点下新增子项并保存。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `name` | `string` | 是 | 子菜单显示名称 |
| `url` | `string` | 是 | 子菜单链接 |

**返回：** `void`。

```ts
k.api.post(() => {
    const root = k.site.menus.get("main")
    root.createSubMenu("新栏目", "/new-section")
    return { childCount: root.children.length }
})
```

### updateSubMenu(subMenuId, name, url, culture?)

按子项 Id 更新名称或 URL；传入 `culture` 时只更新该语言的 `Values`，默认语言同时更新 `name`。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `subMenuId` | `string` | 是 | 子菜单 Id |
| `name` | `string` | 是 | 新显示名称 |
| `url` | `string` | 是 | 新链接 |
| `culture` | `string` | 否 | 文化代码 |

**返回：** `void`。

```ts
k.api.post(() => {
    const root = k.site.menus.get("main")
    const child = root.children[0]
    root.updateSubMenu(child.id, "Updated", "/updated")
    return { updated: true }
})
```

### removeSubMenu(subMenuId)

按子项 Id 删除子菜单项。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `subMenuId` | `string` | 是 | 子菜单 Id |

**返回：** `void`。

```ts
k.api.post(() => {
    const root = k.site.menus.get("main")
    const child = root.children[0]
    root.removeSubMenu(child.id)
    return { removed: true }
})
```

## 前台使用方式

后台配好菜单后，在模板中任选一种方式输出（可组合：顶栏用组件、页脚用脚本）。

### 方式一：`<menu>` 组件（推荐，沿用后台模板）

在 **Layout** 或 **Page** 中插入 Kooboo **Menu** 组件：标签名为 `menu`，`id` 为后台菜单 **名称**。渲染时使用 [开发 → 菜单](../../cms/development/menus.md#菜单模板弹窗) 中配置的 HTML 模板与占位符（`{href}`、`{anchortext}`、`{items}` 等），并自动处理当前页高亮与多语言 URL。

```html
<!-- 输出名为 main 的整棵菜单（深度默认不限） -->
<menu id="main"></menu>

<!-- 仅渲染前 2 层 -->
<menu id="main" menulevel="2"></menu>
```

在布局/页面设计器的组件面板中拖入 **Menu** 效果相同。`id` 错误或菜单不存在时，该位置输出为空。

### 方式二：`k.site.menus` + `k-for`（自定义 HTML）

在服务端脚本中取出菜单，再用 [模板绑定语法](../../templateEngine/binding/index.md) 的 `k-for` 渲染，结构完全由模板控制。

```html
<script env="server">
    var nav = k.site.menus.get("main");
    var items = nav && nav.children ? nav.children : [];
</script>
<nav class="site-nav">
    <ul>
        <li k-for="item in items">
            <a k-attribute="href {item.url}" k-content="item.name"></a>
        </li>
    </ul>
</nav>
```

多级子菜单可对 `item.children` 再套一层 `k-for`，或拆成 [View](../../templateEngine/view/index.md) 片段递归引用。

多语言显示名示例：

```html
<script env="server">
    var culture = k.site.multilingual.currentCulture;
    var nav = k.site.menus.get("main");
    var items = (nav && nav.children ? nav.children : []).map(function (c) {
        return { name: c.getName(culture), url: c.url };
    });
</script>
<ul>
    <li k-for="item in items">
        <a k-attribute="href {item.url}" k-content="item.name"></a>
    </li>
</ul>
```

### 方式三：Code / API 中维护菜单

在 CodeBlock 或 `k.api` 中调用 `create`、`createSubMenu` 等，适合安装向导、迁移脚本；日常导航仍建议在后台 **开发 → 菜单** 维护。

## 相关文档

- [k.site](./) — 站点资源总览
- [菜单（CMS）](../../cms/development/menus.md) — 后台创建菜单、菜单项、HTML 模板与权限
- [模板绑定语法](../../templateEngine/binding/index.md) — `<script env="server">`、`k-for`、`k-content`
- [k.site.pages](./page.md) — 菜单项常链接的页面资源
- [开发概述](../../cms/development/index.md) — 开发分组与菜单入口
