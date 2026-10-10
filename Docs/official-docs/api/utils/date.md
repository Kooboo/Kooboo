# k.utils.date

> Date time and formatting

## Overview

`k.utils.date` provides operations and formatting between the current time and `DateTime`.

## Properties

| property | illustrate |
|------|------|
| `utcNow` | UTC current time |
| `now` | local current time |

**Type:** `Date`.

## format()

Format date (the method name is spelled `formate` in the source code).

| Parameter | Type | Required | Description |
|------|------|------|------|
| `dateTime` | `Date` | Yes | Date/time to format |
| `formate` | `string` | No | Format string; defaults to `yyyy-MM-dd HH:mm:ss` |

**Returns:** `string`.

```ts
const s = k.utils.date.formate(k.utils.date.now, "yyyy-MM-dd")
const s2 = k.utils.date.formate(k.utils.date.now) // Default format
```

## addSeconds/addMinutes/addHours/addDays/addMonths/addYears

Offset the given `DateTime` and return the new time.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `dateTime` | `Date` | Yes | Base date/time |
| `value` | `number` | Yes | Offset amount; `addMonths` and `addYears` use integer values |

**Returns:** `Date`, the offset date/time.

```ts
const tomorrow = k.utils.date.addDays(k.utils.date.now, 1)
const expiresAt = k.utils.date.addMinutes(k.utils.date.now, 10)
```

## Related Docs

- [k.utils Overview](./index.md)
