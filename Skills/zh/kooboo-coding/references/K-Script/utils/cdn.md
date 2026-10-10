# k.utils.CDN

> 控制当前响应的 CDN 缓存

`CDN` 为 PascalCase。应在写出响应内容前调用这些方法。当前运行时的 `enableCDN()` 实现也调用了禁用操作；在修复该运行时缺陷前，不要依赖它启用 CDN 缓存。

```ts
k.utils.CDN.disableCDN();
k.utils.CDN.enableCDN();
k.utils.CDN.setCacheTime(60); // 分钟
```

| 方法 | 用途 |
| --- | --- |
| `disableCDN()` | 禁用当前响应的 CDN 缓存 |
| `enableCDN()` | 设计意图是启用 CDN 缓存，但当前实现实际调用了禁用操作 |
| `setCacheTime(minutes)` | 设置 CDN 缓存分钟数 |
