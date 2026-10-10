# k.session

> Session-level caching - stores session data on the server side

## Overview

`k.session` provides session-level caching function. Data is stored on the server side and can be accessed across requests during the same session.

::: The difference between tip and cookie
- **Session**: stored on the server side, suitable for storing large amounts of data or sensitive information
- **Cookie**: stored on the client, suitable for storing a small amount of simple data
:::

## life cycle

| Configuration | value |
|------|-----|
| Cookie name | `_kb_session_id` |
| Cookie expiration | 7 days |
| Server expiration | Automatically cleared after **30 minutes of inactivity** |

**illustrate**:
- Closing the browser will not clear the session (Cookie is valid for 7 days)
- If there is no operation within 30 minutes, the server data will be cleared
- A new session will be created when accessing again

## TypeScript definition

```ts
interface Session {
  keys: string[];       // All key names
  values: any[];        // All values
  length: number;       // Number of key-value pairs

  set(key: string, value: any): void;      // Set value
  get(key: string): any;                    // Get value
  containsKey(key: string): boolean;        // Check whether the key exists
  remove(key: string): boolean;              // Remove key
  clear(): void;                            // Clear all entries
}
```

## Methods

### set()

Set session value.

```ts
k.api.get(() => {
    k.session.set("username", "kooboo")
    k.session.set("role", "admin")
    return "session set"
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `key` | `string` | yes | Session key |
| `value` | `any` | yes | Value to save |

**Returns:** `void`.

### get()

Get session value.

```ts
k.api.get(() => {
    const username = k.session.get("username")
    return { username }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `key` | `string` | yes | Session key |

**Returns:** `any`, the session value for the key; empty or `undefined` when missing depending on the runtime.

### containsKey()

Check if the key exists.

```ts
k.api.get(() => {
    k.session.set("token", "abc123")
    const hasToken = k.session.containsKey("token")
    return { hasToken }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `key` | `string` | yes | Session key |

**Returns:** `boolean`.

### remove()

Delete the specified key.

```ts
k.api.get(() => {
    k.session.set("temp", "data")
    const removed = k.session.remove("temp")
    return { removed, stillExists: k.session.containsKey("temp") }
})
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `key` | `string` | yes | Session key |

**Returns:** `boolean`, whether the key was removed successfully.

### clear()

Clear all session data.

```ts
k.api.get(() => {
    k.session.set("key1", "value1")
    k.session.clear()
    return "cleared"
})
```

**Parameters:** None.

**Returns:** `void`.

## Properties

### keys

Get a list of all key names.

```ts
k.api.get(() => {
    k.session.set("key1", "value1")
    k.session.set("key2", "value2")
    return { keys: k.session.keys }
})
// Returns: { "keys": ["key1", "key2"] }
```

### values

Get a list of all values.

```ts
k.api.get(() => {
    k.session.set("key1", "value1")
    k.session.set("key2", "value2")
    return { values: k.session.values }
})
// Returns: { "values": ["value1", "value2"] }
```

### length

Get the number of key-value pairs.

```ts
k.api.get(() => {
    k.session.set("key1", "value1")
    k.session.set("key2", "value2")
    return { length: k.session.length }
})
// Returns: { "length": 2 }
```

## Complete Example

### User login status

```ts
// Login
k.api.post(() => {
    const { username, password } = JSON.parse(k.request.body)

    // Validate user...
    const user = { id: "123", name: username, role: "admin" }

    // Store the session
    k.session.set("user", JSON.stringify(user))

    return { success: true, user }
})

// Check login status
k.api.get(() => {
    const userStr = k.session.get("user")
    if (userStr) {
        const user = JSON.parse(userStr)
        return { loggedIn: true, user }
    }
    return { loggedIn: false, user: null }
})

// Log out
k.api.get(() => {
    k.session.remove("user")
    return { success: true }
})
```

## Related Docs

- [k.cookie](../cookie/) - Cookie Management
- [k.request](../request/) - HTTP request information
