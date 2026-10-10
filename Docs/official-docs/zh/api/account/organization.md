# k.account.organization

> 当前站点所属组织及组织内用户、部门

## 概述

`k.account.organization.current` 对应当前站点绑定组织的 `OrganizationModel`；`get(nameOrId)` 按名称或 Id 读取其它组织。

## TypeScript 定义（节选）

```ts
interface KOrganization {
  current: OrganizationModel;
  get(nameOrId: string): OrganizationModel;
}

interface OrganizationModel {
  id: string;
  name: string;
  displayName: string;
  serverLevel: number;
  currency: string;
  isPartner: boolean;
  departments: DepartmentModel[];
  users: UserModel[];
  organizationUsers: UserModel[];
  addDepartment(name: string, displayName: string): void;
  addOrUpdateOrganizationUser(name: string, password: string, isAdmin: boolean): User;
  deleteOrganizationUser(name: string): boolean;
  getOrganizationUser(name: string): User;
}
```

## current

```ts
k.api.get(() => {
    const org = k.account.organization.current
    if (!org) return null
    return {
        name: org.name,
        displayName: org.displayName,
        userCount: org.users.length,
        departmentCount: org.departments.length
    }
})
```

## current.users

组织下的用户列表（`UserModel[]`）。

```ts
k.api.get(() => {
    const users = k.account.organization.current.users
    return {
        count: users.length,
        names: users.slice(0, 10).map((u) => u.userName)
    }
})
```

## current.organizationUsers

仅包含用户名以 `@组织Id` 结尾的**组织子账号**用户。

```ts
k.api.get(() => {
    const users = k.account.organization.current.organizationUsers
    return { count: users.length }
})
```

## current.departments

```ts
k.api.get(() => {
    const deps = k.account.organization.current.departments
    return deps.map((d) => ({ name: d.name, display: d.display }))
})
```

## get()

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `nameOrId` | `string` | 是 | 组织名称或组织 ID。 |

**返回值**：`OrganizationModel | null`。匹配到的组织；不存在时为空值。

```ts
k.api.get(() => {
    const org = k.account.organization.get("org-name-or-id")
    return org ? { name: org.name, displayName: org.displayName } : null
})
```

## addDepartment()

在当前组织下新增部门（需相应权限）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `name` | `string` | 是 | 部门名称。 |
| `displayName` | `string` | 是 | 部门显示名称。 |

**返回值**：`void`。

```ts
k.api.post(() => {
    k.account.organization.current.addDepartment(
        k.request.form.name,
        k.request.form.displayName
    )
    return { ok: true }
})
```

## addOrUpdateOrganizationUser()

创建或更新组织子账号（用户名一般为 `name@组织Id` 形式）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `name` | `string` | 是 | 组织子账号名称。 |
| `password` | `string` | 是 | 登录密码。 |
| `isAdmin` | `boolean` | 是 | 是否为组织管理员。 |

**返回值**：`User | null`。创建或更新后的用户信息。

```ts
k.api.post(() => {
    const user = k.account.organization.current.addOrUpdateOrganizationUser(
        k.request.form.name,
        k.request.form.password,
        k.request.form.isAdmin === "true"
    )
    return { userName: user?.userName }
})
```

## deleteOrganizationUser() / getOrganizationUser()

| 方法 | 参数 | 返回值 | 说明 |
|------|------|--------|------|
| `deleteOrganizationUser(name)` | `name: string` | `boolean` | 删除指定组织子账号，成功时返回 `true`。 |
| `getOrganizationUser(name)` | `name: string` | `User | null` | 读取指定组织子账号；不存在时为空值。 |

```ts
k.api.post(() => {
    const name = k.request.form.name
    const deleted = k.account.organization.current.deleteOrganizationUser(name)
    return { deleted }
})
```

```ts
k.api.get(() => {
    const user = k.account.organization.current.getOrganizationUser(
        k.request.queryString.name
    )
    return user ? { userName: user.userName } : null
})
```

## 相关文档

- [k.account 概述](./index.md)
- [用户](./user.md)
- [登录](./login.md)
