# k.account.user

> 当前平台用户信息与用户查询

## 概述

`k.account.user` 在**已登录**时通过 `current` 读取当前用户；`get` / `exists` / `emailExists` 用于查询。切换组织用 `changeOrganization`。

登录相关 API 见 [login.md](./login.md)。

## TypeScript 定义（节选）

```ts
interface KUser {
  current: UserModel;
  isOfOrganization(organizationName: string): boolean;
  changeOrganization(organizationName: string): User;
  get(userName: string): User | null;
  exists(userName: string): boolean;
  emailExists(email: string): boolean;
}

interface UserModel {
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  language: string;
  currentOrgId: string;
  isAdmin: boolean;
  id: string;
  departments: DepartmentModel[];
  changePassword(oldPassword: string, newPassword: string): boolean;
}
```

## current

当前登录用户的 `UserModel`；未登录时为 `null`。

```ts
k.api.get(() => {
    if (!k.account.isLogin) {
        return { isLogin: false }
    }
    const u = k.account.user.current
    return {
        userName: u.userName,
        email: u.email,
        isAdmin: u.isAdmin
    }
})
```

## get()

按用户名获取用户（返回对象中密码字段已清空）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `userName` | `string` | 是 | 平台用户名。 |

**返回值**：`User | null`。匹配到的用户；不存在时返回 `null`。

```ts
k.api.get(() => {
    const user = k.account.user.get("some-user")
    return user ? { userName: user.userName } : null
})
```

## exists() / emailExists()

| 方法 | 参数 | 返回值 | 说明 |
|------|------|--------|------|
| `exists(userName)` | `userName: string` | `boolean` | 用户名是否已存在。 |
| `emailExists(email)` | `email: string` | `boolean` | 邮箱是否已存在。 |

```ts
k.api.get(() => {
    return {
        byName: k.account.user.exists("some-user"),
        byEmail: k.account.user.emailExists("a@example.com")
    }
})
```

## isOfOrganization()

判断当前用户是否属于指定组织（名称或组织 Id 字符串）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `organizationName` | `string` | 是 | 组织名称或组织 ID。 |

**返回值**：`boolean`。当前用户属于该组织时返回 `true`。

```ts
k.api.get(() => {
    return {
        inOrg: k.account.user.isOfOrganization("my-org")
    }
})
```

## changeOrganization()

将当前用户切换到指定组织（须有权访问该组织）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `organizationName` | `string` | 是 | 要切换到的组织名称或组织 ID。 |

**返回值**：`User | null`。切换后的用户信息；失败时为空值。

```ts
k.api.post(() => {
    const org = k.request.form.organization
    const user = k.account.user.changeOrganization(org)
    return {
        ok: !!user,
        currentOrgId: k.account.user.current?.currentOrgId
    }
})
```

## changePassword()

在 `user.current` 上修改当前用户密码。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `oldPassword` | `string` | 是 | 当前密码。 |
| `newPassword` | `string` | 是 | 新密码。 |

**返回值**：`boolean`。修改成功时返回 `true`。

```ts
k.api.post(() => {
    k.account.ensureLogin("/login")
    const ok = k.account.user.current.changePassword(
        k.request.form.oldPassword,
        k.request.form.newPassword
    )
    return { ok }
})
```

## 相关文档

- [k.account 概述](./index.md)
- [登录](./login.md)
- [组织](./organization.md)
