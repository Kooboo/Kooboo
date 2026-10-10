# 站点用户

> 菜单：**站点设置 → 用户管理**  
> 深链：`/_Admin/system/siteuser?SiteId={站点GUID}`

**站点用户**将当前 **组织（Organization）** 下的成员加入本站，并为其指定 **站点角色**。被加入的用户可登录 Kooboo 后台并进入本站点；其能看到的菜单与可执行操作由 [角色权限](./roles.md) 中对应角色的 **feature** 权限决定。

::: tip 权限
| 操作 | 权限 |
|------|------|
| 查看用户列表 | `siteUser` |
| **添加用户** | `siteUser·edit` |
| 多选 **删除** | `siteUser·delete` |
:::

::: info 与组织管理员
**组织管理员**（`Organization.AdminUser`）默认可管理全组织站点，**不会**出现在「添加用户」的可用用户下拉中，也通常不在本列表展示。其余组织成员须在本页显式添加后，才能访问本站后台（`WebsiteType.m` 站点）。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **站点设置 → 用户管理**。  
2. 左侧 **站点设置 → 用户管理**。

<DocImage src="/cms/settings/settings-site-users-overview.png" alt="站点用户" width="1120" />

## 工具栏

| 控件 | 说明 |
|------|------|
| **添加用户** | 打开 [添加用户弹窗](#添加用户)，从组织未加入本站的成员中选择用户并指定角色 |

<DocImage src="/cms/settings/settings-site-users-toolbar.png" alt="站点用户工具栏" width="1120" />

## 用户列表

| 列 | 说明 |
|----|------|
| **用户名** | 组织账号 `userName` |
| **邮箱** | 对应 `email` |
| **角色** | 本站 `SiteRole` 名称，以标签展示（如 `master`、`developer`、自定义角色名） |

多选可 **删除**，将成员从本站移除（`SiteUser/Deletes`）；**不会**删除组织账号本身。

<DocImage src="/cms/settings/settings-site-users-list.png" alt="站点用户列表" width="1120" />

当前列表 UI **不支持** 行内修改角色；若需变更角色，可先删除再重新添加，或通过 API / 脚本更新（见下文）。

## 添加用户

<DocImage src="/cms/settings/settings-site-users-add-dialog.png" alt="添加用户弹窗" width="1120" />

| 字段 | 说明 |
|------|------|
| **可用用户** | 当前组织中 **尚未加入本站** 的成员（下拉显示 `用户名<邮箱>`）；组织管理员不在此列表 |
| **角色** | 本站已定义角色：`RolePermission` 自定义角色，以及内置 **master**、**developer**、**contentmanager** |

确认后调用 `SiteUser/AddUser`，写入 `UserId` 与 `SiteRole` 并刷新列表。

## 角色与菜单

| 角色类型 | 说明 |
|----------|------|
| **内置角色** | `master`（全权限）、`developer`、`contentmanager`；定义见服务端 `PermissionService.EmbeddedRoles` |
| **自定义角色** | 在 [角色权限](./roles.md) 中创建，按 feature / action 勾选权限 |

用户登录后，左侧菜单根据角色过滤（如运营、设计师、开发者预设可见项）；细粒度 API 与后台按钮仍受角色 **Permissions** 约束。

## 与角色权限的关系

| | **站点用户（本页）** | [角色权限](./roles.md) |
|--|---------------------|-------------------------|
| 作用 | 把 **谁** 加入站点，并绑定 **角色名** | 定义 **角色** 能访问哪些 feature |
| 典型操作 | 添加协作者、移除离职成员 | 新建「内容编辑」角色并勾选 `content` 等 |

先配置角色，再在本页把成员分配到对应角色。

## 典型工作流

1. 在组织层邀请或创建成员账号。  
2. 在 [角色权限](./roles.md) 中确认或新建所需角色。  
3. 本页 **添加用户**，选择成员与角色。  
4. 让对方登录后台，核对可见菜单与关键操作（如发布、域名编辑）是否符合预期。  
5. 成员离项时 **删除** 其站点用户记录。

## 脚本侧（可选）

Code 中可通过 **`k.site.user.addUser`** 以编程方式添加用户（须用户已存在于组织）：

```javascript
k.site.user.addUser({
  nameOrId: 'collaborator',
  role: 'developer'
})
```

站点包导入/导出可能包含 `siteuser` 条目；协作同步场景见运维文档。

## 相关

| 文档 | 说明 |
|------|------|
| [站点设置概述](./index.md) | 菜单总览 |
| [角色权限](./roles.md) | 角色与 feature 权限 |
| [编辑菜单](../navigation.md#权限与编辑菜单) | 控制左侧菜单显示 |
