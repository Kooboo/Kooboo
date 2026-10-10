# YlPopconfirm

Controlled anchored confirmation. Props: modelValue=false, title required, description optional; tone=warning (warning/info/error), simple=false, busy=false, confirmText=确认, cancelText=取消.

Events: update:modelValue(boolean), confirm(), cancel(). Parent handles the request and closes after success; failure keeps the panel open and can use the default slot for an error. busy disables confirm/cancel and outside/Escape dismissal. Confirm does not automatically close. trigger slot receives YlPopover {open,toggle,triggerAttrs}; bind triggerAttrs and toggle to a button. default slot is optional content below description.

```html
<YlPopconfirm v-model="open" title="确认删除？" :busy="saving" @confirm="remove">
  <template #trigger="{triggerAttrs,toggle}"><YlButton v-bind="triggerAttrs" @click="toggle">删除</YlButton></template>
</YlPopconfirm>
```
