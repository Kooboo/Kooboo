# ElMessageBox

Promise-based alert, confirmation and prompt service.

Kind: service. Import `ElMessageBox` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

## Usage and state ownership

Call ElMessageBox.confirm(message, title, options), .alert(...) or .prompt(...). Confirmation resolves on confirm and rejects on cancel/close; treat cancellation as a normal outcome and keep request failures separate. prompt resolves { value, action }. Set distinguishCancelAndClose when different outcomes matter. Never enable dangerouslyUseHTMLString for untrusted text. Keep validation and mutation in the business module.

```js
try { await ElMessageBox.confirm('Deactivate this record?', 'Confirm'); } catch { return; }
await deactivateRecord();
```
