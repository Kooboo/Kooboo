# YlConfirmDialog

Import and register `{ YlConfirmDialog }` from `/yuanli.js`.
Props: `modelValue: boolean` via v-model, `title: string` required, `description: string` required, `busy: boolean`, `danger: boolean`, `confirmLabel = '确认'`, `cancelLabel = '取消'`. Events: `update:modelValue`, `confirm` (no arguments).

```html
<YlConfirmDialog v-model="confirmOpen" title="确认下架" :description="'下架「' + target.name + '」后将停止销售，是否继续？'" confirm-label="确认下架" danger :busy="saving" @confirm="confirmStatusChange" />
<YlMessage v-model="message" />
```

For publish/unpublish, enable/disable and destructive actions: row click captures target and desired status, then opens this dialog. Do not call the mutation API before confirm. Set busy synchronously and guard repeated calls; busy disables confirm, cancel, close and Escape. Only a successful API response closes the dialog and updates/refreshes the row, then assigns a new message object `{ type: 'success', text: '已下架' }`. On rejection keep the confirmation open, clear busy and show `{ type: 'error', text: '下架失败，请重试' }`. Cancel never calls the API. Include the target name and actual consequence in description; never invent consequences.

The component does not auto-close on confirm and does not invoke business APIs. It reuses YlDialog focus containment and focus return. Default initial control is cancel when visible. tone=warning (info/success/warning/error) selects the semantic icon; danger takes precedence. showCancel=true can be false for a success acknowledgement.
