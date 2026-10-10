# ElMessage

Imperative transient message service; call it, do not mount it as a component.

Kind: service. Import `ElMessage` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

## Usage and state ownership

Call ElMessage({ message, type: 'success'|'warning'|'info'|'error', duration, showClose, grouping }); duration defaults to 3000 ms and 0 disables automatic closing. Returns a handler with close(). ElMessage.closeAll() closes messages. Use plain text or a Vue VNode; keep dangerouslyUseHTMLString false for business/user text. Business code owns whether a request succeeded. Component form is not exported by this adapter.

```js
const handle = ElMessage({ message: 'Saved', type: 'success' });
// handle.close();
```
