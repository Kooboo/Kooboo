# message

Ant Design Vue 4.2.6; native export `message`. Import `message` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Static message.success/error/info/warning/loading methods render global notices. Destroy indefinite messages on completion/unmount. Static methods do not inherit ConfigProvider; use AApp.useApp().message inside a descendant of AApp for locale/theme context.

## Props and defaults

- `content`: string| VNode | () => VNode; default -. content of the message
- `duration`: number; default 1.5. time(seconds) before auto-dismiss, don't dismiss if set to 0
- `onClose`: function; default -. Specify a function that will be called when the message is closed

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace><AButton @click="message.success('保存成功')">成功消息</AButton><AButton @click="message.error('请求失败')">错误消息</AButton></ASpace>
```

Source: pinned upstream es/components.js, es/message, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
