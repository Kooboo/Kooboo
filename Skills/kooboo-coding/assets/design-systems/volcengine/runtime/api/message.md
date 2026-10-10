# YlMessage

Import and register `{ YlMessage }` from `/yuanli.js`.
Props: `modelValue: { type?: 'success'|'error'|'warning'|'info'|'loading', text: string } | null = null`, `duration: number = 3000` ms (0 keeps it open), `closeLabel = '关闭提示'`. Event: `update:modelValue` emits null on close/timeout. Assign a new object for each message, including repeated identical text.

```html
<YlMessage v-model="message" :duration="3000" />
```

Use `const message = ref(null)`; after await save(), set `message.value = { type: 'success', text: '保存成功' }`; catch assigns an error message. Never claim success before the API resolves.

Loading messages stay visible until replaced or cleared, regardless of duration, and use a reduced-motion-aware spinner. One message floats at the top center without occupying page layout. New messages replace the previous one and restart the timeout. Hover/focus pauses it; leaving restarts the full duration. A close icon dismisses it. Error uses role=alert, others role=status. Text is escaped. Native Popover API, when supported, places it above modal dialogs without stealing focus; fallback is a fixed layer. Mount one feedback instance per business screen, outside dialog slots, so closing the dialog does not destroy it. No imperative global service or stacked queue is promised.

Use this for transient mutation success/failure instead of inline green/red page banners. Keep persistent list-load errors beside their retry action, field-validation errors beside inputs, and notification-inbox content inside the header popover.
