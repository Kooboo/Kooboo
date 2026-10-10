# AModal

Ant Design Vue 4.2.6; native export `Modal`. Import `AModal` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use v-model:open. @ok does not close a controlled modal automatically: validate, await persistence, then set open=false. Use confirmLoading and guard double submits. Modal.confirm onOk may return a promise; handle cancellation separately. For provider context use AApp.useApp() in a child.

## Props and defaults

- `afterClose`: function; default -. Specify a function that will be called when modal is closed completely.
- `bodyStyle`: object; default {}. Body style for modal body element. Such as height, padding etc.
- `cancelButtonProps`: [ButtonProps](/components/button/#api); default -. The cancel button props
- `cancelText`: string|slot; default `Cancel`. Text of the Cancel button
- `centered`: boolean; default `false`. Centered Modal
- `closable`: boolean; default true. Whether a close (x) button is visible on top right of the modal dialog or not
- `closeIcon`: VNode | slot; default -. custom close icon
- `confirmLoading`: boolean; default false. Whether to apply loading visual effect for OK button or not
- `destroyOnClose`: boolean; default false. Whether to unmount child components on onClose
- `footer`: string|slot; default OK and Cancel buttons. Footer content, set as `:footer="null"` when you don't need default buttons
- `forceRender`: boolean; default false. Force render Modal
- `getContainer`: (instance): HTMLElement; default () => document.body. Return the mount node for Modal
- `mask`: boolean; default true. Whether show mask or not.
- `maskClosable`: boolean; default true. Whether to close the modal dialog when the mask (area outside the modal) is clicked
- `maskStyle`: object; default {}. Style for modal's mask element.
- `okButtonProps`: [ButtonProps](/components/button/#api); default -. The ok button props
- `okText`: string|slot; default `OK`. Text of the OK button
- `okType`: string; default `primary`. Button `type` of the OK button
- `title`: string|slot; default -. The modal dialog's title
- `open(v-model)`: boolean; default false. Whether the modal dialog is visible or not
- `width`: string|number; default 520. Width of the modal dialog
- `wrapClassName`: string; default -. The class name of the container of the modal dialog
- `zIndex`: number; default 1000. The `z-index` of the Modal

## Events

- `cancel`: Specify a function that will be called when a user clicks mask, close button on top right or Cancel button
- `ok`: Specify a function that will be called when a user clicks the OK button

## Slots

- `cancelText`: Text of the Cancel button
- `closeIcon`: custom close icon
- `footer`: Footer content, set as `:footer="null"` when you don't need default buttons
- `okText`: Text of the OK button
- `title`: The modal dialog's title

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AButton @click="open = true">编辑</AButton><AModal v-model:open="open" title="编辑订单" @ok="open = false; event = text"><AInput v-model:value="text"/></AModal><p>{{ event }}</p><AButton @click="confirm">异步确认</AButton>
```

Source: pinned upstream es/components.js, es/modal, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
