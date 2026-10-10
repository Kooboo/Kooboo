# ElNotification

Imperative notification service with explicit close handler.

Kind: service. Import `ElNotification` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

## Usage and state ownership

Call ElNotification({ title, message, type, duration, position, onClick, onClose }); duration defaults to 4500 ms, 0 keeps it open. position is top-right/top-left/bottom-right/bottom-left. Returns close(); ElNotification.closeAll() closes all. Keep message as text or VNode and do not inject untrusted HTML.

```js
ElNotification({ title: 'Export ready', message: 'Download your report.', type: 'success' });
```
