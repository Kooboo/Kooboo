# k.account.user

> Current platform user information and user query

## Overview

`k.account.user` reads the current user through `current` when logged in; `get` / `exists` / `emailExists` are used for querying. Use `changeOrganization` to switch organizations.

For login related API, see [login.md](./login.md).

## TypeScript Definition

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

`UserModel` for the currently logged in user; `null` when not logged in.

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

Get the user by username (the password field in the returned object is cleared).

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `userName` | `string` | Yes | Platform username. |

**Returns**: `User | null`. Matching user, or `null` when it does not exist.

```ts
k.api.get(() => {
    const user = k.account.user.get("some-user")
    return user ? { userName: user.userName } : null
})
```

## exists() / emailExists()

| Method | Parameters | Returns | Description |
|--------|------------|---------|-------------|
| `exists(userName)` | `userName: string` | `boolean` | Whether the username exists. |
| `emailExists(email)` | `email: string` | `boolean` | Whether the email exists. |

```ts
k.api.get(() => {
    return {
        byName: k.account.user.exists("some-user"),
        byEmail: k.account.user.emailExists("a@example.com")
    }
})
```

## isOfOrganization()

Determine whether the current user belongs to the specified organization (name or organization Id string).

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `organizationName` | `string` | Yes | Organization name or organization ID. |

**Returns**: `boolean`. Returns `true` when the current user belongs to the organization.

```ts
k.api.get(() => {
    return {
        inOrg: k.account.user.isOfOrganization("my-org")
    }
})
```

## changeOrganization()

Switch the current user to the specified organization (must have access to the organization).

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `organizationName` | `string` | Yes | Organization name or organization ID to switch to. |

**Returns**: `User | null`. User information after switching, or an empty value on failure.

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

Change the current user password on `user.current`.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `oldPassword` | `string` | Yes | Current password. |
| `newPassword` | `string` | Yes | New password. |

**Returns**: `boolean`. Returns `true` when the password is changed.

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

## Related Docs

- [k.account Overview](./index.md)
- [Login](./login.md)
- [Organization](./organization.md)
