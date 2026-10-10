# AApp

Ant Design Vue 4.2.6; native export `App`. Import `AApp` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

AApp provides context holders for message/notification/modal. Call AApp.useApp() in a descendant component, after wrapping its parent with AApp. Calling it in the same setup that renders AApp does not inherit the new provider.

## Props and defaults

- `message`: [MessageConfig](/components/message/#messageconfig); default -. Global config for Message
- `notification`: [NotificationConfig](/components/notification/#notificationconfig); default -. Global config for Notification

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AApp><ACard title="应用上下文"><AButton @click="message.success('示例成功')">显示消息</AButton><p>自定义主题中的上下文消息请在子组件调用 AApp.useApp()。</p></ACard></AApp>
```

Source: pinned upstream es/components.js, es/app, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
