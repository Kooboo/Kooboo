# 角色权限

> 菜单：**站点设置 → 角色权限**（高级菜单）  
> 深链：`/_Admin/system/roles?SiteId={站点GUID}`

**角色权限**定义本站 **站点角色** 可访问的后台 **feature** 与 **action**（查看 / 编辑 / 删除等）。在 [站点用户](./site-users.md) 中为成员指定角色后，其登录后台时的菜单、按钮与 API 调用均受本页配置约束。

::: tip 权限
| 操作 | 权限 |
|------|------|
| 查看角色列表 | `role` |
| **添加角色**、编辑权限并 **保存** | `role·edit` |
| 多选 **删除** 自定义角色 | `role·delete` |
:::

::: info 高级菜单
本菜单在路由上标记为 `advanced: true`，除勾选 **站点设置** 分组外，还须能在 [编辑菜单](../navigation.md#权限与编辑菜单) 中看到 **角色权限** 子项（或由 `master` 等全权限角色访问）。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **站点设置 → 角色权限**。  
2. 左侧 **站点设置 → 角色权限**。

<DocImage src="/cms/settings/settings-roles-overview.png" alt="角色权限" width="1120" />

## 工具栏

| 控件 | 说明 |
|------|------|
| **添加角色** | 打开 [添加角色弹窗](#添加或编辑角色)，新建自定义角色并勾选权限 |

<DocImage src="/cms/settings/settings-roles-toolbar.png" alt="角色权限工具栏" width="1120" />

## 角色列表

| 列 | 说明 |
|----|------|
| **角色名** | `name`，以标签展示 |
| **编辑** | 铅笔按钮，打开权限编辑弹窗 |

列表含 **内置角色** 与 **自定义角色**，按创建时间倒序。内置三项 **`master`**、**`developer`**、**`contentmanager`** 的复选框 **不可选**（不可批量删除），但仍可 **编辑** 其权限并保存到站点的 `RolePermission` 仓库。

<DocImage src="/cms/settings/settings-roles-list.png" alt="角色列表" width="1120" />

### 内置角色（概览）

| 角色名 | 说明 |
|--------|------|
| **master** | 默认拥有全部已注册 feature 权限（全选） |
| **developer** | 预置开发相关 feature（页面、Code、数据库、服务集成、Hook 等） |
| **contentmanager** | 预置内容与电商运营相关 feature（内容、媒体、商品等） |

具体 feature 集合以 Kooboo 版本内 `PermissionService.EmbeddedRoles` 为准；可在后台 **编辑** 查看或微调后保存。

### 删除限制

- **内置角色** 不可删除。  
- **自定义角色** 若仍有 [站点用户](./site-users.md) 的 `SiteRole` 引用，删除时会报错（*The role is in use and cannot be deleted*）。

## 添加或编辑角色

<DocImage src="/cms/settings/settings-roles-add-dialog.png" alt="添加角色弹窗" width="1120" />

编辑已有角色时弹窗标题为 **编辑角色**，**角色名** 输入框 **禁用**（不可改名）；新建时须填写唯一名称（1～50 字符，且不能与内置角色名冲突）。

<DocImage src="/cms/settings/settings-roles-edit-dialog.png" alt="编辑角色权限" width="1120" />

### 权限面板

| 区域 | 说明 |
|------|------|
| **全选** | 勾选/取消当前角色全部 permission |
| **Feature 列** | 每项对应一个 `feature`（如 `pages`、`content`、`domain`），文案来自后台国际化（如「页面」「域名管理」「服务集成」） |
| **单项复选框** | 仅含一种 action 的 feature 直接勾选 |
| **带下拉的 Feature** | 点击标签弹出 **查看 / 编辑 / 删除**（及 `debug` 等）子项；可对该 feature **全选** |

勾选 **编辑**、**删除** 或 **debug** 时，会自动勾选同 feature 的 **查看**；若子项仍选中，则 **查看** 不可单独取消。含多种 action 的 feature 点击标签可展开子复选框（如 **查看 / 编辑 / 删除**）。

保存调用 `Role/post`，写入站点 `RolePermissionRepository`。

## feature 与后台菜单

权限项由服务端 `PermissionService.GetList()` 汇总各 API 的 `[Permission]` 特性生成；新增 Kooboo 版本可能增加 feature。常见对应关系示例：

| feature（部分） | 后台能力 |
|-----------------|----------|
| `pages` | 页面设计与路由 |
| `content` / `contentType` | 内容与数据类型 |
| `database` | 数据库菜单 |
| `config` | [服务集成](./integrations/) |
| `domain` | [域名管理](./domains.md) |
| `siteUser` / `role` | [站点用户](./site-users.md)、本页 |
| `frontEvents` / `backendEvents` | [请求 Hook](./request-hooks.md)、[操作 Hook](./action-hooks.md) |
| `commerce*` 系列 | 电商各子菜单 |

左侧菜单可见性还会受前端 **角色菜单过滤**（运营/设计师/开发者预设）影响；API 与按钮仍以本页勾选的 **Permissions** 为准。

## 与站点用户的关系

| | **角色权限（本页）** | [站点用户](./site-users.md) |
|--|---------------------|------------------------------|
| 回答 | 某角色 **能做什么** | **谁** 拥有某角色 |
| 顺序 | 通常 **先** 定义或确认角色 | **再** 把成员加入站点并选角色 |

## 典型工作流

1. 查看内置 `developer` / `contentmanager` 是否满足团队分工，必要时 **编辑** 微调。  
2. 若无合适预设，**添加角色**（如 `content-editor`），按 feature 勾选权限并 **保存**。  
3. 在 [站点用户](./site-users.md) 把协作者加入本站并分配角色。  
4. 让对方登录验证菜单与关键操作；权限不足时回到本页调整。  
5. 停用角色前，先移除或改派所有引用该角色的站点用户，再 **删除** 自定义角色。

## 相关

| 文档 | 说明 |
|------|------|
| [站点设置概述](./index.md) | 菜单总览 |
| [站点用户](./site-users.md) | 成员与角色绑定 |
| [编辑菜单](../navigation.md#权限与编辑菜单) | 菜单显示与高级项 |
