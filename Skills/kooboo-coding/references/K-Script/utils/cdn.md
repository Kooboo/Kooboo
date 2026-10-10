# k.utils.CDN

> CDN cache control for the current response

`CDN` is PascalCase. Call these methods before writing response content. The current runtime implementation of `enableCDN()` also calls the disable operation; do not rely on it to enable CDN caching until that runtime defect is fixed.

```ts
k.utils.CDN.disableCDN();
k.utils.CDN.enableCDN();
k.utils.CDN.setCacheTime(60); // minutes
```

| Method | Purpose |
| --- | --- |
| `disableCDN()` | Disable CDN caching for the current response |
| `enableCDN()` | Intended to enable CDN caching, but currently invokes the disable operation |
| `setCacheTime(minutes)` | Set the CDN cache duration in minutes |
