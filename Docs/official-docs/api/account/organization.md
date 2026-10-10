# k.account.organization

> The organization to which the current site belongs and the users and departments within the organization

## Overview

`k.account.organization.current` corresponds to `OrganizationModel` of the current site-bound organization; `get(nameOrId)` reads other organizations by name or Id.

## TypeScript Definition

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

List of users under the organization (`UserModel[]`).

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

Only include **organization subaccount** users whose username ends with `@OrganizationId`.

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

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `nameOrId` | `string` | Yes | Organization name or organization ID. |

**Returns**: `OrganizationModel | null`. Matching organization, or an empty value when it does not exist.

```ts
k.api.get(() => {
    const org = k.account.organization.get("org-name-or-id")
    return org ? { name: org.name, displayName: org.displayName } : null
})
```

## addDepartment()

Add a new department under the current organization (requires corresponding permissions).

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `name` | `string` | Yes | Department name. |
| `displayName` | `string` | Yes | Department display name. |

**Returns**: `void`.

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

Create or update an organization sub-account (the username is generally in the form `name@OrganizationId`).

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `name` | `string` | Yes | Organization sub-account name. |
| `password` | `string` | Yes | Login password. |
| `isAdmin` | `boolean` | Yes | Whether the user is an organization administrator. |

**Returns**: `User | null`. Created or updated user information.

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

| Method | Parameters | Returns | Description |
|--------|------------|---------|-------------|
| `deleteOrganizationUser(name)` | `name: string` | `boolean` | Delete the organization sub-account. Returns `true` on success. |
| `getOrganizationUser(name)` | `name: string` | `User | null` | Read the organization sub-account, or an empty value when it does not exist. |

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

## Related Docs

- [k.account Overview](./index.md)
- [User](./user.md)
- [Login](./login.md)
