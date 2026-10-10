# notification

Ant Design Vue 4.2.6; native export `notification`. Import `notification` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

notification.open/success/error({message,description,key}) renders a global notice. Close using its key. Static methods do not inherit provider context; use AApp.useApp().notification in descendants.

## Props and defaults

- `bottom`: string; default `24px`. Distance from the bottom of the viewport, when `placement` is `bottomRight` or `bottomLeft` (unit: pixels).
- `btn`: VNode | () => VNode; default -. Customized close button
- `class`: string; default -. Customized CSS class
- `closeIcon`: VNode | () => VNode; default -. custom close icon
- `description`: string| VNode | () => VNode; default -. The content of notification box (required)
- `duration`: number; default 4.5. Time in seconds before Notification is closed. When set to 0 or null, it will never be closed automatically
- `getContainer`: () => HTMLNode; default () => document.body. Return the mount node for Notification
- `icon`: VNode | () => VNode; default -. Customized icon
- `key`: string; default -. The unique identifier of the Notification
- `message`: string| VNode | () => VNode; default -. The title of notification box (required)
- `placement`: string; default `topRight`. Position of Notification, can be one of `top` `topLeft` `topRight` `bottom` `bottomLeft` `bottomRight`
- `style`: Object | string; default -. Customized inline style
- `top`: string; default `24px`. Distance from the top of the viewport, when `placement` is `topRight` or `topLeft` (unit: pixels).
- `onClick`: Function; default -. Specify a function that will be called when the notification is clicked
- `onClose`: Function; default -. Specify a function that will be called when the close button is clicked

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AButton @click="notification.open({message:'任务完成',description:'已处理 3 个订单'})">打开通知</AButton>
```

Source: pinned upstream es/components.js, es/notification, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
