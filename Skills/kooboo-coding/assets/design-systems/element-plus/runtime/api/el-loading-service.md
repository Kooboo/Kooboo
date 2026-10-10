# ElLoadingService

Imperative loading overlay service; close it when the request settles.

Kind: service. Import `ElLoadingService` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

## Usage and state ownership

Call ElLoadingService({ target, fullscreen, lock, text, background }); target defaults to document.body and fullscreen defaults to true. Returns an instance with close(). Use try/finally to close an overlay. This is a service, not a template component.

```js
const loading = ElLoadingService({ target: panel, fullscreen: false, text: 'Loading' });
try { await loadRecords(); } finally { loading.close(); }
```
