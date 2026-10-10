# k.utils.CDN

> Current site CDN switch and cache time

## Overview

`k.utils.CDN` controls CDN behavior in the current request context (attribute name **`CDN`**, case sensitive).

## disableCDN() / enableCDN()

Turn off or enable CDN.

| Method | Parameter | Return | Description |
|------|------|------|------|
| `disableCDN()` | None | `void` | Do not use CDN for the current response |
| `enableCDN()` | None | `void` | Allow CDN for the current response |

```ts
k.api.get("preview", () => {
    k.utils.CDN.disableCDN()
    return {
        draft: k.request.queryString.get("id"),
        generatedAt: k.utils.date.formate(k.utils.date.now)
    }
})
```

## setCacheTime()

Set CDN cache minutes.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `minutes` | `number` | Yes | CDN cache time in minutes |

**Returns:** `void`.

```ts
k.utils.CDN.enableCDN()
k.utils.CDN.setCacheTime(60)
```

For public content APIs, explicitly enable CDN and set the cache duration:

```ts
k.api.get("public-summary", () => {
    k.utils.CDN.enableCDN()
    k.utils.CDN.setCacheTime(15)

    return {
        title: "Public summary",
        cachedForMinutes: 15
    }
})
```

## Related Docs

- [k.utils Overview](./index.md)
